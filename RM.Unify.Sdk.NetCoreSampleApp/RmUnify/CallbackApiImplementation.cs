using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using RM.Unify.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace RmUnifySample.RmUnify
{
    public class CallbackApi : RmUnifyCallbackApi
    {
        private readonly IHttpContextAccessor _http;

        public CallbackApi(IHttpContextAccessor http, IRmUnifyCache cache)
        {
            _http = http;
            Cache = cache;
        }
        public override string Realm
        {
           get { return "https://rmunifysampleapp.rm.com/realm"; }
        }

        public override void CreateOrUpdateOrganization(RmUnifyOrganization organization, Source source)
        {
            
        }

        /// <summary>
        /// Create or update a user in the application
        /// In this example, we want to store some custom data about the user (display name, last login and deleted date).
        /// Last login and deleted date allow us to soft delete users who haven't logged in for 3 months and then purge
        /// them after 1 year.
        /// We've implement the custom data by just extending the account model and going direct to the database to read
        /// and write the data; we could have equally created a custom membership user
        /// (http://msdn.microsoft.com/en-us/library/ms366730.aspx).
        /// </summary>
        /// <param name="rmUser">RM Unify user</param>
        /// <param name="source">Source of user information (always SingleSignOn at the moment)</param>
        public override void  CreateOrUpdateUser(RmUnifyUser rmUser, Source source)
        {
            
        }

        public override void DoLogin(RmUnifyUser user, DateTime maxSessionEnd, string returnUrl)
        {
            
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.DisplayName),
                    new Claim(ClaimTypes.Role, user.UserRole.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(120),
                    IsPersistent = true
                };

                try
                {
                    _http.HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(claimsIdentity), 
                        authProperties).Wait();
                }
                catch
                {
                    throw;
                }

        }

        public override void DoLogout()
        {
          _http.HttpContext.SignOutAsync().Wait();
        }
    }
}