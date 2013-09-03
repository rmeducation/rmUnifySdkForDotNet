//-------------------------------------------------
// <copyright file="SsoUser.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;

namespace RM.Unify.Sdk.Client.SsoImpl
{
    internal class SsoUser : RmUnifyUser
    {
        /// <summary>
        /// Construct an RmUnifyUser from a SignInMessage
        /// </summary>
        /// <param name="m">SignInMessage</param>
        internal SsoUser(SignInMessage m)
        {
            RawSsoAttributes = m.Attributes;
            byte[] id;

            PersonId = m.Attributes.GetOrDefault("http://schemas.rm.com/identity/claims/identityguid");
            if (PersonId != null)
            {
                id = new Guid(PersonId).ToByteArray();
            }
            else
            {
                PersonId = m.Attributes.GetOrDefault("http://schemas.rm.com/identity/claims/persistentid");
                id = Convert.FromBase64String(PersonId);
            }

            DisplayName = m.Attributes.GetOrDefault("http://schemas.rm.com/identity/claims/displayname");
            FirstName = m.Attributes.GetOrDefault("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            LastName = m.Attributes.GetOrDefault("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname");
            UnifyUserName = m.Attributes.GetOrDefault("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

            string role = m.Attributes.GetOrDefault("http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            switch (role)
            {
                case "Student":
                    UserRole = Role.Student;
                    break;
                case "TeachingStaff":
                    UserRole = Role.TeachingStaff;
                    break;
                case "NonTeachingStaff":
                    UserRole = Role.NonTeachingStaff;
                    break;
                case "Parent":
                    UserRole = Role.Parent;
                    break;
                case "Governor":
                    UserRole = Role.Governor;
                    break;
                case "Other":
                    UserRole = Role.Other;
                    break;
                default:
                    UserRole = Role.Unknown;
                    break;
            }

            string yearOfEntryStr = m.Attributes.GetOrDefault("http://schemas.rm.com/identity/claims/yearofentry");
            if (yearOfEntryStr != null)
            {
                YearOfEntry = int.Parse(yearOfEntryStr);
            }

            string isUnifyAdminStr = m.Attributes.GetOrDefault("http://schemas.rm.com/identity/claims/isunifyadmin");
            IsUnifyAdmin = (isUnifyAdminStr != null && isUnifyAdminStr.ToLower() == "true");

            UnifyEmailAddress = m.Attributes.GetOrDefault("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

            Organization = new SsoOrganization(m);

            if (id != null && Organization.Id != null)
            {
                // Create short base64 encoding to decrease risk of overflowing existing DB column
                byte[] orgId = new Guid(Organization.Id).ToByteArray();
                byte[] combinedId = new byte[orgId.Length + id.Length];
                Array.Copy(orgId, combinedId, orgId.Length);
                Array.Copy(id, 0, combinedId, orgId.Length, id.Length);
                Id = Convert.ToBase64String(combinedId);
            }
        }
    }
}
