//-------------------------------------------------
// <copyright file="RmUnifyOrganization.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;

namespace RM.Unify.Sdk.Client
{
    public abstract class RmUnifyOrganization
    {
        /// <summary>
        /// RM Unify organization identifier (if requested).
        /// The RM Unify organization identifier is guaranteed not to change while the organization still exists.
        /// </summary>
        public abstract string Id
        {
            get;
        }

        /// <summary>
        /// The name of the organization (if requested).
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// A code identifying the organization, where possible given by an external body.
        /// For schools in England and Wales, this is the 7-digit DfE number (LA code followed by school code).
        /// For schools in Scotland, this is the 7-digit SEED number.
        /// </summary>
        public abstract string Code
        {
            get;
        }

        /// <summary>
        /// A string identifying the licence that the organization have purchased for your app through RM Unify;
        /// null if no organization licence recorded (SSO Connector or licence has been procured as part of a
        /// larger deal).
        /// </summary>
        public abstract string LicenceDescription
        {
            get;
        }

        /// <summary>
        /// Whether the organization has an SSO Connector (rather than purchasing a licence through RM Unify).
        /// </summary>
        public abstract bool IsSsoConnector
        {
            get;
        }

        /// <summary>
        /// Whether the organization is currently using the app as part of a free trial.
        /// </summary>
        public abstract bool IsFreeTrial
        {
            get;
        }

        /// <summary>
        /// The app establishment key of the current organization.
        /// This is an string you issue to the organization to identify a pre-existing establishment within your
        /// app.
        /// null if not an SSO Connector and not using the RM Unify user account matching process.
        /// </summary>
        public abstract string AppEstablishmentKey
        {
            get;
        }
    }
}
