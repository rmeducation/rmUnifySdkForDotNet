using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RM.Unify.Sdk.SampleApp2.Models;

namespace RM.Unify.Sdk.SampleApp2.Controllers
{
    [Authorize(Roles="GlobalAdmin")]
    public class SchoolController : Controller
    {
        public ActionResult Index()
        {
            if (TempData["message"] != null)
            {
                ViewBag.Message = TempData["message"] as string;
            }
            using (var context = new Context())
            {
                var schools = (from s in context.Schools
                               where s.Id != 1
                               select s).ToList();
                return View(schools);
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(School school)
        {
            try
            {
                using (var context = new Context())
                {
                    context.Schools.Add(school);
                    context.SaveChanges();

                    var account = new Account()
                    {
                        LoginName = "school" + school.Id.ToString() + "admin",
                        Password = "password",
                        SchoolId = school.Id,
                        Role = (int) Role.Admin,
                        DisplayName = school.Name + " admin"
                    };
                    context.Accounts.Add(account);
                    context.SaveChanges();

                    TempData["message"] = "New admin account: " + account.LoginName + " " + account.Password;
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Edit(int id)
        {
            var school = GetSchool(id);
            if (school == null)
            {
                return new HttpNotFoundResult();
            }
            return View(school);
        }

        [HttpPost]
        public ActionResult Edit(int id, School school)
        {
            using (var context = new Context())
            {
                var sch = (from s in context.Schools
                              where s.Id == id
                              select s).FirstOrDefault();
                if (sch == null)
                {
                    return new HttpNotFoundResult();
                }

                sch.Name = school.Name;
                sch.PostCode = school.PostCode;
                sch.DfeCode = school.DfeCode;
                sch.Licenced = school.Licenced;
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            var school = GetSchool(id);
            if (school == null)
            {
                return new HttpNotFoundResult();
            }
            return View(school);
        }

        [HttpPost]
        public ActionResult Delete(int id, School school)
        {
            if (id != 0)
            {
                using (var context = new Context())
                {
                    var accounts = (from a in context.Accounts
                                    where a.SchoolId == id
                                    select a);
                    foreach (var account in accounts)
                    {
                        context.Accounts.Remove(account);
                    }

                    var posts = (from p in context.Posts
                                 where p.Account.SchoolId == id
                                 select p);
                    foreach (var post in posts)
                    {
                        context.Posts.Remove(post);
                    }

                    var sch = (from s in context.Schools
                               where s.Id == id
                               select s).FirstOrDefault();
                    if (sch != null)
                    {
                        context.Schools.Remove(sch);
                    }

                    context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        private School GetSchool(int id)
        {
            using (var context = new Context())
            {
                var school = (from s in context.Schools
                              where s.Id == id
                              select s).FirstOrDefault();
                return school;
            }
        }
    }
}
