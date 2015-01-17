using System.ComponentModel.Composition;
using System.Windows;
using PlanterApp.Applications.Views;
using System.Windows.Controls;

namespace PlanterApp.Presentation.Views
{
    [Export(typeof(ITrayView))]
    public partial class TrayView : UserControl, ITrayView
    {
        public TrayView()
        {
            InitializeComponent();
        }
    }
}
