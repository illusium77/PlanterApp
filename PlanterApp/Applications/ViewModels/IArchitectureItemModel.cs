using System;
using System.Collections;
using System.ComponentModel;

namespace PlanterApp.Applications.ViewModels
{
    internal interface IArchitectureItemModel : INotifyPropertyChanged
    {
        event EventHandler StatesUpdated;

        IArchitectureItemModel Parent { get; set; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        bool Focusable { get; set; }
        ICollection PlanterMenuItems { get; }
    }
}
