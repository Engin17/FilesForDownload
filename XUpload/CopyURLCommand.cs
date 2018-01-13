using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XUpload
{
    public class CopyURLCommand : ICommand
    {
        private Action _eventHandler;

        public CopyURLCommand(Action handler)
        {
            _eventHandler = handler;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _eventHandler();
        }
    }
}
