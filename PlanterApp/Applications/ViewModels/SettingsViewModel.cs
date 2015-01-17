using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;

namespace PlanterApp.Applications.ViewModels
{
    [Export]
    internal class SettingsViewModel : ViewModel<ISettingsView>
    {
        [ImportingConstructor]
        public SettingsViewModel(ISettingsView view) : base(view)
        {
        }
    }
}
