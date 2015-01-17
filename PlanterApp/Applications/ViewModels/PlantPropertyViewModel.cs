using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using PlanterApp.Applications.Services;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;

namespace PlanterApp.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.Shared)]
    internal class PlantPropertyViewModel : ViewModel<IPlantPropertyView>
    {
        private IExperimentService _experimentService;
        public IExperimentService ExperimentService
        {
            get { return _experimentService; }
            set { SetProperty(ref _experimentService, value); }
        }

        [ImportingConstructor]
        public PlantPropertyViewModel(IPlantPropertyView view, IExperimentService experimentService)
            : base(view)
        {
            _experimentService = experimentService;
        }
    }
}
