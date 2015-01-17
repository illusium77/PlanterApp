using PlanterApp.Applications.Views;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace PlanterApp.Presentation.Views
{
    [Export(typeof(IArchitectureNodeView))]
    public partial class ArchitectureNodeView : UserControl, IArchitectureNodeView
    {
        public ArchitectureNodeView()
        {
            InitializeComponent();
        }
    }
}
