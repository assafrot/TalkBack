using Client.BL;
using Client.Commands;
using Client.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace Client.ViewModels
{
    class ChatVM : INotifyPropertyChanged, IClosing
    {
        public event PropertyChangedEventHandler PropertyChanged;
        ChatBL chatBl;
        private Dispatcher _guiDispatcher;
        public ObservableCollection<string> Chat { get; set; } = new ObservableCollection<string>();
        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
                SendMessageCommand.NotifyCanExecuteChanged();
            }
        }
        private string _room { get; set; }
        public CommandExecuter SendMessageCommand { get; set; }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ChatVM(string room)
        {
            this._room = room;
            chatBl = ChatBL.Instance;
            OnPropertyChanged(nameof(Chat));
            chatBl.OnNewMessage += OnNewMessage;
            chatBl.OnChatClosed += OnChatClosed;
            _guiDispatcher = Dispatcher.CurrentDispatcher;
            SendMessageCommand = new CommandExecuter(SendMessage, () => { return !string.IsNullOrWhiteSpace(Message); });
        }

        public async void SendMessage()
        {
            await chatBl.SendMessageToRoom(_room, Message);
            Message = "";
        }

        private void OnChatClosed(string room, string sender)
        {
            if (this._room == room)
                _guiDispatcher.Invoke(() =>
                {
                    Chat.Add($"{sender} has left the chat room.");
                });
        }

        public void OnNewMessage(string room, string msg)
        {
            _guiDispatcher.Invoke(() =>
            {
                if (this._room == room)
                    Chat.Add(msg);
            });
        }

        public async void OnClosing()
        {
            await chatBl.CloseChat(_room);
        }
    }
}
