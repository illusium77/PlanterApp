using PlanterApp.Applications.Services;
using PlanterApp.Applications.ViewModels;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;

namespace PlanterApp.Presentation.Views
{
    [Export(typeof(IArchitectureMeristemView))]
    public partial class ArchitectureMeristemView : UserControl, IArchitectureMeristemView
    {
        public ArchitectureMeristemView()
        {
            InitializeComponent();
        }
    }
}
