using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using RM.Unify.Sdk.Client;
using RM.Unify.Sdk.SampleApp2.Models;

// RMUNIFY
namespace RM.Unify.Sdk.SampleApp2.Helpers
{
    /// <summary>
    /// This is a basic implementation of the RM Unify callback API
    /// 
    /// It uses the rmUnifyUser.Id value as the LoginName for the user; this guarantees a unique LoginName
    /// that can be written to the authentication cookie, but means that the app cannot display the LoginName.
    /// 
    /// If you have a need to display LoginName (and cannot change the app to use DisplayName instead), see
    /// RmUnifyWithLoginNames.cs.
    /// 
    /// This implementation provides no support for account linking and SSO connectors.  See RmUnifyWithAccountLinking.cs
    /// for this.
    /// </summary>
    public class RmUnify : RmUnifyCallbackApi
    {
        // Because this class is created once per web request, we can hold temporary data in some private fields
        protected School _school = null;
        protected Account _account = null;

        /// <summary>
        /// The WS-Federation realm of your app, as configured in RM Unify
        /// This must be a URL in a domain that you own, but need not actually exist.
        /// For example, if you are working on a development copy of your app, and your live app is hosted at
        /// https://myapp.mycompany.com/, you might choose the URL https://dev.myapp.mycompany.com/realm.
        /// </summary>
        public override string Realm
        {
            get { return "https://rmunifysampleapp.rm.com/realm"; }
        }

        /// <summary>
        /// Create a new organization or update an existing one.
        /// This method will never be called if the organization.Id is null (for example, if the attribute
        /// has not been requested from RM Unify).
        /// If your app stores information about an organization, it should use organization.Id as a key to create a
        /// new organization record or update an existing one.
        /// </summary>
        /// <param name="org">Organization profile</param>
        /// <param name="source">Source of update (sign on or provisioning)</param>
        public override void CreateOrUpdateOrganization(RmUnifyOrganization org, Source source)
        {
            using (var context = new Context())
            {
                School school = (from s in context.Schools
                                 where s.RmUnifyId == org.Id
                                 select s).SingleOrDefault();
                if (school == null)
                {
                    school = new School()
                    {
                        RmUnifyId = org.Id
                    };
                    context.Schools.Add(school);
                }
                school.Name = org.Name;
                school.DfeCode = org.Code;
                school.PostCode = "N/A";
                school.IsRmUnifySchool = true;
                context.SaveChanges();

                // Cache school for next method
                _school = school;
            }
        }

        /// <summary>
        /// Create a new user or update an existing one.
        /// This method will never be called if the user.Id is null (for example, if neither IdentityGuid nor
        /// PersistentId has been requested from RM Unify).
        /// If your app stores information about a user, it should use user.Id as a key to create a new user record or
        /// update an existing one.
        /// Be aware that it is possible for a user to move organization (i.e. the same user.Id may appear in a different
        /// organization.Id).  In this case, it is safe to delete the old user profile.
        /// </summary>
        /// <param name="user">User profile</param>
        /// <param name="source">Source of update (sign on or provisioning)</param>
        public override void CreateOrUpdateUser(RmUnifyUser rmUser, Source source)
        {
            if (_school == null)
            {
                throw new Exception("CreateOrUpdateUser() called before CreateOrUpdateOrganization()");
            }

            using (var context = new Context())
            {
                Account account = (from a in context.Accounts
                                   where a.RmUnifyId == rmUser.Id
                                   select a).SingleOrDefault();
                if (account == null)
                {
                    account = new Account()
                    {
                        RmUnifyId = rmUser.Id,
                        SchoolId = _school.Id,
                        Password = Guid.NewGuid().ToString() // use random unguessable password
                    };
                    context.Accounts.Add(account);
                }
                else
                {
                    if (account.SchoolId != _school.Id)
                    {
                        // If you use rmUnifyUser.PersonId, you will need to support a user moving between schools
                        // If you use rmUnifyUser.Id, a user moving between schools will be assigned a new Id
                        throw new Exception("School moves not supported");
                    }
                }

                account.LoginName = rmUser.Id;
                account.DisplayName = rmUser.DisplayName;
                account.RoleEnum = GetRole(rmUser);
                account.DeletedDate = null;  // if previously deleted, restore
                if (source == Source.SingleSignOn)
                {
                    account.LastLogin = DateTime.Now;
                }
                context.SaveChanges();

                // Cache account for next method
                _account = account;
            }

            // Purge any old users from the system
            // In this example, we've chosen to implement this in the login process
            // If this is likely to be long running, we might want to implement it as a background task
            PurgeUsers();
        }

        /// <summary>
        /// Log the current user into your app.
        /// This method is called when single sign on from RM Unify successfully completes.  CreateOrUpdateOrganization()
        /// and CreateOrUpdateUser() will always be called first (in that order) provided the appropriate Ids are available.
        /// Your app should set any session cookies required to log the user in.
        /// Your app should then redirect the user to returnUrl (if set) or to the default URL for this user in your app.
        /// </summary>
        /// <param name="user">User profile</param>
        /// <param name="maxSessionEnd">Maxiumum time after which reauthentication should be prompted</param>
        /// <param name="returnUrl">Return URL specified in login request (null if none)</param>
        public override void DoLogin(RmUnifyUser user, DateTime maxSessionEnd, string returnUrl)
        {
            if (_account == null)
            {
                throw new Exception("DoLogin() called before CreateOrUpdateUser()");
            }

            // Create our own cookie so we can make sure session length is appropriate.
            // Calculate session expiry as minimum of our app policy and that provided by RM Unify
            DateTime endTime = DateTime.Now.Add(FormsAuthentication.Timeout);
            if (endTime > maxSessionEnd)
            {
                endTime = maxSessionEnd;
            }

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2,
                _account.LoginName,
                DateTime.Now,
                endTime,
                false,
                "",
                FormsAuthentication.FormsCookiePath);
            string encTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            cookie.HttpOnly = true;

            // If your website is entirely https, it is good practise to mark the cookie as secure
            // cookie.Secure = true;
            HttpContext.Current.Response.Cookies.Add(cookie);

            HttpContext.Current.Response.Redirect(returnUrl == null ? "/" : returnUrl, true);
        }

        /// <summary>
        /// Log the current user out of your app.
        /// This method is called when single logout is initiated from RM Unify.  If should delete any session cookies
        /// associated with your app.  The method should *not* error if there is no session.
        /// You may also wish any open app tabs to redirect to a logged out page.  This can be
        /// achieved by regularly checking for the existence of a session cookie using JavaScript, and disabling the app if
        /// none is found.
        /// </summary>
        public override void DoLogout()
        {
            FormsAuthentication.SignOut();
        }

        #region Private methods

        protected Role GetRole(RmUnifyUser rmUser)
        {
            if (rmUser.IsUnifyAdmin)
            {
                return Role.Admin;
            }

            switch (rmUser.UserRole)
            {
                case RmUnifyUser.Role.TeachingStaff:
                case RmUnifyUser.Role.NonTeachingStaff:
                    return Role.Staff;
                case RmUnifyUser.Role.Parent:
                    return Role.Parent;
                case RmUnifyUser.Role.Student:
                    return Role.Student;
            }

            return Role.Guest;
        }

        protected void PurgeUsers()
        {
            using (var context = new Context())
            {
                // Purge users who were deleted more than a year ago
                // Don't purge users who have authored blog posts (because that will break attribution on the blog post)
                DateTime minDeleted = DateTime.Now.AddMonths(-12);
                var toPurge = from a in context.Accounts
                              where a.School.IsRmUnifySchool == true && a.DeletedDate != null && a.DeletedDate < minDeleted
                                && !(from p in context.Posts select p.AccountId).Contains(a.Id)
                              select a;
                foreach (var account in toPurge)
                {
                    context.Accounts.Remove(account);
                }

                // Soft delete users who last logged in more than 3 months ago
                DateTime minLastLogin = DateTime.Now.AddMonths(-3);
                var toDelete = from a in context.Accounts
                               where a.School.IsRmUnifySchool == true && a.DeletedDate == null && a.LastLogin != null && a.LastLogin < minLastLogin
                               select a;
                foreach (var account in toDelete)
                {
                    account.DeletedDate = DateTime.Now;
                }

                context.SaveChanges();
            }
        }

        #endregion
    }
}
// END RMUNIFY