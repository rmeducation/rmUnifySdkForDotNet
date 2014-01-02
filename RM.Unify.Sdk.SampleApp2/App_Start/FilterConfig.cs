using System.Web;
using System.Web.Mvc;

namespace RM.Unify.Sdk.SampleApp2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}