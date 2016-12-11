using System;
using System.Windows.Input;

namespace Player.Commands
{
    public class SimpleCommand : ICommand
    {

        private Action action;


        public SimpleCommand(Action command)
        {
            action = command;
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action();
        }

        public event EventHandler CanExecuteChanged;
    }
}