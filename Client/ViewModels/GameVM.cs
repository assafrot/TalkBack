using Client.BL;
using Client.Commands;
using Client.Interfaces;
using Common.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Client.ViewModels
{
    class GameVM : INotifyPropertyChanged, IClosing
    {
        public event PropertyChangedEventHandler PropertyChanged;
        GameBL gameBl;
        private string _currentStatus;
        public string CurrentStatus
        {
            get { return _currentStatus; }
            set
            {
                _currentStatus = value;
                OnPropertyChanged(nameof(CurrentStatus));
            }
        }
        private string _room { get; set; }
        private int[] _dice;
        public int[] Dice
        {
            get { return _dice; }
            set
            {
                _dice = value;
                OnPropertyChanged(nameof(Dice));
            }
        }
        private string _playerUserName;
        public string UserName
        {
            get { return _playerUserName; }
            set
            {
                _playerUserName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        public string Sender { get; set; }
        private TurnStatus _turnStatus;
        public TurnStatus TurnStatus
        {
            get { return _turnStatus; }
            set
            {
                _turnStatus = value;
                OnPropertyChanged(nameof(TurnStatus));
                _guiDispatcher.Invoke(() => { RollDiceCommand.NotifyCanExecuteChanged(); });

            }
        }
        private bool _currentTurn;
        public bool CurrentTurn
        {
            get { return _currentTurn; }
            set
            {
                _currentTurn = value;
                OnPropertyChanged(nameof(CurrentTurn));
            }
        }
        private int _checkerSelection;
        private string _playerColor;
        public string PlayerColor
        {
            get { return _playerColor; }
            set
            {
                _playerColor = value;
                OnPropertyChanged(nameof(PlayerColor));
            }
        }
        private Board _board;
        public Board Board
        {
            get { return _board; }
            set
            {
                _board = value;
                OnPropertyChanged(nameof(Board));
            }
        }
        public CommandExecuter RollDiceCommand { get; set; }
        public ChooseCommand ChooseCheckerCommand { get; set; }
        private Dispatcher _guiDispatcher;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public GameVM(string roomID)
        {
            _room = roomID;
            gameBl = GameBL.Instance;
            _guiDispatcher = Dispatcher.CurrentDispatcher;
            Dice = new int[2];
            _checkerSelection = -1;
            RollDiceCommand = new CommandExecuter(RollDice, () => { return TurnStatus == TurnStatus.RollDice && CurrentTurn; });
            ChooseCheckerCommand = new ChooseCommand((place) => ChooseChecker(place));

            gameBl.OnStartGame += OnStartGame;
            gameBl.OnDenyInitation += OnDenyInvitation;
            gameBl.OnGetPlayerColor += OnGetColor;
            gameBl.OnCurrentTurn += OnCurrentTurn;
            gameBl.OnGetPlayerName += OnGetPlayerName;
            gameBl.OnTurnStatusChange += OnChangeStatus;
            gameBl.OnBoardUpdate += UpdateBoard;
            gameBl.OnDiceResult += OnDiceResult;
            gameBl.OnGameOver += GameOver;
            gameBl.OnGameClosed += OnGameClosed;
        }

        public void OnStartGame(string roomId, Board board)
        {
            if (_room == roomId)
                _guiDispatcher.Invoke(() =>
                {
                    Board = board;
                });
        }

        public async void RollDice()
        {
            if (TurnStatus == TurnStatus.RollDice && CurrentTurn)
                await gameBl.RollDice(_room);
        }

        public void OnDiceResult(string room, int[] dice)
        {
            if (room == _room)
                Dice = dice;
        }

        public void ChooseChecker(int selection)
        {
            if (_checkerSelection == -1)
                _checkerSelection = selection;
            else
            {
                if (selection == 99)
                    ThrowChecker(_checkerSelection);
                else
                    PlayMove(_checkerSelection, selection);
                _checkerSelection = -1;
            }
        }

        public async void PlayMove(int source, int target)
        {
            if (!await gameBl.PlayMove(_room, source, target))
                MessageBox.Show("You Cant Play this move");
        }

        public async void ThrowChecker(int source)
        {
            if (!(await gameBl.CanThrowCheckers(_room) &&
                await gameBl.ThrowChecker(_room, source)))
                MessageBox.Show("You Cant Throw This Checker");
        }

        public void OnDenyInvitation(string username)
        {
            _guiDispatcher.Invoke(() =>
            {
                MessageBox.Show($"{username} deny your invitation");
            });
        }

        public async void GetBoard()
        {
            Board = await gameBl.GetBoard(_room);
        }

        public void OnGetColor(string roomId, CheckerColor color)
        {
            if (_room != roomId) return;
            if (color == CheckerColor.Black)
                PlayerColor = "Your Color is Black";
            else if (color == CheckerColor.White)
                PlayerColor = "Your Color is White";
        }

        public void OnCurrentTurn(string roomId, string username)
        {
            if (roomId != _room)
                return;
            if (UserName == username)
                CurrentTurn = true;
            else
            {
                CurrentTurn = false;
            }
        }

        public void OnGetPlayerName(string username)
        {
            UserName = username;
        }

        public void UpdateBoard(string roomid, Board board)
        {
            if (_room == roomid)
                Board = board;
        }

        public void GameOver(string room, string user)
        {
            if (_room != room)
                return;
            MessageBox.Show($"{user} Won the Game!");
        }

        public void OnChangeStatus(string roomId, TurnStatus status)
        {
            if (_room != roomId)
                return;
            TurnStatus = status;
            _guiDispatcher.Invoke(() =>
            {
                if (CurrentTurn)
                {
                    if (TurnStatus == TurnStatus.RollDice)
                        CurrentStatus = "Please roll dice";
                    else if (TurnStatus == TurnStatus.Move)
                        CurrentStatus = "Please play your moves";
                    else if (TurnStatus == TurnStatus.EndTurn)
                        CurrentStatus = "Your turn is ended";
                }
                if (TurnStatus == TurnStatus.GameOver)
                    CurrentStatus = "Game Over";
            });
        }

        public void OnGameClosed(string room, string sender)
        {
            if (room == _room)
                MessageBox.Show($"{sender} has left the game!");
        }

        public async void OnClosing()
        {
            await gameBl.CloseGame(_room);
        }
    }
}
