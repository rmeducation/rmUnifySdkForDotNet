//-------------------------------------------------
// <copyright file="RmUnifyUser.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System.Collections.Generic;

namespace RM.Unify.Sdk.Client
{
    /// <summary>
    /// Representation of an RM Unify user
    /// </summary>
    public class RmUnifyUser
    {
        /// <summary>
        /// Roles that a user can have in their organization
        /// </summary>
        public enum Role
        {
            Student,
            TeachingStaff,
            NonTeachingStaff,
            Parent,
            Governor,
            Other,
            Unknown
        }

        /// <summary>
        /// Unique identifier for this user at this organization; guaranteed not to change
        /// This will be the Organization Id plus either IdentityGuid (if requested) or PersistentId
        /// If OrganizationId has not been requested, or neither IdentityGuid or PersistentId have been requested, this
        /// will be null
        /// </summary>
        public string Id
        {
            get;
            protected set;
        }

        /// <summary>
        /// Unique identifer for this user (can persist when users move organization if the organization's share an AD)
        /// This will be eiether the IdentityGuid (if requested) or PersistentId
        /// </summary>
        public string PersonId
        {
            get;
            protected set;
        }

        /// <summary>
        /// Display name for this user
        /// </summary>
        public string DisplayName
        {
            get;
            protected set;
        }

        /// <summary>
        /// First name for this user (if requested)
        /// Normally available, but may be computed from display name or empty string if not in data source; we recommend that you also cope with nulls
        /// </summary>
        public string FirstName
        {
            get;
            protected set;
        }

        /// <summary>
        /// Last name for this user (if requested)
        /// Normally available, but may be computed from display name if not in data source; we recommend that you also cope with nulls
        /// </summary>
        public string LastName
        {
            get;
            protected set;
        }

        /// <summary>
        /// User name for this user in RM Unify (if requested)
        /// WARNING: usernames can change; please do not use this value as your primary identifier for the user
        /// This will have the form username@organizationscope
        /// </summary>
        public string UnifyUserName
        {
            get;
            protected set;
        }

        /// <summary>
        /// User's role in organization (if requested)
        /// </summary>
        public Role UserRole
        {
            get;
            protected set;
        }

        /// <summary>
        /// User's theoretical year of entry into education
        /// This is the year that the student's current year group started in Year 1 (England and Wales) or Primary 1 (Scotland); if
        /// the student skips or repeats a year, then YearOfEntry will change to match their new year group
        /// This value can be in the future if the student is in a reception / nursery class
        /// Not available (null) for non-students and for students where the data source does not contain year of entry information
        /// </summary>
        public int? YearOfEntry
        {
            get;
            protected set;
        }

        /// <summary>
        /// Whether the user is an administrator of RM Unify for their organization
        /// </summary>
        public bool IsUnifyAdmin
        {
            get;
            protected set;
        }

        /// <summary>
        /// The email address that this user has been assigned in RM Unify (typically used for accessing O365 or Google Apps)
        /// Not available (null) if the user has not yet been given an email address in RM Unify
        /// </summary>
        public string UnifyEmailAddress
        {
            get;
            protected set;
        }

        /// <summary>
        /// The raw attributes from single sign on
        /// Only needed for advanced scenarios (i.e. if using a non-standard attribute)
        /// Not available (null) if the RmUnifyUser object was not generated from a single sign on request
        /// </summary>
        public IDictionary<string, string> RawSsoAttributes
        {
            get;
            protected set;
        }

        /// <summary>
        /// The organization that the user belongs to
        /// </summary>
        public RmUnifyOrganization Organization
        {
            get;
            protected set;
        }
    }
}
