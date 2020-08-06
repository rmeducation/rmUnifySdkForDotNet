using Microsoft.AspNetCore.Builder;
using RM.Unify.Sdk.Client;
using RM.Unify.Sdk.Client.Platform;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RmUnifyMiddleware
    {
        public static IServiceCollection AddRmUnify<T>(this IServiceCollection services)
        {
            services.AddScoped<RmUnifyClientApi>();
            services.AddScoped<IRmUnifyCache, RmUnifyDefaultCache>();
            services.AddScoped(typeof(RmUnifyCallbackApi), typeof(T));

            return services;
        }
    }
}