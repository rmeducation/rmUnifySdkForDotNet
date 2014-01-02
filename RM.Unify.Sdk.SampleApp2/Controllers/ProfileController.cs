using System.Linq;
using System.Web.Mvc;
using RM.Unify.Sdk.SampleApp2.Models;
using RM.Unify.Sdk.SampleApp2.ViewModels;

namespace RM.Unify.Sdk.SampleApp2.Controllers
{
    public class ProfileController : Controller
    {
        public ActionResult Index()
        {
            var model = new AccountViewModel(Account.Current);
            model.CanEditDisplayName = CanEditDisplayName();
            ViewBag.CancelController = "Blog";
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (CanEditDisplayName())
                {
                    using (var context = new Context())
                    {
                        Account account = (from a in context.Accounts
                                           where a.Id == Account.Current.Id
                                           select a).FirstOrDefault();
                        account.DisplayName = model.DisplayName;
                        context.SaveChanges();
                    }
                }

                return RedirectToAction("Index", "Blog");
            }

            return Index();
        }

        public ActionResult Password()
        {
            var model = new PasswordChangeViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Password(PasswordChangeViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Account.Current.Password != model.OldPassword)
                {
                    ModelState.AddModelError("OldPassword", "Incorrect old password");
                    return View(model);
                }
                using (var context = new Context())
                {
                    Account account = (from a in context.Accounts
                                       where a.Id == Account.Current.Id
                                       select a).FirstOrDefault();
                    account.Password = model.NewPassword;
                    context.SaveChanges();
                }

                return RedirectToAction("Index", "Blog");
            }

            return View(model);
        }

        private bool CanEditDisplayName()
        {
            // RMUNIFY
            if (School.Current.IsRmUnifySchool)
            {
                return false;
            }
            // END RMUNIFY
            return Account.Current.RoleEnum == Role.Admin || Account.Current.RoleEnum == Role.Staff;
        }
    }
}
