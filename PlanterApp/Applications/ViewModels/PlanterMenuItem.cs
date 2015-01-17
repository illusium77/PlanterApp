using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlanterApp.Applications.ViewModels
{
    internal class PlanterMenuItem : InputBinding
    {
        public PlanterMenuItem(ICommand command, PlanterMenuCommandParam commandParameter)
            : this(command, commandParameter, Key.None, ModifierKeys.None)
        {
        }

        public PlanterMenuItem(ICommand command, PlanterMenuCommandParam commandParameter, Key key, ModifierKeys modifier)
        {
            if (key != Key.None)
            {
                var gestureDisplayString = modifier == ModifierKeys.None ? key.ToString() : modifier.ToString() + "+" + key.ToString();
                var gesture = new KeyGesture(key, modifier, gestureDisplayString);

                Gesture = gesture;
            }

            Command = command;
            CommandParameter = commandParameter;
        }
    }

    internal class PlanterMenuCommandParam
    {
        public object TargetState { get; set; }
        public object Source { get; set; }
    }
}
