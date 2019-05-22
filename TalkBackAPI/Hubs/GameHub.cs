using Common.Models;
using Microsoft.AspNet.SignalR;
using System;
using TalkBackAPI.BL;

namespace TalkBackAPI.Hubs
{
    public class GameHub : Hub
    {
        UserBL userBl = UserBL.Instance;
        GameBL gameBl = GameBL.Instance;

        public string GameInvatation(string recepient)
        {
            var recepientConnId = userBl.GetConnectionId(recepient);
            var gameRoom = gameBl.OpenNewGameRoom(Context.ConnectionId, recepientConnId);
            var sender = userBl.GetUserName(Context.ConnectionId);
            Clients.Client(recepientConnId).GetGameInvitation(gameRoom, sender);
            return gameRoom;
        }

        public void DenyGameInvatation(string sender)
        {
            var senderConnId = userBl.GetConnectionId(sender);
            var username = userBl.GetUserName(Context.ConnectionId);
            Clients.Client(senderConnId).DenyGame(username);
        }

        public void AcceptGameInvatation(string roomId, string sender)
        {
            var gameroom = gameBl.GetGameRoom(roomId);
            Groups.Add(gameroom.WhiteUser, roomId);
            Groups.Add(gameroom.BlackUser, roomId);
            Clients.Client(gameroom.BlackUser).GetPlayerColor(roomId, CheckerColor.Black);
            Clients.Client(gameroom.WhiteUser).GetPlayerColor(roomId, CheckerColor.White);
            Clients.Client(Context.ConnectionId).GetUserName(userBl.GetUserName(Context.ConnectionId));
            Clients.Client(userBl.GetConnectionId(sender)).GetUserName(sender);
            Clients.Group(roomId).StartGame(roomId, gameroom.GameManager.Board);
            var currentPlayer = userBl.GetUserName(gameroom.GameManager.Turn);
            Clients.Group(roomId).CurrentTurn(roomId, currentPlayer);
            Clients.Group(roomId).ChangeTurnStatus(roomId, gameroom.GameManager.TurnStatus);
        }

        public CheckerColor GetPlayerColor(string roomid, string connectionId)
        {
            return gameBl.GetPlayerColor(roomid, connectionId);
        }

        public Board GetBoard(string roomId)
        {
            return gameBl.GetBoard(roomId);
        }

        public bool RollDice(string roomId)
        {
            var gameroom = gameBl.GetGameRoom(roomId);
            if (gameroom.GameManager.Turn != Context.ConnectionId)
                return false;
            var res = gameroom.GameManager.Roll();
            if (gameroom.GameManager.TurnStatus == TurnStatus.EndTurn)
            {
                Clients.Group(roomId).ChangeTurnStatus(roomId, gameroom.GameManager.TurnStatus);
                gameroom.GameManager.SwitchTurns();
                var currentPlayer = userBl.GetUserName(gameroom.GameManager.Turn);
                Clients.Group(roomId).CurrentTurn(roomId, currentPlayer);
            }
            Clients.Group(roomId).ChangeTurnStatus(roomId, gameroom.GameManager.TurnStatus);
            Clients.Group(roomId).DiceResult(roomId, res);
            return true; ;
        }

        public bool PlayMove(string roomId, int source, int target)
        {
            var gameroom = gameBl.GetGameRoom(roomId);
            if (gameroom.GameManager.Turn != Context.ConnectionId)
                return false;
            if (gameroom.GameManager.PlayMove(source, target))
            {
                Clients.Group(roomId).UpdateBoard(roomId, gameroom.GameManager.Board);
                if (gameroom.GameManager.TurnStatus == TurnStatus.GameOver)
                {
                    Clients.Group(roomId).ChangeTurnStatus(roomId, gameroom.GameManager.TurnStatus);
                }
                if (gameroom.GameManager.TurnStatus == TurnStatus.EndTurn)
                {
                    Clients.Group(roomId).ChangeTurnStatus(roomId, gameroom.GameManager.TurnStatus);
                    gameroom.GameManager.SwitchTurns();
                    var currentPlayer = userBl.GetUserName(gameroom.GameManager.Turn);
                    Clients.Group(roomId).CurrentTurn(roomId, currentPlayer);
                    Clients.Group(roomId).ChangeTurnStatus(roomId, gameroom.GameManager.TurnStatus);
                }
                return true;
            }
            return false;
        }

        public bool ThrowChecker(string roomId, int source)
        {
            var gameroom = gameBl.GetGameRoom(roomId);
            if (gameroom.GameManager.ThrowChecker(source))
            {
                Clients.Group(roomId).UpdateBoard(roomId, gameroom.GameManager.Board);
                if (gameroom.GameManager.IsWinner())
                {
                    Clients.Group(roomId).ChangeTurnStatus(roomId, gameroom.GameManager.TurnStatus);
                    Clients.Group(roomId).GameOver(roomId, userBl.GetUserName(Context.ConnectionId));
                }
                else if (gameroom.GameManager.TurnStatus == TurnStatus.EndTurn)
                {
                    Clients.Group(roomId).ChangeTurnStatus(roomId, gameroom.GameManager.TurnStatus);
                    gameroom.GameManager.SwitchTurns();
                    var currentPlayer = userBl.GetUserName(gameroom.GameManager.Turn);
                    Clients.Group(roomId).CurrentTurn(roomId, currentPlayer);
                    Clients.Group(roomId).ChangeTurnStatus(roomId, gameroom.GameManager.TurnStatus);
                }
                return true;
            }
            return false;
        }

        public bool CanTrhowCheckers(string roomId)
        {
            var gameroom = gameBl.GetGameRoom(roomId);
            var color = gameBl.GetPlayerColor(roomId, Context.ConnectionId);
            return gameroom.GameManager.CanThrowCheckers(color);
        }


        public void CloseRoom(string room)
        {
            string sender = Context.ConnectionId;
            string recipientId = gameBl.CloseRoom(room, sender);
            string senderName = userBl.GetUserName(sender);
            if (recipientId != String.Empty)
                Clients.Client(recipientId).GameClosed(room, senderName);
        }




    }
}