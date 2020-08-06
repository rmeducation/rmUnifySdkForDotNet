//-------------------------------------------------
// <copyright file="IRmUnifyCache.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;

namespace RM.Unify.Sdk.Client
{
    public interface IRmUnifyCache
    {
        /// <summary>
        /// Add a key to the cache
        /// </summary>
        /// <param name="key">Key name</param>
        /// <param name="value">Value</param>
        /// <param name="expiry">Expiry time</param>
        void Add(string key, string value, DateTime expiry);

        /// <summary>
        /// Get the value associated with a key in the cache
        /// </summary>
        /// <param name="key">Key name</param>
        /// <returns>Value (null if key doesn't exist)</returns>
        string Get(string key);
    }
}
