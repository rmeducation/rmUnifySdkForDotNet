using System;
using System.Linq;
using RM.Unify.Sdk.Client;
using RM.Unify.Sdk.SampleApp2.Models;

// RMUNIFY
// Only needed if you are supporting SSO Connectors or the RM Unify user account matching process
namespace RM.Unify.Sdk.SampleApp2.Helpers
{
    /// <summary>
    /// This extends RmUnifyWithUsernames to support for SSO Connectors (http://dev.rmunify.com/reference/supporting-sso-connector-licensing.aspx)
    /// and the RM Unify user account matching process (http://dev.rmunify.com/reference/supporting-user-account-matching/the-rm-unify-process.aspx).
    /// </summary>
    public class RmUnifyWithAccountLinking : RmUnifyWithLoginNames
    {
        /// <summary>
        /// Check that a pre-existing organization that has been linked to RM Unify is licenced in your app.
        /// Only necessary if your app supports SSO Connectors (http://dev.rmunify.com/reference/supporting-sso-connector-licensing.aspx).
        /// In this case, the organization has purchased your app outside RM Unify and wishes to connect RM Unify to this
        /// licence. They get an "app establishment key" from you and enter it into RM Unify. This key is passed in to this
        /// method for you to verify the licence.
        /// Called before UpdateLinkedOrganization() if the organization has an SSO Connector to your app.
        /// </summary>
        /// <param name="appEstablishmentKey">App establishment key (as provided by you to the organization)</param>
        /// <param name="organization">Organization profile</param>
        /// <param name="source">Source of update (sign on or provisioning)</param>
        /// <returns>True if organization licensed, false otherwise</returns>
        public override bool IsOrganizationLicensed(string appEstablishmentKey, RmUnifyOrganization organization, Source source)
        {
            using (var context = new Context())
            {
                var school = (from s in context.Schools
                                where s.RmUnifyId == appEstablishmentKey
                                select s).SingleOrDefault();
                if (school == null)
                {
                    throw new RmUnifySsoException(RmUnifySsoException.ERRORCODES_INVALIDAPPESTABLISHMENTKEY, "No school with app establishment key: " + appEstablishmentKey);
                }
                return school.Licenced;
            }
        }

        /// <summary>
        /// Update properties of a pre-existing organization that has been linked to RM Unify.
        /// Only necessary if your app supports SSO Connectors (http://dev.rmunify.com/reference/supporting-sso-connector-licensing.aspx)
        /// or the RM Unify user account matching process (http://dev.rmunify.com/reference/supporting-user-account-matching/the-rm-unify-process.aspx).
        /// In this case, the organization has obtained your app outside RM Unify and wishes to connect RM Unify to their
        /// existing establishment in your app. They get an "app establishment key" from you and enter it into RM Unify.
        /// This key is passed in to this method for you to identify the establishment and update it.
        /// Called instead of CreateOrUpdateOrganization() if the organization has an SSO Connector to your app
        /// or has linked the organization as part of the RM Unify user account matching process.
        /// </summary>
        /// <param name="appEstablishmentKey">App establishment key (as provided by you to the organization)</param>
        /// <param name="organization">Organization profile</param>
        /// <param name="source">Source of update (sign on or provisioning)</param>
        public override void UpdateLinkedOrganization(string appEstablishmentKey, RmUnifyOrganization organization, Source source)
        {
            using (var context = new Context())
            {
                var school = (from s in context.Schools
                                where s.RmUnifyId == appEstablishmentKey
                                select s).SingleOrDefault();
                if (school == null)
                {
                    throw new RmUnifySsoException(RmUnifySsoException.ERRORCODES_INVALIDAPPESTABLISHMENTKEY, "No school with app establishment key: " + appEstablishmentKey);
                }

                school.Name = organization.Name;
                school.DfeCode = organization.Code;
                school.IsRmUnifySchool = true;
                context.SaveChanges();

                // Cache school for next method
                _school = school;
            }
        }

        /// <summary>
        /// Update properties of a pre-existing user that has been linked to RM Unify.
        /// Only necessary if your app supports the RM Unify user account matching process
        /// (http://dev.rmunify.com/reference/supporting-user-account-matching/the-rm-unify-process.aspx).
        /// In this case, the organization has provisioned users into your app outside RM Unify and wishes to connect RM Unify
        /// to their existing users. Their "app establishment key" and the existing user id in your app (typically login name)
        /// is passed in to this method so that you can verify that the user is in the establishment and update them.
        /// Called instead of CreateOrUpdateUser() if the current user has been linked to a user in your app as part of the
        /// RM Unify user account matching process.
        /// </summary>
        /// <param name="appUserId">User ID in your app (typically login name)</param>
        /// <param name="appEstablishmentKey">App establishment key (as provided by you to the organization)</param>
        /// <param name="user">User profile</param>
        /// <param name="source">Source of update (sign on or provisioning)</param>
        public override void UpdateLinkedUser(string appUserId, string appEstablishmentKey, RmUnifyUser rmUser, Source source)
        {
            if (_school == null)
            {
                throw new Exception("UpdateLinkedUser() called before UpdateLinkedOrganization()");
            }

            using (var context = new Context())
            {
                var account = (from a in context.Accounts
                                where a.LoginName == appUserId
                                select a).SingleOrDefault();
                if (account == null)
                {
                    throw new RmUnifySsoException(RmUnifySsoException.ERRORCODES_INVALIDAPPUSERID, "No such username: " + appUserId);
                }
                if (account.SchoolId != _school.Id)
                {
                    throw new RmUnifySsoException(RmUnifySsoException.ERRORCODES_APPUSERIDNOTINESTABLISHMENT, "User " + appUserId + " is not in school with establishment key " + appEstablishmentKey);
                }

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
        }

        /// <summary>
        /// Log in as a pre-existing user who has been linked to an RM Unify user.
        /// This method is called when single sign on from RM Unify successfully completes.  CreateOrUpdateOrganization()
        /// and CreateOrUpdateUser() will always be called first (in that order) provided the appropriate Ids are available.
        /// Your app should set any session cookies required to log the user in.
        /// Your app should then redirect the user to returnUrl (if set) or to the default URL for this user in your app.
        /// </summary>
        /// <param name="appUserId">User ID in your app (typically login name)</param>
        /// <param name="appEstablishmentKey">App establishment key (as provided by you to the organization)</param>
        /// <param name="user">User profile</param>
        /// <param name="maxSessionEnd">Maxiumum time after which reauthentication should be prompted</param>
        /// <param name="returnUrl">Return URL specified in login request (null if none)</param>
        public override void DoLoginForLinkedUser(string appUserId, string appEstablishmentKey, RmUnifyUser user, DateTime maxSessionEnd, string returnUrl)
        {
            if (_account == null)
            {
                throw new Exception("DoLoginForLinkedUser() called before UpdateLinkedUser()");
            }

            if (_account.SchoolId != _school.Id)
            {
                throw new RmUnifySsoException(RmUnifySsoException.ERRORCODES_APPUSERIDNOTINESTABLISHMENT, "User " + appUserId + " is not in school with establishment key " + appEstablishmentKey);
            }

            // Can just call RmUnify.DoLogin() because the user to login as is already stored in _account
            DoLogin(user, maxSessionEnd, returnUrl);
        }
    }
}
// END RMUNIFY