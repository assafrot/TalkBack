using Client.BL;
using Client.Commands;
using Client.Interfaces;
using Client.Models;
using Client.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;


namespace Client.ViewModels
{
    class MainWindowVM : INotifyPropertyChanged, IClosing
    {
        public event PropertyChangedEventHandler PropertyChanged;
        UserBL userBl;
        ChatBL chatBl;
        GameBL gameBl;
        public ObservableCollection<string> Users { get; set; }
        public ObservableCollection<string> OnlineUsers { get; set; }
        public ObservableCollection<Invitation> Invitations { get; set; }
        private Dispatcher _guiDispatcher;
        private string _selectedUser;
        public string SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                StartChatCommand.NotifyCanExecuteChanged();
                InviteToGameCommand.NotifyCanExecuteChanged();
            }
        }
        public string UserWelcome { get; set; }
        public CommandExecuter InviteToGameCommand { get; set; }
        public CommandExecuter StartChatCommand { get; set; }
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainWindowVM(User user)
        {
            OnPropertyChanged(nameof(Users));
            OnPropertyChanged(nameof(OnlineUsers));
            OnPropertyChanged(nameof(Invitations));
            userBl = UserBL.Instance;
            chatBl = ChatBL.Instance;
            gameBl = GameBL.Instance;
            _guiDispatcher = Dispatcher.CurrentDispatcher;
            userBl.OnLoggedIn += OnUserLoggedIn;
            userBl.OnLoggedOut += OnUserLoggedOut;
            userBl.OnRoomOpen += OnChatRoomOpen;
            gameBl.OnGameInvitation += OnGameInvitation;
            GetAllUsers(user);
            UserWelcome = $"Welcome {user.Name}";
            StartChatCommand = new CommandExecuter(StartChat, () =>
            {
                return SelectedUser != null;
            });
            InviteToGameCommand = new CommandExecuter(InviteToGame, () =>
            {
                return SelectedUser != null;
            });
            Invitations = new ObservableCollection<Invitation>();
        }

        public async void GetAllUsers(User user)
        {
            Users = new ObservableCollection<string>();
            OnlineUsers = new ObservableCollection<string>();
            IEnumerable<string> userList = await userBl.GetAllUsers(user);
            foreach (var u in userList)
            {
                if (await userBl.IsUserOnline(u)) OnlineUsers.Add(u);
                else Users.Add(u);
            }
        }

        public void OnUserLoggedIn(string userName)
        {
            _guiDispatcher.Invoke(() =>
            {
                Users.Remove(userName);
                OnlineUsers.Add(userName);
            });
        }

        public void OnUserLoggedOut(string userName)
        {
            _guiDispatcher.Invoke(() =>
            {
                OnlineUsers.Remove(userName);
                Users.Add(userName);
            });
        }

        public async void StartChat()
        {
            string room = await chatBl.JoinRoom(SelectedUser);
            _guiDispatcher.Invoke(() =>
            {
                if (room != String.Empty)
                {
                    ChatVM chatVM = new ChatVM(room);
                    Chat chat = new Chat();
                    chat.DataContext = chatVM;
                    chat.Visibility = Visibility.Visible;
                }
            });
        }

        public void OnChatRoomOpen(string room)
        {
            _guiDispatcher.Invoke(() =>
            {
                if (room != String.Empty)
                {
                    ChatVM chatVM = new ChatVM(room);
                    Chat chat = new Chat();
                    chat.DataContext = chatVM;
                    chat.Visibility = Visibility.Visible;
                }
            });
        }

        private async void InviteToGame()
        {
            string room = await gameBl.InviteToGame(SelectedUser);
            _guiDispatcher.Invoke(() =>
            {
                GameVM gameVm = new GameVM(room);
                GameRoom gameRoom = new GameRoom();
                gameRoom.DataContext = gameVm;
                gameRoom.Visibility = Visibility.Visible;
            });
        }

        public void OnGameInvitation(string room, string sender)
        {
            _guiDispatcher.Invoke(() =>
            {
                var invitation = new Invitation(sender, room);
                invitation.OnResponse += OnInvitationResponse;
                Invitations.Add(invitation);
            });
        }

        public void OnInvitationResponse(string room)
        {
            _guiDispatcher.Invoke(() =>
            {
                var invitation = Invitations.Where(i => i.Room == room).FirstOrDefault();
                Invitations.Remove(invitation);
            });
        }

        public async void OnClosing()
        {
            await userBl.Logout();
            Application.Current.Shutdown();
        }
    }
}
