using Common.Models;
using System;
using System.Collections.Generic;
using TalkBackAPI.Models;

namespace TalkBackAPI.BL
{
    public class GameBL
    {
        private static GameBL _singelton;
        private static Object rootSync = new Object();
        Dictionary<string, GameRoom> GameRooms = new Dictionary<string, GameRoom>();

        public static GameBL Instance
        {
            get
            {
                lock (rootSync)
                {
                    if (_singelton == null)
                        _singelton = new GameBL();
                }
                return _singelton;
            }
        }

        internal string OpenNewGameRoom(string senderConnId, string recepientConnId)
        {
            string room = Guid.NewGuid().ToString();
            GameRoom gameRoom = new GameRoom(recepientConnId, senderConnId);
            GameRooms.Add(room, gameRoom);
            return room;
        }

        internal CheckerColor GetPlayerColor(string roomid, string connectionId)
        {
            if (GameRooms.ContainsKey(roomid))
            {
                if (GameRooms[roomid].GameManager.BlackUser == connectionId)
                    return CheckerColor.Black;
                else if (GameRooms[roomid].GameManager.WhiteUser == connectionId)
                    return CheckerColor.White;
            }
            return CheckerColor.None;
        }

        internal GameRoom GetGameRoom(string room)
        {
            if (GameRooms.ContainsKey(room))
            {
                return GameRooms[room];
            }

            return null;
        }

        internal List<string> GetPlayersIds(string roomid)
        {
            List<string> users = new List<string>();
            if (GameRooms.ContainsKey(roomid))
            {
                users.Add(GameRooms[roomid].WhiteUser);
                users.Add(GameRooms[roomid].BlackUser);
            }

            return users;

        }

        internal Board GetBoard(string roomId)
        {
            if (GameRooms.ContainsKey(roomId))
                return GameRooms[roomId].GameManager.Board;
            return null;
        }

        internal string CloseRoom(string roomId, string sender)
        {
            string recipient = String.Empty;
            if (GameRooms.ContainsKey(roomId))
            {
                if (GameRooms[roomId].WhiteUser == sender)
                    recipient = GameRooms[roomId].WhiteUser;
                else if (GameRooms[roomId].BlackUser == sender)
                    recipient = GameRooms[roomId].BlackUser;
                GameRooms.Remove(roomId);
            }
            return recipient;
        }
    }
}