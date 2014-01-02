using System;
using System.Linq;
using System.Web.Mvc;
using RM.Unify.Sdk.SampleApp2.Models;

namespace RM.Unify.Sdk.SampleApp2.Controllers
{
    [Authorize]
    public class BlogController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new Context())
            {
                ViewBag.SchoolName = Account.Current.School.Name;
                var posts = (from p in context.Posts.Include("Account")
                             where p.Account.SchoolId == School.Current.Id
                             orderby p.Created descending
                             select p).Take(10).ToList();

                return View(posts);
            }
        }

        [Authorize(Roles="admin,staff")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin,staff")]
        [HttpPost]
        public ActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                using (var context = new Context())
                {
                    post.Created = DateTime.Now;
                    post.AccountId = Account.Current.Id;
                    context.Posts.Add(post);
                    context.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(post);
        }
    }
}
