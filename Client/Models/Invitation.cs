using Client.BL;
using Client.Commands;
using Client.ViewModels;
using Client.Views;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Client.Models
{
    class Invitation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _sender;
        public string Sender
        {
            get { return _sender; }
            set
            {
                _sender = value;
                OnPropertyChanged(nameof(Sender));
            }
        }
        public string Room { get; set; }
        GameBL gameBl;
        public event Action<string> OnResponse;
        public CommandExecuter AcceptInvitationCommand { get; set; }
        public CommandExecuter DenyInvitationCommand { get; set; }
        private Dispatcher _guiDispatcher;

        public Invitation(string sender, string room)
        {
            Room = room;
            gameBl = GameBL.Instance;
            Sender = sender;
            _guiDispatcher = Dispatcher.CurrentDispatcher;
            AcceptInvitationCommand = new CommandExecuter(Accept, () => { return true; });
            DenyInvitationCommand = new CommandExecuter(Deny, () => { return true; });
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public async void Accept()
        {
            _guiDispatcher.Invoke(() =>
            {
                OnResponse?.Invoke(Room);
                GameVM gameVm = new GameVM(Room);
                GameRoom gameRoom = new GameRoom();
                gameRoom.DataContext = gameVm;
                gameRoom.Visibility = Visibility.Visible;
            });
            await gameBl.AcceptInvitation(Room, Sender);
        }

        public async void Deny()
        {
            OnResponse?.Invoke(Room);
            await gameBl.DenyInvitation(Sender);
        }
    }
}
