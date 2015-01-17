using PlanterApp.Applications.Services;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Data;

namespace PlanterApp.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class TraitViewModel : ViewModel<ITraitView>
    {
        private IExperimentService _experimentService;
        public IExperimentService ExperimentService
        {
            get { return _experimentService; }
            set { SetProperty(ref _experimentService, value); }
        }

        private object _architectureView;
        public object ArchitectureView
        {
            get { return _architectureView; }
            set { SetProperty(ref _architectureView, value); }
        }
        
        [ImportingConstructor]
        internal TraitViewModel(ITraitView view, IExperimentService experimentService)
            : base(view)
        {
            _experimentService = experimentService;
        }
    }
}
