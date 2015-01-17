using System.Waf.Applications;

namespace PlanterApp.Applications.Views
{
    interface ISettingsWindow : IView
    {
        void ShowDialog(object owner);
    }
}
