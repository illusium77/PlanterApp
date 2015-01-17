using PlanterApp.Applications.Views;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace PlanterApp.Presentation.Views
{
    [Export(typeof(ITraitView))]
    public partial class TraitView : UserControl, ITraitView
    {
        public TraitView()
        {
            InitializeComponent();
        }
    }
}
