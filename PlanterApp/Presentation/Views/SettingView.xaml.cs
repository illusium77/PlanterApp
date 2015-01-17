using System.ComponentModel.Composition;
using System.Windows;
using PlanterApp.Applications.Views;
using System.Windows.Controls;

namespace PlanterApp.Presentation.Views
{
    [Export(typeof(ISettingsView))]
    public partial class SettingView : UserControl, ISettingsView
    {
        public SettingView()
        {
            InitializeComponent();
        }
    }
}
