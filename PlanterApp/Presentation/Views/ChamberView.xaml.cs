using System.ComponentModel.Composition;
using System.Windows;
using PlanterApp.Applications.Views;
using System.Windows.Controls;

namespace PlanterApp.Presentation.Views
{
    [Export(typeof(IChamberView))]
    public partial class ChamberView : UserControl, IChamberView
    {
        public ChamberView()
        {
            InitializeComponent();
        }
    }
}
