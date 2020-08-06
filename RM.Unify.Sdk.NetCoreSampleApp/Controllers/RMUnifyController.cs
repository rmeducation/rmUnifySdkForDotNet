using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RM.Unify.Sdk.Client;

namespace RM.Unify.Sdk.NetCoreSampleApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RMUnifyController: ControllerBase
    {
        private readonly ILogger<RMUnifyController> _logger;
        private readonly RmUnifyClientApi _client;
        private readonly IHttpContextAccessor _http;

        public RMUnifyController(ILogger<RMUnifyController> logger, 
                                RmUnifyClientApi client, 
                                IHttpContextAccessor http)
        {
            _logger = logger;
            _client = client;
            _http = http;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _client.ProcessSso(false);

            return new EmptyResult();
        }

        [HttpPost]
        public IActionResult Post()
        {
            _client.ProcessSso(false);

            return Redirect("/");
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            await _http.HttpContext.SignOutAsync();
            
            if (_client.Logout(false))
            {
                return new EmptyResult();
            }

            return Redirect("/");
        }
    }
}
