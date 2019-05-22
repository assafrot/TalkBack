using System;
using System.Collections.Generic;
using System.Linq;

namespace TalkBack.DAL
{
    public class UserRepository : IDisposable
    {
        //  TalkBackContext _context = new TalkBackContext();

        public UserDb Add(UserDb user)
        {
            using (var context = new TalkBackContext())
            {
                var newUser = context.Users.Add(user);

                context.SaveChanges();
                return newUser;

            }

            //return newUser;
        }


        public UserDb get(int id)
        {
            using (var context = new TalkBackContext())
            {
                return context.Users.FirstOrDefault(u => u.Id == id);
            }
        }

        public UserDb Login(string username, string password)
        {
            using (var context = new TalkBackContext())
            {
                
                return context.Users.FirstOrDefault(u => u.Name == username && u.Password == password);
            }
        }


        public bool IsUserExist(string username)
        {
            using (var context = new TalkBackContext())
            {
                if (context.Users.FirstOrDefault(u => u.Name == username) != null)
                    return true;
                return false;
            }
        }

        public IEnumerable<string> GetAll()
        {
            using (var context = new TalkBackContext())
            {
                var users = context.Users.Select(u => u.Name);
                if (users != null)
                    return users.ToList();

            }
            return null;
        }

        public void Dispose()
        {
            using (var context = new TalkBackContext())
            {
                context.Dispose();
            }
        }
    }
}
