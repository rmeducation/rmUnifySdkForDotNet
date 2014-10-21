//-------------------------------------------------
// <copyright file="SsoUser.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace RM.Unify.Sdk.Client.SsoImpl
{
    internal class SsoUser : RmUnifyUser
    {
        private string _id = "";

        /// <summary>
        /// Construct an RmUnifyUser from a SignInMessage
        /// </summary>
        /// <param name="m">SignInMessage</param>
        internal SsoUser(SignInMessage m)
        {
            RawSsoAttributes = m.Attributes;
            Organization = new SsoOrganization(this);
        }

        public override string Id
        {
            get
            {
                if (_id != "")
                {
                    return _id;
                }

                string idstr;
                byte[] id = null;
                RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/identityguid", out idstr);

                if (idstr != null)
                {
                    id = new Guid(idstr).ToByteArray();
                }
                else
                {
                    RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/persistentid", out idstr);
                    if (idstr != null)
                    {
                        id = Convert.FromBase64String(idstr);
                    }
                }

                if (id != null)
                {
                    string orgstr = this.Organization.Id;
                    if (orgstr != null)
                    {
                        byte[] orgId = new Guid(Organization.Id).ToByteArray();
                        byte[] combinedId = new byte[orgId.Length + id.Length];
                        Array.Copy(orgId, combinedId, orgId.Length);
                        Array.Copy(id, 0, combinedId, orgId.Length, id.Length);
                        _id = Convert.ToBase64String(combinedId);
                    }
                }

                return _id == "" ? null : _id;
            }
        }

        public override string PersonId
        {
            get
            {
                string personId;

                RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/identityguid", out personId);
                if (personId == null)
                {
                    RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/persistentid", out personId);
                }

                return personId;
            }
        }

        public override string DisplayName
        {
            get
            {
                string val;
                RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/displayname", out val);
                return val;
            }
        }

        public override string FirstName
        {
            get
            {
                string val;
                RawSsoAttributes.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", out val);
                return val;
            }
        }

        public override string LastName
        {
            get
            {
                string val;
                RawSsoAttributes.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", out val);
                return val;
            }
        }

        public override string UnifyUserName
        {
            get
            {
                string val;
                RawSsoAttributes.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", out val);
                return val;
            }
        }

        public override Role UserRole
        {
            get
            {
                string roleStr;
                RawSsoAttributes.TryGetValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", out roleStr);
                switch (roleStr)
                {
                    case "Student":
                        return Role.Student;
                    case "TeachingStaff":
                        return Role.TeachingStaff;
                    case "NonTeachingStaff":
                        return Role.NonTeachingStaff;
                    case "Parent":
                        return Role.Parent;
                    case "Governor":
                        return Role.Governor;
                    case "Other":
                        return Role.Other;
                }
                return Role.Unknown;
            }
        }

        public override int? YearOfEntry
        {
            get
            {
                string yearOfEntryStr;
                RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/yearofentry", out yearOfEntryStr);
                if (yearOfEntryStr != null)
                {
                    return int.Parse(yearOfEntryStr);
                }
                return null;
            }
        }

        public override bool IsUnifyAdmin
        {
            get
            {
                string isUnifyAdminStr;
                RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/isunifyadmin", out isUnifyAdminStr);
                return (isUnifyAdminStr != null && isUnifyAdminStr.ToLower() == "true");
            }
        }

        public override string UnifyEmailAddress
        {
            get
            {
                string val;
                RawSsoAttributes.TryGetValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", out val);
                return val;
            }
        }

        public override string AppUserId
        {
            get
            {
                string val;
                RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/appuserid", out val);
                return string.IsNullOrEmpty(val) ? null : val;
            }
        }

        public override string MisId
        {
            get
            {
                string val;
                RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/misidentifier", out val);
                return string.IsNullOrEmpty(val) ? null : val;
            }
        }

        public override string RegGroup
        {
            get
            {
                string val;
                RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/registrationgroup", out val);
                return string.IsNullOrEmpty(val) ? null : val;
            }
        }

        public override string UPN
        {
            get
            {
                string val;
                RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/uniquepupilnumber", out val);
                return string.IsNullOrEmpty(val) ? null : val;
            }
        }

        public override string SCN
        {
            get
            {
                string val;
                RawSsoAttributes.TryGetValue("http://schemas.rm.com/identity/claims/scottishcandidatenumber", out val);
                return string.IsNullOrEmpty(val) ? null : val;
            }
        }
    }
}
