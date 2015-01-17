using PlanterApp.Applications.ViewModels;
using PlanterApp.Applications.Views;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlanterApp.Presentation.Views
{
    [Export(typeof(IArchitectureView))]
    public partial class ArchitectureView : UserControl, IArchitectureView
    {
        private IArchitectureItemModel _lastItem;
        //private readonly Lazy<ArchitectureViewModel> _viewModel;

        public ArchitectureView()
        {
            InitializeComponent();
            //_viewModel = new Lazy<ArchitectureViewModel>(() => ViewHelper.GetViewModel<ArchitectureViewModel>(this));
        }

        private void OnNodeSelected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            //var model = _viewModel.Value;

            if (/*model == null ||*/ e.NewValue == null)
            {
                return;
            }

            var item = e.NewValue as IArchitectureItemModel;

            if (item != null)
            {
                if (_lastItem != null)
                {
                    PropertyChangedEventManager.RemoveHandler(_lastItem, OnRefreshInputBindings, "PlanterMenuItems");
                }

                PropertyChangedEventManager.AddHandler(item, OnRefreshInputBindings, "PlanterMenuItems");

                OnRefreshInputBindings(item, null);

                _lastItem = item;
            }
        }


        private void OnRefreshInputBindings(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as IArchitectureItemModel;
            if (item != null)
            {
                InputBindings.Clear();

                var itemBindings = item.PlanterMenuItems;

                if (itemBindings != null)
                {
                    InputBindings.AddRange(itemBindings);
                }
            }
        }
    }
}
