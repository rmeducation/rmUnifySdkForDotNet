using System;
using System.Data.Entity;
using RM.Unify.Sdk.SampleApp2.Models;

namespace RM.Unify.Sdk.SampleApp2
{
    public class SeedData : CreateDatabaseIfNotExists<Context>
    {
        protected override void Seed(Context context)
        {
            var school = new School()
            {
                Name = "Global Admin school",
                DfeCode = "0000000",
                PostCode = "N/A",
                Licenced = true
            };
            context.Schools.Add(school);
            context.SaveChanges();

            var account = new Account()
            {
                LoginName = "globaladmin",
                Password = "password",
                DisplayName = "Global Admin",
                SchoolId = school.Id,
                Role = (int)Role.Admin
            };
            context.Accounts.Add(account);
            context.SaveChanges();

            context.Posts.Add(
                new Post()
                {
                    Title = "Welcome to School Blog",
                    Body = "This is the global admin school.  You can use this account to create more schools (but you don't need to if you are using RM Unify as the schools will be automatically created).",
                    Created = DateTime.Now,
                    AccountId = account.Id
                }
            );
        }
    }
}