using ModelUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModelUI.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }


        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }


        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;


        public void Execute(object parameter)
        {
            IntPtr cursor = IntPtr.Zero;
            try
            {
                cursor = System32.GetCursor();
                IntPtr handle = System32.LoadCursor(new HandleRef(), 0x7f02);
                HandleRef inst = new HandleRef(parameter, handle);
                System32.SetCursor(inst);
                _execute(parameter);
            }
            catch (Exception)
            {
                //Logger.Log("RelayCommand.Execute: Unhandled exception found executing command", TraceEventType.Error, ex);
                throw;
            }
            finally
            {
                System32.SetCursor(new HandleRef(parameter, cursor));
            }
        }

        public void RaiseCanExecuteChanged(object param = null)
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(param, new EventArgs());
        }

        #endregion

    }
}
