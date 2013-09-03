//-------------------------------------------------
// <copyright file="SsoOrganization.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System.Text.RegularExpressions;

namespace RM.Unify.Sdk.Client.SsoImpl
{
    internal class SsoOrganization : RmUnifyOrganization
    {
        /// <summary>
        /// Construct an RmUnifyOrganization from a SignInMessage
        /// </summary>
        /// <param name="m">SignInMessage</param>
        internal SsoOrganization(SignInMessage m)
        {
            Id = m.Attributes.GetOrDefault("http://schemas.rm.com/identity/claims/organisationid");
            Name = m.Attributes.GetOrDefault("http://schemas.rm.com/identity/claims/organisationName");
            Code = m.Attributes.GetOrDefault("http://schemas.rm.com/identity/claims/organisationCode");
        }

    }
}
