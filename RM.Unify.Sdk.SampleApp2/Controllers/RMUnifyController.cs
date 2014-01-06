using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using RM.Unify.Sdk.SampleApp2.Models;
using RM.Unify.Sdk.SampleApp2.ViewModels;

// RMUNIFY
namespace RM.Unify.Sdk.SampleApp2.Controllers
{
    public class RmUnifyController : Controller
    {
        [AllowAnonymous]
        [ValidateInput(false)]
        public ActionResult Index()
        {
            RM.Unify.Sdk.Client.RmUnifyClientApi client = new RM.Unify.Sdk.Client.RmUnifyClientApi(new Helpers.RmUnifyWithAccountLinking());
            client.ProcessSso(false);

            return new EmptyResult();
        }

        [Authorize(Roles = "admin")]
        public ActionResult AppEstablishmentKey()
        {
            RmUnifyAppEstablishmentKeyViewModel m = new RmUnifyAppEstablishmentKeyViewModel();
            m.AppEstablishmentKey = School.Current.RmUnifyId;

            if (string.IsNullOrEmpty(m.AppEstablishmentKey))
            {
                using (var context = new Context())
                {
                    School school = (from s in context.Schools
                                     where s.Id == School.Current.Id
                                     select s).SingleOrDefault();
                    school.RmUnifyId = Guid.NewGuid().ToString();
                    context.SaveChanges();
                    m.AppEstablishmentKey = school.RmUnifyId;
                }
                
            }

            return View(m);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult AppEstablishmentKey(RmUnifyAppEstablishmentKeyViewModel m)
        {
            Regex displayNameRegex = new Regex(@"^\s*(Mr.?|Mrs.?|Ms.?|Dr.?|Madame|Monsieur|Prof.?|Professor|Sir)?\s*(\S*)\s*(.+)\s*$");

            // Get a CSV of all users in the school
            using (var context = new Context())
            {
                var accounts = (from a in context.Accounts
                                where a.SchoolId == School.Current.Id && a.DeletedDate == null
                                select a);

                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Disposition", "attachment;filename=appusers.csv");
                Response.Write("FirstName,LastName,UserId\r\n");
                foreach (var account in accounts)
                {
                    Match match = displayNameRegex.Match(account.DisplayName);
                    if (match.Success)
                    {
                        Response.Write(EscapeCsvField(match.Groups[2].Value));
                        Response.Write(",");
                        Response.Write(EscapeCsvField(match.Groups[3].Value));
                        Response.Write(",");
                    }
                    else
                    {
                        Response.Write(",");
                        Response.Write(EscapeCsvField(account.DisplayName));
                        Response.Write(",");
                    }
                    Response.Write(EscapeCsvField(account.LoginName));
                    Response.Write("\r\n");
                }
                Response.End();
            }

            return this.Content("Should never see this");
        }

        private string EscapeCsvField(string field)
        {
            if (field.Contains('"') || field.Contains(','))
            {
                field = field.Replace("\"", "\"\"");
                return "\"" + field + "\"";
            }
            return field;
        }
    }
}
// END RMUNIFY