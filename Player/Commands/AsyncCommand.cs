using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Player.Commands
{
    public class AsyncCommand:ICommand
    {

        private Func<Task> asyncTask;

        public AsyncCommand(Func<Task> action)
        {
            asyncTask = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync();
        }


        private Task ExecuteAsync()
        {
            return asyncTask();
        }

        public event EventHandler CanExecuteChanged;
    }
}