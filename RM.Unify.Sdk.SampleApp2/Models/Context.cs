using System.Data.Entity;

namespace RM.Unify.Sdk.SampleApp2.Models
{
    public class Context : DbContext
    {
        public Context()
            : base("DefaultConnection")
        {
        }

        public DbSet<School> Schools { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Post> Posts { get; set; }
    }
}