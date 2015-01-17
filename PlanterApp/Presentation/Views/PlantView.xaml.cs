using PlanterApp.Applications.Services;
using PlanterApp.Applications.ViewModels;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using System;
using System.Linq;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlanterApp.Presentation.Views
{
    [Export(typeof(IPlantView))]
    public partial class PlantView : UserControl, IPlantView
    {
        private readonly Lazy<PlantViewModel> _viewModel;
        private bool _isMouseDown;

        public PlantView()
        {
            InitializeComponent();
            _viewModel = new Lazy<PlantViewModel>(() => ViewHelper.GetViewModel<PlantViewModel>(this));
        }

        private void OnContextMenuOpen(object sender, ContextMenuEventArgs e)
        {
            var model = _viewModel.Value;

            if (model == null || model.ExperimentService.IsTimeMachineOn == true || model != model.ExperimentService.SelectedPlant || model.ChangeStateMenu.Count() == 0)
            {
                // do now show context menu
                e.Handled = true;
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMouseDown == false)
            {
                return;
            }

            _isMouseDown = true;

            if (e.ChangedButton == MouseButton.Left)
            {
                var model = _viewModel.Value;

                if (model != null)
                {
                    model.CommandService.SelectPlantCommand.Execute(model);
                }
            }
        }
    }
}
