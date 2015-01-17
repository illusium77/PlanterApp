using System.ComponentModel.Composition;
using System.Windows;
using PlanterApp.Applications.Views;
using System.Windows.Controls;

namespace PlanterApp.Presentation.Views
{
    [Export(typeof(IPlantPropertyView))]
    public partial class PlantPropertyView : UserControl, IPlantPropertyView 
    {
        public PlantPropertyView()
        {
            InitializeComponent();
        }
    }
}
