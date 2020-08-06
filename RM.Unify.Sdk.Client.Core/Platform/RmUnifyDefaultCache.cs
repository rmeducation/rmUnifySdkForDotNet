//-------------------------------------------------
// <copyright file="RmUnifyDefaultCache.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;
using System.Web;
using Microsoft.Extensions.Caching.Memory;

namespace RM.Unify.Sdk.Client.Platform
{
    /// <summary>
    /// Simple instance of a TokenCache, using HttpContext
    /// Override to support a cache across mulitple servers
    /// </summary>
    internal class RmUnifyDefaultCache : IRmUnifyCache
    {
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Create a TokenCache
        /// </summary>
        public RmUnifyDefaultCache(IMemoryCache cache)
        {
            CachePrefix = "_rmunify_tokencache";
            _cache = cache;
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
            _cache.Set(CachePrefix + key, value, new MemoryCacheEntryOptions {
                Priority = CacheItemPriority.High
            });
        }

        public virtual string Get(string key)
        {
            return _cache.Get(CachePrefix + key) as string;
        }
    }
}
