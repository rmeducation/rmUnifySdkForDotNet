using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using RM.Unify.Sdk.SampleApp2.Models;
using RM.Unify.Sdk.SampleApp2.ViewModels;

namespace RM.Unify.Sdk.SampleApp2.Controllers
{
    [Authorize(Roles="admin")]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!User.Identity.IsAuthenticated)
            {
                using (var context = new Context())
                {
                    Account account = (from a in context.Accounts.Include("School")
                                       where a.LoginName == model.LoginName
                                         && a.DeletedDate == null
                                       select a).SingleOrDefault();

                    if (account == null || account.Password != model.Password)
                    {
                        ModelState.AddModelError("LoginName", "Invalid username or password");
                    }

                    // RMUNIFY
                    if (ModelState.IsValid && account.School.IsRmUnifySchool)
                    {
                        ModelState.AddModelError("LoginName", "RM Unify sign in required");
                        ViewBag.ValidationSummary = "Your school now signs in with RM Unify; please click on the \"Sign in with RM Unify\" button";
                    }
                    // END RMUNIFY

                    if (ModelState.IsValid && account.School.Licenced == false)
                    {
                        ModelState.AddModelError("LoginName", "");
                        ViewBag.ValidationSummary = "Your school licence for School Blog has expired; please contact School Blog support to renew";
                    }

                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }

                    account.LastLogin = DateTime.Now;
                    context.SaveChanges();

                    FormsAuthentication.SetAuthCookie(account.LoginName, false);
                }
            }

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Blog");
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            // RMUNIFY
            RM.Unify.Sdk.Client.RmUnifyClientApi client = new RM.Unify.Sdk.Client.RmUnifyClientApi(new Helpers.RmUnifyWithAccountLinking());
            if (client.Logout(false))
            {
                return new EmptyResult();
            }
            // END RMUNIFY
            return RedirectToAction("Index", "Blog");
        }

        public ActionResult Index()
        {
            using (var context = new Context())
            {
                var accountViewModels = (from a in context.Accounts
                                         where a.SchoolId == School.Current.Id && a.DeletedDate == null
                                         orderby a.DisplayName
                                         select a).ToList().Select(model => new AccountViewModel(model));
                return View(accountViewModels);
            }
        }

        public ActionResult Create()
        {
            AccountViewModel model = new AccountViewModel(new Account());
            model.CanEditDisplayName = model.CanEditLoginName = model.CanEditRole = model.CanEditPassword = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var context = new Context())
                {
                    Account oldAccount = (from a in context.Accounts
                                          where a.LoginName == model.LoginName
                                          select a).SingleOrDefault();
                    if (oldAccount != null)
                    {
                        ModelState.AddModelError("LoginName", "Login name already in use");
                    }
                    else
                    {
                        Account account = new Account()
                        {
                            LoginName = model.LoginName,
                            Password = model.Password,
                            DisplayName = model.DisplayName,
                            RoleEnum = model.RoleEnum,
                            SchoolId = School.Current.Id
                        };
                        context.Accounts.Add(account);
                        context.SaveChanges();

                        return RedirectToAction("Index");
                    }
                }
            }

            model.CanEditDisplayName = model.CanEditLoginName = model.CanEditRole = model.CanEditPassword = true;
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            using (var context = new Context())
            {
                Account account = (from a in context.Accounts
                                   where a.Id == id && a.SchoolId == School.Current.Id
                                     && a.DeletedDate == null
                                   select a).SingleOrDefault();

                // account not found or not in school
                if (account == null)
                {
                    return this.HttpNotFound();
                }

                var model = new AccountViewModel(account);

                model.CanEditDisplayName = true;
                model.CanEditLoginName = model.CanEditRole = (id != Account.Current.Id);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, AccountViewModel model)
        {
            // RMUNIFY
            if (School.Current.IsRmUnifySchool)
            {
                throw new Exception("Can't edit users in RM Unify schools");
            }
            // END RMUNIFY

            model.CanEditDisplayName = true;
            model.CanEditLoginName = model.CanEditRole = (id != Account.Current.Id);

            if (ModelState.IsValid)
            {
                using (var context = new Context())
                {
                    Account oldAccount = (from a in context.Accounts
                                          where a.LoginName == model.LoginName
                                          select a).SingleOrDefault();
                    if (oldAccount != null)
                    {
                        ModelState.AddModelError("LoginName", "Login name already in use");
                    }
                    else
                    {
                        Account account = (from a in context.Accounts
                                           where a.Id == id && a.SchoolId == School.Current.Id
                                             && a.DeletedDate == null
                                           select a).SingleOrDefault();

                        // account not found or not in school
                        if (account == null)
                        {
                            return this.HttpNotFound();
                        }

                        if (model.CanEditLoginName)
                        {
                            account.LoginName = model.LoginName;
                        }
                        if (model.CanEditDisplayName)
                        {
                            account.DisplayName = model.DisplayName;
                        }
                        if (model.CanEditRole)
                        {
                            account.RoleEnum = model.RoleEnum;
                        }
                        context.SaveChanges();
                    }
                }
                
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            using (var context = new Context())
            {
                Account account = (from a in context.Accounts
                                   where a.Id == id && a.SchoolId == School.Current.Id
                                     && a.DeletedDate == null
                                   select a).SingleOrDefault();

                // account not found or not in school
                if (account == null)
                {
                    return this.HttpNotFound();
                }

                return View(new AccountViewModel(account));
            }
        }

        [HttpPost]
        public ActionResult Delete(int id, string btnSubmit)
        {
            // RMUNIFY
            if (School.Current.IsRmUnifySchool)
            {
                throw new Exception("Can't delete users in RM Unify schools");
            }
            // END RMUNIFY

            // can't delete myself
            if (id == Account.Current.Id)
            {
                return new HttpUnauthorizedResult();
            }

            using (var context = new Context())
            {
                Account account = (from a in context.Accounts
                                   where a.Id == id && a.SchoolId == School.Current.Id
                                     && a.DeletedDate == null
                                   select a).SingleOrDefault();

                // account not found or not in school
                if (account == null)
                {
                    return this.HttpNotFound();
                }

                account.DeletedDate = DateTime.Now;
                context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
