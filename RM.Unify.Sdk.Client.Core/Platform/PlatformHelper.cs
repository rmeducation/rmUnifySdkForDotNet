//-------------------------------------------------
// <copyright file="PlatformHelper.cs" company="RM Education">                    
//     Copyright © 2013 RM Education Ltd
//     See LICENCE.txt file for more details
// </copyright>
//-------------------------------------------------
using System;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace RM.Unify.Sdk.Client.Platform
{
    internal class PlatformHelper
    {
        private static byte[] _tickImage = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAABEAAAANCAYAAABPeYUaAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAadEVYdFNvZnR3YXJlAFBhaW50Lk5FVCB2My41LjEwMPRyoQAAAH9JREFUOE+dkMkNwCAMBKmGguiHgugozzzzTAeEtWxkrCAMIyHkazjCCVXBKT88V8tbarwi7YDLa/TwtoA6G+lONIxd4JYRW0CME62AyyNUUUguP7lffymQRix5q1sA/gZ0jOX6RCvaFoCZBDmXQLCi5T/MENGxQKDpBocOQvgAl6sV4Md89G8AAAAASUVORK5CYII=");

        internal static string UrlEncode(string s)
        {
            return HttpUtility.UrlEncode(s);
        }

        internal static void RedirectBrowser(IHttpContextAccessor http, string url, bool endResponse)
        {
            var response = http.HttpContext.Response;
            response.Redirect(url, endResponse);
        }

        internal static void AddSessionCookie(IHttpContextAccessor http, string cookieName, string cookieValue)
        {
            http.HttpContext.Response.Cookies.Append(cookieName, cookieValue);
        }

        /// Returns null if cookie does not exist
        internal static string GetCookie(IHttpContextAccessor http, string cookieName)
        {
            return http.HttpContext.Request.Cookies[cookieName];
        }

        internal static void DeleteCookie(IHttpContextAccessor http, string cookieName)
        {
            http.HttpContext.Response.Cookies.Delete(cookieName, new CookieOptions{ Expires =  DateTime.Now.AddDays(-1d)});
        }

        // Returns null if parameter doesn't exist (as get or post)
        internal static string GetParam(IHttpContextAccessor http, string paramName)
        {
            if(http.HttpContext.Request.HasFormContentType)
                return http.HttpContext.Request.Form[paramName];
                
            return null;
        }

        internal static void SendTickResponse(IHttpContextAccessor http, bool endResponse)
        {
            var originBody = http.HttpContext.Response.Body;
            var memStream = new MemoryStream();
            http.HttpContext.Response.Body = memStream;        

            memStream.Position = 0;
            var responseBody = new StreamReader(memStream).ReadToEnd();

            var memoryStreamModified = new MemoryStream();
            var sw = new StreamWriter(memoryStreamModified);
            sw.Write(_tickImage);
            sw.Flush();
            memoryStreamModified.Position = 0;

            memoryStreamModified.CopyToAsync(originBody).Wait();
        }
    }
}
