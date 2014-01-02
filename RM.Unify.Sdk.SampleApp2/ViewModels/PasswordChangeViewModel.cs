using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RM.Unify.Sdk.SampleApp2.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Required]
        [DisplayName("Old Password")]
        public string OldPassword { get; set; }
        [Required]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }
    }
}