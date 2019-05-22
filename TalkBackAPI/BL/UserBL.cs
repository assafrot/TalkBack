using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalkBack.DAL;

namespace TalkBackAPI.BL
{
    public class UserBL
    {
        private static UserBL _singelton;
        private static Object rootSync = new Object();
        private UserRepository _repo = new UserRepository();

        Dictionary<string, UserDb> OnlineUsersById = new Dictionary<string, UserDb>();
        Dictionary<string, string> OnlineUsersByName = new Dictionary<string, string>();

        public static UserBL Instance
        {
            get
            {
                lock (rootSync)
                {
                    if (_singelton == null)
                        _singelton = new UserBL();
                }
                return _singelton;
            }
        }

        public string Register(string username, string password, string connectionID, Action<string> userNotificationMethod)
        {
            UserDb user = new UserDb(username, password);//should be server model
            if (_repo.IsUserExist(user.Name))
                return "User exists";
            else if (user.Password.Count() < 4)
                return "Password length must be 4 or more";
            else if (OnlineUsersById.ContainsKey(connectionID))
                return "Please sign out first";
            else
            {
                OnlineUsersById.Add(connectionID, user);
                OnlineUsersByName.Add(user.Name, connectionID);
                UserDb userDb = new UserDb(username, password);
                _repo.Add(userDb);
                Task.Run(() =>
                {
                    userNotificationMethod(user.Name);
                });

                return string.Empty;
            }
        }

        internal bool IsUserOnline(string username)
        {
            return OnlineUsersByName.ContainsKey(username);
        }

        internal IEnumerable<string> GetAllOtherUsers(string userName)
        {
            var list = _repo.GetAll();
            return list.Where(x => x != userName).ToList();
        }


        public IEnumerable<string> GetAllUsers()
        {
            return _repo.GetAll();
        }


        public string Login(string username, string password, string connectionID, Action<string> userNotificationMethod)
        {
            UserDb user = new UserDb(username, password);
            UserDb loggedUser = _repo.Login(user.Name, user.Password);
            if (loggedUser == null)
                return "incorrect user or passowrd";
            if (OnlineUsersById.ContainsKey(connectionID))
                return "Please sign out first";
            if (OnlineUsersByName.ContainsKey(username))
                return "You are already signed in in diffrent device";
            else
            {
                OnlineUsersById.Add(connectionID, user);
                OnlineUsersByName.Add(user.Name, connectionID);
                Task.Run(() => { userNotificationMethod(loggedUser.Name); });
                return string.Empty;
            }
        }

        public void Logout(string connectionID, Action<string> userNotificationMethod)
        {
            string username;
            if (OnlineUsersById.ContainsKey(connectionID))
            {
                username = OnlineUsersById[connectionID].Name;
                OnlineUsersById.Remove(connectionID);
                if (OnlineUsersByName.ContainsKey(username))
                    OnlineUsersByName.Remove(username);
                Task.Run(() => { userNotificationMethod(username); });
            }
        }

        public string GetConnectionId(string username)
        {
            string res = "";
            if (OnlineUsersByName.ContainsKey(username))
                res = OnlineUsersByName[username];
            return res;
        }

        public string GetUserName(string connectionID)
        {
            string res = "";
            if (OnlineUsersById.ContainsKey((connectionID)))
                res = OnlineUsersById[connectionID].Name;
            return res;
        }


    }
}