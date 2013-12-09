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
        private class Licence
        {
            internal string description = null;
            internal bool isFreeTrial = false;
            internal bool isSsoConnector = false;
        }

        private SsoUser _user = null;
        private Licence _licence = null;
        static Regex licenceRegex = new Regex(@"^\{IsTrial:(True|False)\|IsConnector:(True|False)\|Description:(.*)\}$", RegexOptions.IgnoreCase);

        internal SsoOrganization(SsoUser user)
        {
            _user = user;
        }

        public override string Id
        {
            get
            {
                string val;
                _user.RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/organisationid", out val);
                return val;
            }
        }

        public override string Name
        {
            get
            {
                string val;
                _user.RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/organisationName", out val);
                return val;
            }
        }

        public override string Code
        {
            get
            {
                string val;
                _user.RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/organisationCode", out val);
                return val;
            }
        }

        public override string LicenceDescription
        {
            get
            {
                if (_licence == null)
                {
                    ReadLicence();
                }
                return _licence.description;
            }
        }

        public override bool IsSsoConnector
        {
            get
            {
                if (_licence == null)
                {
                    ReadLicence();
                }
                return _licence.isSsoConnector;
            }
        }

        public override bool IsFreeTrial
        {
            get
            {
                if (_licence == null)
                {
                    ReadLicence();
                }
                return _licence.isFreeTrial;
            }
        }

        public override string AppEstablishmentKey
        {
            get
            {
                string val;
                _user.RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/appestablishmentkey", out val);
                return string.IsNullOrEmpty(val) ? null : val;
            }
        }

        private void ReadLicence()
        {
            _licence = new Licence();
            string licenceStr;
            _user.RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/applicence", out licenceStr);
            if (!string.IsNullOrEmpty(licenceStr))
            {
                Match match = licenceRegex.Match(licenceStr);
                if (match.Success)
                {
                    _licence.isFreeTrial = bool.Parse(match.Groups[1].Value);
                    _licence.isSsoConnector = bool.Parse(match.Groups[2].Value);
                    string description = match.Groups[3].Value.Trim();
                    _licence.description = string.IsNullOrEmpty(description) ? null : description;
                }
            }
        }
    }
}
