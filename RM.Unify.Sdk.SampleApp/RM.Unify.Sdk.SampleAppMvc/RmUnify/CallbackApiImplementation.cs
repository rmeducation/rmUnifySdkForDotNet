using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RM.Unify.Sdk.Client;
using RM.Unify.Sdk.SampleAppMvc.Models;
using System.Web.Security;
using WebMatrix.WebData;

namespace RM.Unify.Sdk.SampleAppMvc.RmUnify
{
    public class CallbackApiImplementation : RmUnifyCallbackApi
    {
        public override string Realm
        {
            get { return "https://rmunifysampleapp.rm.com/realm"; }
        }

        public override void CreateOrUpdateOrganization(RmUnifyOrganization organization, Source source)
        {
            using (var context = new UsersContext())
            {
                // Get the school (if it exists)
                School school = (from s in context.Schools
                              where s.RmUnifyOrganizationId == organization.Id
                              select s).SingleOrDefault();

                if (school == null)
                {
                    // School does not exist - create
                    school = new School()
                    {
                        RmUnifyOrganizationId = organization.Id,
                        DisplayName = organization.Name
                    };
                    context.Schools.Add(school);
                    context.SaveChanges();
                }
                else
                {
                    // School exists - update
                    if (school.Deleted != null || school.RmUnifyOrganizationId != organization.Id)
                    {
                        school.Deleted = null;
                        school.RmUnifyOrganizationId = organization.Id;
                        context.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Create or update a user in the application
        /// In this example, we want to store some custom data about the user (display name, last login and deleted date).
        /// Last login and deleted date allow us to soft delete users who haven't logged in for 3 months and then purge
        /// them after 1 year.
        /// We've implement the custom data by just extending the account model and going direct to the database to read
        /// and write the data; we could have equally created a custom membership user
        /// (http://msdn.microsoft.com/en-us/library/ms366730.aspx).
        /// </summary>
        /// <param name="rmUser">RM Unify user</param>
        /// <param name="source">Source of user information (always SingleSignOn at the moment)</param>
        public override void  CreateOrUpdateUser(RmUnifyUser rmUser, Source source)
        {
            // Update the data associated with the user
            CreateOrUpdateUserData(rmUser);

            // Update the roles associated with the user
            CreateOrUpdateUserRoles(rmUser);

            // Purge any old users from the system
            // In this example, we've chosen to implement this in the login process
            // If this is likely to be long running, we might want to implement it as a background task
            PurgeUsers();
        }

        public override void DoLogin(RmUnifyUser user, DateTime maxSessionEnd, string returnUrl)
        {
            // Create our own cookie so we can insert DisplayName (because we want to use it on every page)
            // and make sure session length is appropriate.
            // If we didn't need this, we could just use FormsAuthentication.SetAuthCookie(user.Id, false)

            // Calculate session expiry as minimum of our app policy and that provided by RM Unify
            DateTime endTime = DateTime.Now.Add(FormsAuthentication.Timeout);
            if (endTime > maxSessionEnd)
            {
                endTime = maxSessionEnd;
            }
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2,
                user.Id,
                DateTime.Now,
                endTime,
                false,
                user.DisplayName,
                FormsAuthentication.FormsCookiePath);
            string encTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            cookie.HttpOnly = true;

            // If your website is entirely https, it is good practise to mark the cookie as secure
            // cookie.Secure = true;
            HttpContext.Current.Response.Cookies.Add(cookie);

            HttpContext.Current.Response.Redirect(returnUrl == null ? "/" : returnUrl);
        }

        public override void DoLogout()
        {
            FormsAuthentication.SignOut();
        }

        private void CreateOrUpdateUserData(RmUnifyUser rmUser)
        {
            using (var context = new UsersContext())
            {
                // Get the user (if they exist)
                UserProfile user = (from u in context.UserProfiles
                                    where u.UserName == rmUser.Id
                                    select u).SingleOrDefault();

                if (user == null)
                {
                    // User does not exist - create
                    School school = (from s in context.Schools
                                     where s.RmUnifyOrganizationId == rmUser.Organization.Id
                                     select s).SingleOrDefault();
                    var userdata = new
                    {
                        DisplayName = rmUser.DisplayName,
                        SchoolId = school.Id,
                        LastLogin = DateTime.Now
                    };
                    WebSecurity.CreateUserAndAccount(rmUser.Id, Guid.NewGuid().ToString(), userdata);
                }
                else
                {
                    // User exists - update
                    // We don't need to worry about the user moving school as we are using rmUser.Id
                    // (which will change if the user moves school) rather than rmUser.PersonId (which may not)
                    if (rmUser.DisplayName != user.DisplayName)
                    {
                        user.DisplayName = rmUser.DisplayName;
                    }
                    if (user.Deleted != null)
                    {
                        user.Deleted = null;
                    }
                    user.LastLogin = DateTime.Now;
                    context.SaveChanges();
                }
            }
        }

        private void CreateOrUpdateUserRoles(RmUnifyUser rmUser)
        {
            string rmRole = null;

            switch (rmUser.UserRole)
            {
                case RmUnifyUser.Role.TeachingStaff:
                case RmUnifyUser.Role.NonTeachingStaff:
                    rmRole = "staff";
                    break;
                case RmUnifyUser.Role.Student:
                    rmRole = "student";
                    break;
            }
            if (rmUser.IsUnifyAdmin)
            {
                rmRole = "admin";
            }
            if (rmRole == null)
            {
                throw new Exception("Sorry, this application does not support users with your role");
            }

            var rolesProvider = (SimpleRoleProvider)Roles.Provider;
            var currentRoles = rolesProvider.GetRolesForUser(rmUser.Id);
            if (!currentRoles.Contains(rmRole))
            {
                var allRoles = rolesProvider.GetAllRoles();
                if (!allRoles.Contains("staff"))
                {
                    rolesProvider.CreateRole("staff");
                }
                if (!allRoles.Contains("student"))
                {
                    rolesProvider.CreateRole("student");
                }
                if (!allRoles.Contains("admin"))
                {
                    rolesProvider.CreateRole("admin");
                }
                rolesProvider.RemoveUsersFromRoles(new string[] { rmUser.Id }, currentRoles.Intersect(new string[] { "staff", "student", "admin" }).ToArray());
                rolesProvider.AddUsersToRoles(new string[] { rmUser.Id }, new string[] { rmRole });
            }
        }

        private void PurgeUsers()
        {
            DateTime minLastLogin = DateTime.Now.AddMonths(-3);
            DateTime minDeleted = DateTime.Now.AddMonths(-12);
            DateTime now = DateTime.Now;

            using (var context = new UsersContext())
            {
                var toDelete = from u in context.UserProfiles
                               where u.Deleted == null && u.LastLogin != null && u.LastLogin < minLastLogin
                               select u;
                if (toDelete.Count() > 0)
                {
                    foreach (var user in toDelete)
                    {
                        user.Deleted = now;
                    }
                    context.SaveChanges();
                }

                var toPurge = from u in context.UserProfiles
                              where u.Deleted != null && u.Deleted < minDeleted
                              select u;
                if (toPurge.Count() > 0)
                {
                    var rolesProvider = (SimpleRoleProvider)Roles.Provider;
                    var membershipProvider = (SimpleMembershipProvider)Membership.Provider;
                    foreach (var user in toPurge)
                    {
                        // TODO: delete any associated data in the app
                        rolesProvider.RemoveUsersFromRoles(new string[] { user.UserName }, rolesProvider.GetRolesForUser(user.UserName));
                        membershipProvider.DeleteUser(user.UserName, true);
                    }
                }
            }
        }
    }
}