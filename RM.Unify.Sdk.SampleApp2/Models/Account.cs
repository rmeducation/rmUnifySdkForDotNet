using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace RM.Unify.Sdk.SampleApp2.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        // EntityFramework does not support enum on .NET 4, so use int
        public int Role { get; set; }
        public int SchoolId { get; set; }
        public virtual School School { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? DeletedDate { get; set; }

        // RMUNIFY
        public string RmUnifyId { get; set; }
        // END RMUNIFY

        [NotMapped]
        public Role RoleEnum
        {
            get
            {
                return (Role) Role;
            }
            set
            {
                Role = (int) value;
            }
        }

        /// <summary>
        /// Get the account of the current logged in user
        /// </summary>
        static public Account Current
        {
            get
            {
                Account account = null;

                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    account = HttpContext.Current.Items["account"] as Account;
                    if (account == null)
                    {
                        using (var context = new Context())
                        {
                            account = (from a in context.Accounts.Include("School")
                                       where a.LoginName == HttpContext.Current.User.Identity.Name
                                         && a.DeletedDate == null
                                       select a).SingleOrDefault();
                            HttpContext.Current.Items["account"] = account;
                        }
                    }
                }
                return account;
            }
        }
    }
}