//-------------------------------------------------
// <copyright file="RmUnifyClientApi.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;

namespace RM.Unify.Sdk.Client
{
    public class RmUnifyClientApi
    {
        #region Private fields
        private SsoImpl.SsoHelper _ssoHelper;
        #endregion

        /// <summary>
        /// Constructor
        /// Pass in an instantiation of the callback API, which contains methods and properties required to integrate with your app
        /// </summary>
        /// <param name="callbackApi">Your implementation of the callback API</param>
        public RmUnifyClientApi(RmUnifyCallbackApi callbackApi)
        {
            _ssoHelper = new SsoImpl.SsoHelper(callbackApi);
        }

        /// <summary>
        /// Initiate login to your application
        /// You can either initiate login by calling this method or by redirecting to your RM Unify endpoint (see ProcessSso()).
        /// If this method is called with refreshOnly=true, the method will return immediately without doing anything unless
        /// the current user was logged in via RM Unify; this is useful if the session in your app expires and you want to 
        /// automatically sign back in (provided the user is still logged in to RM Unify).
        /// </summary>
        /// <param name="returnUrl">URL to redirect to on successful login (null for default URL)</param>
        /// <param name="refreshOnly">If true, login is only initiated if the current user was previously logged in via RM Unify</param>
        /// <param name="endResponse">If true, the method should call Response.End() if redirecting to RM Unify for login.
        /// True by default to maintain compatibility with earlier releases, but recommended value is false (because ending
        /// the response can cause issues with setting cookies).</param>
        public void Login(string returnUrl, bool refreshOnly, bool endResponse = true)
        {
            _ssoHelper.Login(returnUrl, refreshOnly, endResponse);
        }

        /// <summary>
        /// Initiate logout of RM Unify
        /// This method should always be called immediately after logging out of your app (i.e. delete your local session
        /// cookies and then call this method).
        /// If the current user logged in via RM Unify, this method will redirect to the RM Unify single logout page.
        /// If the current user did not log in via RM Unify, this method will do nothing and return immediately.
        /// </summary>
        /// <param name="endResponse">If true, the method should call Response.End() if redirecting to RM Unify for logout.
        /// True by default to maintain compatibility with earlier releases, but recommended value is false (because ending
        /// the response can cause issues with clearing cookies).</param>
        /// <returns>True if the user is an RM Unify user (and the browser has therefore been redirected to the RM Unify logout page)</returns>
        public bool Logout(bool endResponse = true)
        {
            return _ssoHelper.Logout(endResponse);
        }

        /// <summary>
        /// Process single sign on messages
        /// Your application should implement an RM Unify endpoint URL (e.g. https://myapp.com/rmunify) which simply calls
        /// this method.  This will be your RM Unify landing page, and you can initiate login by redirecting to this page
        /// (optionally add ?returnUrl=... when redirecting to this page to return to a specific URL after successful login).
        /// </summary>
        /// <param name="endResponse">If true, the method should call Response.End() after processing redirecting or returning
        /// a sign out tick.  True by default to maintain compatibility with earlier releases, but recommended value is false
        /// (because ending the response can cause issues with setting or clearing cookies).</param>
        public void ProcessSso(bool endResponse = true)
        {
            _ssoHelper.ProcessSso(endResponse);
        }

        /// <summary>
        /// Determine whether the current user logged in using RM Unify
        /// </summary>
        /// <returns>True if the user logged in using RM Unify, false otherwise</returns>
        public static bool IsRmUnifyUser()
        {
            return SsoImpl.SsoHelper.IsRmUnifyUser();
        }
    }
}
