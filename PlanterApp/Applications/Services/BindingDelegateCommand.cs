using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Input;

namespace PlanterApp.Applications.Services
{
    internal class BindingDelegateCommand : DelegateCommand
    {
        private InputBinding _binding;

        public BindingDelegateCommand(Action<object> execute, Key key, ModifierKeys modifier)
            : base(execute)
        {
            InitBinding(this, key, modifier);
        }

        public BindingDelegateCommand(Action execute, Key key, ModifierKeys modifier)
            : base(execute)
        {
            InitBinding(this, key, modifier);
        }

        public BindingDelegateCommand(Action<object> execute, Func<object, bool> canExecute, Key key, ModifierKeys modifier)
            : base(execute, canExecute)
        {
            InitBinding(this, key, modifier);
        }

        public BindingDelegateCommand(Action execute, Func<bool> canExecute, Key key, ModifierKeys modifier) : base(execute, canExecute)
        {
            InitBinding(this, key, modifier);
        }

        private static void InitBinding(BindingDelegateCommand source, Key key, ModifierKeys modifier)
        {
            var gestureDisplayString = modifier == ModifierKeys.None ? key.ToString() : modifier.ToString() + "+" + key.ToString();
            var gesture = new KeyGesture(key, modifier, gestureDisplayString);

            source._binding = new InputBinding(source, gesture);
        }



        //class PlanterMenuItem : InputBinding
        //{
        //    public PlanterMenuItem(ICommand command, object commandParameter, Key key, ModifierKeys modifier)
        //    {
        //        var gestureDisplayString = modifier == ModifierKeys.None ? key.ToString() : modifier.ToString() + "+" + key.ToString();
        //        var gesture = new KeyGesture(key, modifier, gestureDisplayString);

        //        Command = command;
        //        Gesture = gesture;
        //        CommandParameter = commandParameter;
        //    }
        //}

    }
}
