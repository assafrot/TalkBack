using System.Data.Entity;

namespace TalkBack.DAL
{
    class TalkBackInitializer : DropCreateDatabaseAlways<TalkBackContext>
    {
        protected override void Seed(TalkBackContext context)
        {
            UserDb user = new UserDb("admin", "1234");
            context.Users.Add(user);
            UserDb user1 = new UserDb("assaf", "1234");
            context.Users.Add(user1);
            UserDb user2 = new UserDb("gal", "1234");
            context.Users.Add(user2);
            UserDb user3 = new UserDb("daniel", "1234");
            context.Users.Add(user3);
            base.Seed(context);
        }

    }
}
