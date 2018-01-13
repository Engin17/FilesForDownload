using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XUpload
{
    public class UploadCommand : ICommand
    {
        private Action _eventHandler;

        public UploadCommand(Action handler)
        {
            _eventHandler = handler;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return MainWindowViewModel.IsButtonUploadEnabled;
        }

        public void Execute(object parameter)
        {
            _eventHandler();
        }
    }
}
