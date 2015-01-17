using PlanterApp.Domain;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Collections.ObjectModel;
using System.Text;

namespace PlanterApp.Applications.Services
{
    [Export(typeof(IViewService)), PartCreationPolicy(CreationPolicy.Shared)]
    internal class ViewService : Model, IViewService
    {
        private object _plantPropertyView;
        public object PlantPropertyView
        {
            get { return _plantPropertyView; }
            set { SetProperty(ref _plantPropertyView, value); }
        }

        private object _statisticView;
        public object StatisticView
        {
            get { return _statisticView; }
            set { SetProperty(ref _statisticView, value); }
        }

        private object _traitView;
        public object TraitView
        {
            get { return _traitView; }
            set { SetProperty(ref _traitView, value); }
        }
    }
}
