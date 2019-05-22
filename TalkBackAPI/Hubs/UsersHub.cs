using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using TalkBackAPI.BL;

namespace TalkBackAPI.Hubs
{
    [HubName("UserHub")]
    public class UsersHub : Hub
    {
        UserBL userBl = UserBL.Instance;

        public string Login(string username, string passpowrd)
        {
            Clients.CallerState.UserName = username;
            return userBl.Login(username, passpowrd, Context.ConnectionId, UserConnected);
        }

        public string Register(string username, string password)
        {

            Clients.CallerState.UserName = username;
            return userBl.Register(username, password, Context.ConnectionId, UserConnected);
        }

        public void LogOut()
        {
            userBl.Logout(Context.ConnectionId, UserDisconnected);
        }

        public void UserConnected(string userName)
        {
            this.Clients.Others.LogInNotificated(userName);
        }

        public void UserDisconnected(string userName)
        {
            this.Clients.Others.LogOutNotificated(userName);

        }

        public bool IsUserOnline(string username)
        {
            return userBl.IsUserOnline(username);
        }


        public IEnumerable<string> GetAllOtherUsers(string userName)
        {
            return userBl.GetAllOtherUsers(userName);
        }


    }
}