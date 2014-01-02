using System;
using System.Linq;
using System.Text.RegularExpressions;
using RM.Unify.Sdk.Client;
using RM.Unify.Sdk.SampleApp2.Models;

// RMUNIFY
// Only needed if you need to display login names (rather than display names) in your application
namespace RM.Unify.Sdk.SampleApp2.Helpers
{
    /// <summary>
    /// This class extends the basic RM Unify callback API implementation by allowing the user to have
    /// a login name that is suitable for display, but avoids collisions with existing login names.
    /// 
    /// This login name is for presentation only; it is based on the RM Unify login name but will be
    /// modified if this would cause a collision.
    /// </summary>
    public class RmUnifyWithLoginNames : RmUnify
    {
        /// <summary>
        /// Create a new organization or update an existing one.
        /// This method will never be called if the organization.Id is null (for example, if the attribute
        /// has not been requested from RM Unify).
        /// If your app stores information about an organization, it should use organization.Id as a key to create a
        /// new organization record or update an existing one.
        /// </summary>
        /// <param name="organization">Organization profile</param>
        /// <param name="source">Source of update (sign on or provisioning)</param>
        public override void CreateOrUpdateUser(RmUnifyUser rmUser, Source source)
        {
            /// This override supports giving each user a login name that is suitable for display
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

                account.LoginName = DeDupeLoginName(rmUser.UnifyUserName, rmUser.Id);
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
        /// Make sure that the login name supplied for an RM Unify user is unique
        /// This is only necessary if we are using RM Unify login names - which we only need to do if our
        /// app displays a login name.  Otherwise, we can simply store the rmUnifyUser.Id property in
        /// place of a login name.
        /// </summary>
        /// <param name="loginName">Login name</param>
        /// <param name="rmUnifyId">RM Unify User ID</param>
        /// <returns>New username</returns>
        private static string DeDupeLoginName(string loginName, string rmUnifyId)
        {
            using (var context = new Context())
            {
                // Should always return 0 or 1 matches, but we'll cope with the case of already having a username clash
                var matchingAccounts = (from a in context.Accounts
                                        where a.LoginName == loginName
                                        select a).ToList();

                // Are there any accounts which were not created by RM Unify with a matching login name?
                foreach (var account in matchingAccounts)
                {
                    // Note that we need to test RmUnifyId == null, not School.IsRmUnifySchool = false.
                    // This is because School.IsRmUnifySchool = true will include accounts which have been linked to RM Unify
                    // (where login name does matter - see RmUnifyWithAccountLinking.cs); accounts with RmUnifyId != null
                    // were *created* by RM Unify - in this case login name is not important.
                    if (account.RmUnifyId == null)
                    {
                        // If there are non-RM Unify accounts, we'll need to find a different login name for this account
                        return DeDupeLoginName(IncrementLoginName(loginName), rmUnifyId);
                    }
                }

                // Rename any existing RM Unify accounts that match loginName
                // We rename the matching accounts rather than the current one as we assume that the matching accounts are defunct
                foreach (var account in matchingAccounts)
                {
                    if (account.RmUnifyId != null && account.RmUnifyId != rmUnifyId)
                    {
                        account.LoginName = DeDupeLoginName(IncrementLoginName(account.LoginName), account.RmUnifyId);
                    }
                }

                context.SaveChanges();
            }

            return loginName;
        }

        /// <summary>
        /// Try to create a unique login name by appending "_2", "_3" etc. to the login name supplied by RM Unify.
        /// The RM Unify login name would normally be for the form username@scope.  To avoid confusion, we will
        /// make a unique login by transforming this to username_X@scope.
        /// </summary>
        /// <param name="currentLoginName">Login name to be incremented</param>
        /// <returns>New login name</returns>
        private static string IncrementLoginName(string currentLoginName)
        {
            string regex = @"^(.+?)(_\d+)?(\@[^\@]*)?$";
            MatchCollection matches = Regex.Matches(currentLoginName, regex);
            if (matches.Count != 1)
            {
                throw new Exception("Unexpected login name format: " + currentLoginName);
            }

            GroupCollection groups = matches[0].Groups;
            string login = groups[1].Value;
            string increment = groups[2].Value;
            string suffix = groups[3].Value;
            if (string.IsNullOrEmpty(increment))
            {
                increment = "_2";
            }
            else
            {
                increment = "_" + (int.Parse(increment.Substring(1)) + 1).ToString();
            }

            return login + increment + suffix;
        }
    }
}
// END RMUNIFY