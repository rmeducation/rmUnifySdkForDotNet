using System.ComponentModel;

// RMUNIFY
// Only needed if you are supporting SSO Connectors or the RM Unify user account matching process
namespace RM.Unify.Sdk.SampleApp2.ViewModels
{
    public class RmUnifyAppEstablishmentKeyViewModel
    {
        [DisplayName("Establishment key")]
        public string AppEstablishmentKey { get; set;  }
    }
}
// END RMUNIFY