using Client.BL;
using Client.Commands;
using Client.Models;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Client.ViewModels
{
    class LogInVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CommandExecuter CreateUserCommand { get; set; }
        public CommandExecuter LoginCommand { get; set; }
        public CommandExecuter LogOutCommand { get; set; }
        UserBL userBl;
        private string _userName;
        private string _password;
        private string _error;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
                CreateUserCommand.NotifyCanExecuteChanged();
                LoginCommand.NotifyCanExecuteChanged();

            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                CreateUserCommand.NotifyCanExecuteChanged();
                LoginCommand.NotifyCanExecuteChanged();
            }
        }
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
                OnPropertyChanged(nameof(Error));
            }
        }
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public LogInVM()
        {
            userBl = UserBL.Instance;
            CreateUserCommand = new CommandExecuter
                (Register, () => { return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password); });
            LoginCommand = new CommandExecuter(
                Login, () => { return !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password); });
        }

        public async void Register()
        {
            if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
            {
                User user = new User(UserName, Password);
                Error = await userBl.Register(user);
                if (Error == String.Empty)
                {
                    MainWindowVM mainVM = new MainWindowVM(user);
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.DataContext = mainVM;
                    mainWindow.Visibility = Visibility.Visible;

                    var window = Application.Current.Windows[0];
                    window.Close();
                }
                else MessageBox.Show(Error);
            }
        }

        public async void Login()
        {
            if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
            {
                User user = new User(UserName, Password);
                Error = await userBl.Login(user);
                if (Error == String.Empty)
                {
                    MainWindowVM mainVM = new MainWindowVM(user);
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.DataContext = mainVM;
                    mainWindow.Visibility = Visibility.Visible;
                    var window = Application.Current.Windows[0];
                    window.Close();
                }
                else
                    MessageBox.Show(Error);
            }
        }

    }
}
