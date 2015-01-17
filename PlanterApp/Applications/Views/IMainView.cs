using System.Waf.Applications;

namespace PlanterApp.Applications.Views
{
    internal interface IMainView : IView
    {
        void Show();

        void Close();
    }
}
