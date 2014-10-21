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
        /// This identifier will typically persist for users in the Scottish "Glow" implementation of RM Unify when they
        /// move between schools.
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
        /// The MIS ID of the current user.
        /// This is an internal identifier supplied by the MIS to RM Unify.  If your app connects to the school MIS, this
        /// can be used to link an RM Unify account to an MIS record.
        /// Identifiers will overlap for students and staff within the same organisation and for users in different
        /// organisations; when matching to MIS data, you will need to also use the organization code and the user role
        /// to match to the correct MIS record.
        /// This attribute is only available if the school has linked RM Unify to their MIS, and if the individual user
        /// has been matched to an MIS record in RM Unify.  It is available for all users with MIS records in state-run
        /// schools using the Scottish "Glow" implementation of RM Unify.
        /// See http://dev.rmunify.com/reference/understanding-rm-unify/mis-data-from-rm-unify.aspx for more information.
        /// </summary>
        public abstract string MisId
        {
            get;
        }

        /// <summary>
        /// The registration group of the current user.
        /// This is the name the school gives to the registration group (or class) that the student is in.
        /// Each student can only be in one registration group.
        /// This attribute is only available if the school has linked RM Unify to their MIS, and if the individual user
        /// has been matched to an MIS record in RM Unify.  It is available for all students in state-run schools using
        /// the Scottish "Glow" implementation of RM Unify.
        /// </summary>
        public abstract string RegGroup
        {
            get;
        }

        /// <summary>
        /// The unique pupil number for the current user.
        /// UPN only applies to students in England and Wales.  This is a government-issued identifier which should
        /// not change even when the student moves school.
        /// This attribute is only available if the school has linked RM Unify to their MIS, and if the individual user
        /// has been matched to an MIS record in RM Unify.
        /// </summary>
        public abstract string UPN
        {
            get;
        }

        /// <summary>
        /// The Scottish candidate number for the current user.
        /// SCN only applies to students in Scotland.  This is a government-issued identifier which should not change
        /// even when the student moves school.
        /// SCN is available for all students in state-run schools using the Scottish "Glow" implementation of RM Unify.
        /// In the case of independent Scottish schools or Scottish schools using RM Unify outside "Glow", it is only
        /// available if the school has linked RM Unify to their MIS, and if the individual user has been matched to an
        /// MIS record in RM Unify.
        /// </summary>
        public abstract string SCN
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
