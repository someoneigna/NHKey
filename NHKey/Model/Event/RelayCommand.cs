using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NHKey.Model.Event
{
    public class RelayCommand : ICommand
    {    

        public Action Action { get; set; }
        public Func<bool> Condition { get; set; }

        public RelayCommand()
        {
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null) 
                CanExecuteChanged(this, new EventArgs());
        }

        public bool CanExecute(object parameter)
        {
            if (Condition == null)
            {
                return true;
            }
            return Condition.Invoke();
        }

        public void Execute(object parameter) => Action.Invoke();
    }
}
