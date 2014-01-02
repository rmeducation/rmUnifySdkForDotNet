using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RM.Unify.Sdk.SampleApp2.Models
{
    public class School
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayName("DFE code")]
        public string DfeCode { get; set; }

        [Required]
        [DisplayName("Post code")]
        public string PostCode { get; set; }

        [Required]
        [DisplayName("Licenced?")]
        public bool Licenced { get; set; }

        /// <summary>
        /// Get the school of the current logged in user
        /// </summary>
        static public School Current
        {
            get
            {
                return Account.Current.School;
            }
        }

        // RMUNIFY
        public bool IsRmUnifySchool { get; set; }
        public string RmUnifyId { get; set; } // N.B. School can have an RmUnifyId but IsRmUnifySchool=false, because we create IDs in /RMUnify/AppEstablishmentKey
        // END RMUNIFY
    }
}