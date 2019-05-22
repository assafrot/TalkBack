using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace TalkBackAPI.BL
{
    public class ChatBL
    {
        private static ChatBL _singelton;
        private static Object rootSync = new Object();
        Dictionary<string, List<string>> ChatRooms = new Dictionary<string, List<string>>();

        public static ChatBL Instance
        {
            get
            {
                lock (rootSync)
                {
                    if (_singelton == null)
                        _singelton = new ChatBL();
                }
                return _singelton;
            }
        }

        internal string OpenNewRoom(string recepientConnId, string senderConnId)
        {
            var users = new List<string>();
            users.Add(senderConnId);
            users.Add(recepientConnId);
            bool valid = true;
            foreach (var chat in ChatRooms)
            {
                if (chat.Value.All(users.Contains))
                    valid = false;
            }

            if (valid)
            {
                string room = Guid.NewGuid().ToString();
                ChatRooms.Add(room, users);
                return room;
            }
            return String.Empty;
        }

        internal List<string> GetRoomUsers(string roomId)
        {
            if (ChatRooms.ContainsKey(roomId))
                return ChatRooms[roomId];
            else return null;
        }

        internal string CloseRoom(string room,string sender)
        {
            string recipient=String.Empty;
            if (ChatRooms.ContainsKey(room))
            {
                List<string> users = ChatRooms[room];
                if (users[0] == sender)
                    recipient = users[1];
                else if (users[1] == sender)
                    recipient = users[0];
                ChatRooms.Remove(room);
            }
            return recipient;
        }
    }
}