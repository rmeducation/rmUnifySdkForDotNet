using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace RM.Unify.Sdk.SampleApp2
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/css/bootstrap.css",
                        "~/Content/css/schoolblog.css"));
        }
    }
}
