using System;
using System.Windows.Input;

namespace Player.Commands
{
    public class CommandWithParammeter: ICommand
    {
        private Action<object> action;


        public CommandWithParammeter(Action<object> actionCommand)
        {
            action = actionCommand;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}