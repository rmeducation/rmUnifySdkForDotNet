using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RM.Unify.Sdk.NetCoreSampleApp.Models;

namespace RM.Unify.Sdk.NetCoreSampleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _http;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor http)
        {
            _logger = logger;
            _http = http;
        }

        public IActionResult Index()
        {
             if(_http.HttpContext.User.Identity.IsAuthenticated)
            {
                ViewBag.User = _http.HttpContext.User.Identity.Name;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
