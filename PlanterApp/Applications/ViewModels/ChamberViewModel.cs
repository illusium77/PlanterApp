using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;

namespace PlanterApp.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class ChamberViewModel : ViewModel<IChamberView>
    {
        private Chamber _chamber;

        public Chamber Chamber
        {
            get { return _chamber; }
            set { SetProperty(ref _chamber, value); }
        }

        private SynchronizingCollection<TrayViewModel, Tray> _trays;
        public SynchronizingCollection<TrayViewModel, Tray> Trays
        {
            get { return _trays; }
            set { SetProperty(ref _trays, value); }
        }

        public IEnumerable<PlantViewModel> Plants
        {
            get
            {
                //var plantModels = new List<PlantViewModel>();
                //foreach (var trayModel in Trays)
                //{
                //    plantModels.AddRange(trayModel.Plants);
                //}

                //return plantModels;

                return (from tray in _trays
                        select tray.Plants).SelectMany(p => p);
            }
        }

        [ImportingConstructor]
        public ChamberViewModel(IChamberView view)
            : base(view)
        {
        }

        internal void Reset()
        {
            if (_trays == null)
            {
                return;
            }

            foreach (var tray in _trays)
            {
                tray.Reset();
            }

            _trays = null;

        }
    }
}
