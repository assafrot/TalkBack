using Common.Models;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace Client.BL
{
    class GameBL
    {
        private static GameBL singelton;
        ServerBL server;
        public event Action<string, Board> OnStartGame;
        public event Action<string, string> OnGameInvitation;
        public event Action<string, CheckerColor> OnGetPlayerColor;
        public event Action<string> OnDenyInitation;
        public event Action<string, string> OnCurrentTurn;
        public event Action<string> OnGetPlayerName;
        public event Action<string, TurnStatus> OnTurnStatusChange;
        public event Action<string, Board> OnBoardUpdate;
        public event Action<string, int[]> OnDiceResult;
        public event Action<string, string> OnGameOver;
        public event Action<string, string> OnGameClosed;
        public static GameBL Instance
        {
            get
            {
                if (singelton == null) singelton = new GameBL();
                return singelton;
            }
        }

        public GameBL()
        {
            server = ServerBL.Instance;
            server.gameHubProxy.On("StartGame", (string roomId, Board board) => OnStartGame?.Invoke(roomId, board));
            server.gameHubProxy.On("GetPlayerColor", (string roomId, CheckerColor color) => OnGetPlayerColor?.Invoke(roomId, color));
            server.gameHubProxy.On("GetGameInvitation", (string room, string sender) => OnGameInvitation?.Invoke(room, sender));
            server.gameHubProxy.On("DenyGame", (string username) => OnDenyInitation?.Invoke(username));
            server.gameHubProxy.On("CurrentTurn", (string roomId, string username) => OnCurrentTurn?.Invoke(roomId, username));
            server.gameHubProxy.On("GetUserName", (string userName) => OnGetPlayerName?.Invoke(userName));
            server.gameHubProxy.On("UpdateBoard", (string roomId, Board board) => OnBoardUpdate?.Invoke(roomId, board));
            server.gameHubProxy.On("DiceResult", (string room, int[] dice) => OnDiceResult?.Invoke(room, dice));
            server.gameHubProxy.On("ChangeTurnStatus",
                (string roomId, TurnStatus turnStatus) => OnTurnStatusChange?.Invoke(roomId, turnStatus));
            server.gameHubProxy.On("GameOver", (string roomid, string username) => OnGameOver?.Invoke(roomid, username));
            server.gameHubProxy.On("GameClosed", (string room, string sender) => OnGameClosed?.Invoke(room, sender));
        }

        public async Task<string> InviteToGame(string recipient)
        {
            return await server.gameHubProxy.Invoke<string>("GameInvatation", recipient);
        }

        public async Task DenyInvitation(string sender)
        {
            await server.gameHubProxy.Invoke<string>("DenyGameInvatation", sender);
        }

        public async Task AcceptInvitation(string room, string sender)
        {
            await server.gameHubProxy.Invoke("AcceptGameInvatation", room, sender);
        }

        public async Task<Board> GetBoard(string roomId)
        {
            return await server.gameHubProxy.Invoke<Board>("GetBoard", roomId);
        }

        internal async Task<bool> RollDice(string roomId)
        {
            return await server.gameHubProxy.Invoke<bool>("RollDice", roomId);
        }

        internal async Task<bool> PlayMove(string roomId, int source, int target)
        {
            return await server.gameHubProxy.Invoke<bool>("PlayMove", roomId, source, target);
        }

        internal async Task<bool> CanThrowCheckers(string roomId)
        {
            return await server.gameHubProxy.Invoke<bool>("CanTrhowCheckers", roomId);
        }

        internal async Task<bool> ThrowChecker(string roomId, int source)
        {
            return await server.gameHubProxy.Invoke<bool>("ThrowChecker", roomId, source);
        }

        internal async Task CloseGame(string room)
        {
            await server.gameHubProxy.Invoke("CloseRoom", room);
        }


    }
}
