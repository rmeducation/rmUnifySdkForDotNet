using System.ComponentModel.DataAnnotations;

namespace RM.Unify.Sdk.SampleApp2.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string LoginName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}