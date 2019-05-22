using Microsoft.AspNet.SignalR.Client;
using System;

namespace Client.BL
{
    class ServerBL
    {
        private static ServerBL singelton;
        private static Object rootSync = new Object();
        public HubConnection hubConnection { get; set; }
        public IHubProxy userHubProxy { get; set; }
        public IHubProxy chatHubProxy { get; set; }
        public IHubProxy gameHubProxy { get; set; }

        public static ServerBL Instance
        {
            get
            {
                lock (rootSync)
                {
                    if (singelton == null) singelton = new ServerBL();
                }
                return singelton;
            }

        }


        public ServerBL()
        {
            hubConnection = new HubConnection("http://localhost:3000/");
            userHubProxy = hubConnection.CreateHubProxy("UserHub");
            chatHubProxy = hubConnection.CreateHubProxy("ChatHub");
            gameHubProxy = hubConnection.CreateHubProxy("GameHub");
            hubConnection.Start().Wait();
        }

    }
}
