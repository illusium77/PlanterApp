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
    [Export(typeof(IStatisticView))]
    public partial class StatisticView : UserControl, IStatisticView
    {
        private readonly Lazy<StatisticViewModel> _viewModel;

        public StatisticView()
        {
            InitializeComponent();
            _viewModel = new Lazy<StatisticViewModel>(() => ViewHelper.GetViewModel<StatisticViewModel>(this));
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid == null)
            {
                return;
            }

            var selectedLine = grid.SelectedItem as StaticticLineModel;
            if (selectedLine == null)
            {
                return;
            }

            if (_viewModel != null)
            {
                _viewModel.Value.CommandService.SelectMultiplePlantsCommand.Execute(selectedLine.PlantModels);
            }
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid == null)
            {
                return;
            }

            grid.SelectedItem = null;
        }
    }
}
