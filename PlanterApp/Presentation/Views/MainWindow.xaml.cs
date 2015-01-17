using PlanterApp.Applications.ViewModels;
using PlanterApp.Applications.Views;
using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlanterApp.Presentation.Views
{
    [Export(typeof(IMainView))]
    public partial class MainWindow : Window, IMainView
    {
        private readonly Lazy<MainViewModel> _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new Lazy<MainViewModel>(() => ViewHelper.GetViewModel<MainViewModel>(this));
        }

         private void OnKeyUpDownEvent(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl)
            {
                return;
            }

            var model = _viewModel.Value;
            if (model != null && model.CommandService.CtrlDownCommand != null)
            {
                model.CommandService.CtrlDownCommand.Execute(e.IsDown);
            }
        }

         private void OnMouseWheel(object sender, MouseWheelEventArgs e)
         {
             var currentDate = sender as DatePicker;
             var model = _viewModel.Value;

             if (currentDate != null && model != null)
             {
                 if (e.Delta < 0 && model.CurrentDate > currentDate.DisplayDateStart)
                 {
                     model.CurrentDate = model.CurrentDate.AddDays(-1);
                 }
                 else if (e.Delta > 0 && model.CurrentDate < currentDate.DisplayDateEnd)
                 {
                     model.CurrentDate = model.CurrentDate.AddDays(1);
                 }
             }
         }
    }
}
