//-------------------------------------------------
// <copyright file="RmUnifyDefaultCache.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;
using System.Web;
using System.Web.Caching;

namespace RM.Unify.Sdk.Client.Platform
{
    /// <summary>
    /// Simple instance of a TokenCache, using HttpContext
    /// Override to support a cache across mulitple servers
    /// </summary>
    internal class RmUnifyDefaultCache : IRmUnifyCache
    {
        /// <summary>
        /// Create a TokenCache
        /// </summary>
        public RmUnifyDefaultCache()
        {
            CachePrefix = "_rmunify_tokencache";
        }

        /// <summary>
        /// Prefix to use for cache keys
        /// Default is _rmunify_tokencache
        /// </summary>
        public string CachePrefix
        {
            get;
            set;
        }

        public virtual void Add(string key, string value, DateTime expiry)
        {
            HttpContext.Current.Cache.Add(CachePrefix + key, value, null, expiry, Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }

        public virtual string Get(string key)
        {
            return (HttpContext.Current.Cache.Get(CachePrefix + key) as string);
        }
    }
}
