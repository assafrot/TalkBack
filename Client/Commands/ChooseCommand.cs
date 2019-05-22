using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.Commands
{
    class ChooseCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        Action<int> ExecuteAction { get; }

        public ChooseCommand(Action<int> executeAction)
        {
            ExecuteAction = executeAction;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            int value = int.Parse((string)parameter) ;
            ExecuteAction(value);
        }

        public void NotifyCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }
    }
}
