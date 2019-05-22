using Client.Models;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.BL
{
    class UserBL
    {
        private static UserBL singelton;
        ServerBL server;
        public event Action<string> OnLoggedIn;
        public event Action<string> OnLoggedOut;
        public event Action<string> OnRoomOpen;

        public static UserBL Instance
        {
            get
            {
                if (singelton == null) singelton = new UserBL();
                return singelton;
            }
        }

        public UserBL()
        {
            server = ServerBL.Instance;
            server.userHubProxy.On("LogInNotificated", (string userName) => OnLoggedIn?.Invoke(userName));
            server.userHubProxy.On("LogOutNotificated", (string userName) => OnLoggedOut?.Invoke(userName));
            server.chatHubProxy.On("addToRoom", (string room) => OnRoomOpen?.Invoke(room));
        }

        public async Task<IEnumerable<string>> GetAllUsers(User user)
        {
            return await server.userHubProxy.Invoke<IEnumerable<string>>("GetAllOtherUsers", user.Name);
        }

        public async Task<bool> IsUserOnline(string userName)
        {
            return await server.userHubProxy.Invoke<bool>("IsUserOnline", userName);
        }

        public async Task<string> Register(User user)
        {
            return await server.userHubProxy.Invoke<string>("Register", user.Name, user.Password);

        }

        public async Task<string> Login(User user)
        {
            return await server.userHubProxy.Invoke<string>("Login", user.Name, user.Password);
        }

        public async Task Logout()
        {
            await server.userHubProxy.Invoke("LogOut");
        }




    }
}
