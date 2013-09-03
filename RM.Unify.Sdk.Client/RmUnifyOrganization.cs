//-------------------------------------------------
// <copyright file="RmUnifyOrganization.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;

namespace RM.Unify.Sdk.Client
{
    public class RmUnifyOrganization
    {
        /// <summary>
        /// RM Unify organization identifier (if requested)
        /// The RM Unify organization identifier is guaranteed not to change while the organization still exists
        /// </summary>
        public string Id
        {
            get;
            protected set;
        }

        /// <summary>
        /// The name of the organization (if requested)
        /// </summary>
        public string Name
        {
            get;
            protected set;
        }

        /// <summary>
        /// A code identifying the organization, where possible given by an external body
        /// For schools in England and Wales, this is the 7-digit DfE number (LA code followed by school code)
        /// For schools in Scotland, this is the 7-digit SEED number
        /// </summary>
        public string Code
        {
            get;
            protected set;
        }
    }
}
