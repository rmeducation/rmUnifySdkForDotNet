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
    /// Representation of an RM Unify user.
    /// </summary>
    public abstract class RmUnifyUser
    {
        /// <summary>
        /// Roles that a user can have in their organization.
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
        /// Unique identifier for this user at this organization; guaranteed not to change.
        /// This will be the Organization Id plus either IdentityGuid (if requested) or PersistentId.
        /// If OrganizationId has not been requested, or neither IdentityGuid or PersistentId have been requested, this
        /// will be null.
        /// </summary>
        public abstract string Id
        {
            get;
        }

        /// <summary>
        /// Unique identifer for this user (can persist when users move organization if the organization's share an AD).
        /// This will be eiether the IdentityGuid (if requested) or PersistentId.
        /// </summary>
        public abstract string PersonId
        {
            get;
        }

        /// <summary>
        /// Display name for this user
        /// </summary>
        public abstract string DisplayName
        {
            get;
        }

        /// <summary>
        /// First name for this user (if requested).
        /// Normally available, but may be computed from display name or empty string if not in data source; we recommend that you also cope with nulls.
        /// </summary>
        public abstract string FirstName
        {
            get;
        }

        /// <summary>
        /// Last name for this user (if requested).
        /// Normally available, but may be computed from display name if not in data source; we recommend that you also cope with nulls.
        /// </summary>
        public abstract string LastName
        {
            get;
        }

        /// <summary>
        /// User name for this user in RM Unify (if requested).
        /// WARNING: usernames can change; please do not use this value as your primary identifier for the user.
        /// This will have the form username@organizationscope.
        /// </summary>
        public abstract string UnifyUserName
        {
            get;
        }

        /// <summary>
        /// User's role in organization (if requested).
        /// </summary>
        public abstract Role UserRole
        {
            get;
        }

        /// <summary>
        /// User's theoretical year of entry into education.
        /// This is the year that the student's current year group started in Year 1 (England and Wales) or Primary 1 (Scotland); if
        /// the student skips or repeats a year, then YearOfEntry will change to match their new year group.
        /// This value can be in the future if the student is in a reception / nursery class.
        /// Not available (null) for non-students and for students where the data source does not contain year of entry information.
        /// </summary>
        public abstract int? YearOfEntry
        {
            get;
        }

        /// <summary>
        /// Whether the user is an administrator of RM Unify for their organization.
        /// </summary>
        public abstract bool IsUnifyAdmin
        {
            get;
        }

        /// <summary>
        /// The email address that this user has been assigned in RM Unify (typically used for accessing O365 or Google Apps).
        /// Not available (null) if the user has not yet been given an email address in RM Unify.
        /// </summary>
        public abstract string UnifyEmailAddress
        {
            get;
        }

        /// <summary>
        /// The app user ID of the current user.
        /// This is a string identifying a pre-existing user that the current user should log in to your app as.
        /// It is only provided if your app supports the RM Unify user account matching process and the current user
        /// has been matched to a pre-existing user.
        /// </summary>
        public abstract string AppUserId
        {
            get;
        }

        /// <summary>
        /// The raw attributes from single sign on.
        /// Only needed for advanced scenarios (i.e. if using a non-standard attribute).
        /// Not available (null) if the RmUnifyUser object was not generated from a single sign on request.
        /// </summary>
        public IDictionary<string, string> RawSsoAttributes
        {
            get;
            protected set;
        }

        /// <summary>
        /// The organization that the user belongs to.
        /// </summary>
        public RmUnifyOrganization Organization
        {
            get;
            protected set;
        }
    }
}
