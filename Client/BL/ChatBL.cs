using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.BL
{
    class ChatBL
    {
        private static ChatBL singelton;
        ServerBL server;
        public event Action<string, string> OnNewMessage;
        public event Action<string, string> OnChatClosed;


        public static ChatBL Instance
        {
            get
            {
                if (singelton == null) singelton = new ChatBL();
                return singelton;
            }
        }

        public ChatBL()
        {
            server = ServerBL.Instance;
            server.chatHubProxy.On("newMessage",
                (string room, string msg) => OnNewMessage?.Invoke(room, msg));
            server.chatHubProxy.On("ChatClosed",
                (string room, string sender) => OnChatClosed?.Invoke(room, sender));
        }

        public async Task<string> JoinRoom(string recipient)
        {
            return await server.chatHubProxy.Invoke<string>("JoinRoom", recipient);
        }

        public async Task SendMessageToRoom(string room, string message)
        {
            await server.chatHubProxy.Invoke("BroadcastPrivateMessage", room, message);
        }

        internal async Task CloseChat(string room)
        {
            await server.chatHubProxy.Invoke("CloseRoom", room);
        }
    }
}
