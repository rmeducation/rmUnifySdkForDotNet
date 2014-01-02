using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RM.Unify.Sdk.SampleApp2.Models;

namespace RM.Unify.Sdk.SampleApp2.ViewModels
{
    public class AccountViewModel
    {
        public int Id;

        [Required]
        [StringLength(20, MinimumLength=4)]
        [DisplayName("Login")]
        public string LoginName { get; set; }
        [DisplayName("Password")]
        public string Password { get; set; }
        [Required]
        [DisplayName("Display name")]
        public string DisplayName { get; set; }
        [Required]
        [DisplayName("Role")]
        public Role RoleEnum { get; set; }
        [DisplayName("Last login")]
        public DateTime? LastLogin { get; set; }

        public bool CanEditLoginName { get; set; }
        public bool CanEditPassword { get; set; }
        public bool CanEditDisplayName { get; set; }
        public bool CanEditRole { get; set; }
        public bool CanEditAnything
        {
            get
            {
                return CanEditLoginName || CanEditPassword || CanEditDisplayName || CanEditRole;
            }
        }

        public AccountViewModel()
        {
        }

        public AccountViewModel(Account acc)
        {
            Id = acc.Id;
            LoginName = (acc.LoginName == null) ? "" : acc.LoginName;
            DisplayName = (acc.DisplayName == null) ? "" : acc.DisplayName;
            LastLogin = acc.LastLogin;
            RoleEnum = acc.RoleEnum;
        }
    }
}