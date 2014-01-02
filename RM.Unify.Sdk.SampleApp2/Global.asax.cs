using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using RM.Unify.Sdk.SampleApp2.Models;

namespace RM.Unify.Sdk.SampleApp2
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            System.Data.Entity.Database.SetInitializer(new SeedData());
        }

        void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var currentContext = HttpContext.Current;
            if (currentContext.Request.IsAuthenticated)
            {
                Account account = Account.Current;
                Role role = (Role) account.Role;
                List<string> roles = new List<string>();
                roles.Add(role.ToString());
                if (role == Role.Admin && School.Current.Id == 1) // global admin school
                {
                    roles.Add("GlobalAdmin");
                }
                var user = new GenericPrincipal(currentContext.User.Identity, roles.ToArray());
                currentContext.User = Thread.CurrentPrincipal = user;
            }
        }
    }
}