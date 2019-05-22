using System.Data.Entity;


namespace TalkBack.DAL
{
    public class TalkBackContext : DbContext
    {
        public TalkBackContext() : base("TalkBackConnectionString")
        {
            Database.SetInitializer(new TalkBackInitializer());
        }

        public DbSet<UserDb> Users { get; set; }
    }
}
