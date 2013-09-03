using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RM.Unify.Sdk.SampleAppMvc.Filters;

namespace RM.Unify.Sdk.SampleAppMvc.Controllers
{
    [InitializeSimpleMembership]
    public class RmUnifyController : Controller
    {
        //
        // GET: /RmUnify/

        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult Index()
        {
            RM.Unify.Sdk.Client.RmUnifyClientApi client = new RM.Unify.Sdk.Client.RmUnifyClientApi(new RmUnify.CallbackApiImplementation());
            client.ProcessSso();

            // Should never get here
            return this.Content("Should never see this");
        }

    }
}
