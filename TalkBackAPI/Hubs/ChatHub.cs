using Microsoft.AspNet.SignalR;
using System;
using TalkBackAPI.BL;

namespace TalkBackAPI.Hubs
{
    public class ChatHub : Hub
    {
        UserBL userBl = UserBL.Instance;
        ChatBL chatBl = ChatBL.Instance;

        public string JoinRoom(string recepient)
        {
            var recepientConnId = userBl.GetConnectionId(recepient);
            var senderConnId = Context.ConnectionId;
            var room = chatBl.OpenNewRoom(recepientConnId, senderConnId);
            if (room != String.Empty)
                Clients.Client(recepientConnId).addToRoom(room);
            return room;
        }

        public void BroadcastPrivateMessage(string roomId, string message)
        {
            var users = chatBl.GetRoomUsers(roomId);
            string sender, recepient;
            if (users[0] == Context.ConnectionId)
            {
                sender = users[0];
                recepient = users[1];
            }
            else
            {
                sender = users[1];
                recepient = users[0];
            }

            string msg = $"{userBl.GetUserName(sender)}: {message}";
            string mymsg = $"Me: {message}";
            Clients.Client(sender).newMessage(roomId, mymsg);
            Clients.Client(recepient).newMessage(roomId, msg);
        }

        public void CloseRoom(string room)
        {
            string sender = Context.ConnectionId;
            string recipeintId = chatBl.CloseRoom(room, sender);
            string senderName = userBl.GetUserName(sender);
            if (recipeintId != String.Empty)
                Clients.Client(recipeintId).ChatClosed(room, senderName);
        }

    }
}