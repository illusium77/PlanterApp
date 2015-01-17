using PlanterApp.Applications.Services;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Data;
using System.Windows.Input;

namespace PlanterApp.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class ArchitectureViewModel : ViewModel<IArchitectureView>
    {
        //private object _architecture;
        //public object Architecture
        //{
        //    get { return _architecture; }
        //    set { SetProperty(ref _architecture, value); }
        //}

        private IExperimentService _experimentService;
        public IExperimentService ExperimentService
        {
            get { return _experimentService; }
            set { SetProperty(ref _experimentService, value); }
        }

        [ImportingConstructor]
        internal ArchitectureViewModel(IArchitectureView view, IExperimentService experimentService)
            : base(view)
        {
            _experimentService = experimentService;
        }
    }
}
