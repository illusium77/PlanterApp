using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using PlanterApp.Domain;
using PlanterApp.Applications.Views;

namespace PlanterApp.Applications.ViewModels
{
    [Export]
    internal class SettingsWindowViewModel : ViewModel<ISettingsWindow>
    {
        ISettingsView _settingView;

        public ISettingsView SettingView { get { return _settingView; } }
        
        [ImportingConstructor]
        public SettingsWindowViewModel(ISettingsWindow view, ISettingsView settingView, SettingsViewModel viewModel) : base(view)
        {
            _settingView = settingView;
            _settingView.DataContext = viewModel;

        }

        public void ShowDialog(object owner)
        {
            ViewCore.ShowDialog(owner);
        }
    }
}
