using PlanterApp.Applications.Services;
using PlanterApp.Applications.ViewModels;
using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Input;

namespace PlanterApp.Applications.Controllers
{
    [Export]
    internal class SelectionController : IController
    {
        private readonly TraitController _traitController;

        private readonly ICommandService _commandService;
        private readonly IExperimentService _experimentService;

        private ObservableCollection<PlantViewModel> _selectedPlantModels;
        public IEnumerable<PlantViewModel> SelectedPlants { get { return _selectedPlantModels; } }

        private bool _isCtrlDown;

        [ImportingConstructor]
        public SelectionController(TraitController traitController, ICommandService commandService, IExperimentService experimentService)
        {
            _traitController = traitController;
            _commandService = commandService;
            _experimentService = experimentService;

            _selectedPlantModels = new ObservableCollection<PlantViewModel>();
            _selectedPlantModels.CollectionChanged += OnCollectionChanged;

            _commandService.CtrlDownCommand = new DelegateCommand(p => _isCtrlDown = (bool)p);
            _commandService.SelectPlantCommand = new DelegateCommand(p => OnSelectPlant(p as PlantViewModel));
            _commandService.SelectMultiplePlantsCommand = new DelegateCommand(p => OnSelectMultiplePlants(p as IEnumerable<PlantViewModel>));
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (PlantViewModel model in e.OldItems)
                {
                    model.IsSelected = false;
                }
            }
            if (e.NewItems != null)
            {
                foreach (PlantViewModel model in e.NewItems)
                {
                    model.IsSelected = true;
                }
            }
        }

        public void Initialize()
        {
            _traitController.Initialize();
        }

        private void OnSelectPlant(PlantViewModel model)
        {
            OnSelectMultiplePlants(new List<PlantViewModel>() { model });
        }

        private void OnSelectMultiplePlants(IEnumerable<PlantViewModel> models)
        {
            if (_isCtrlDown == false)
            {
                Reset();
            }

            if (models != null)
            {
                foreach (var model in models)
                {
                    _selectedPlantModels.Add(model);
                }
            }

            _experimentService.SelectedPlant = _selectedPlantModels.LastOrDefault();
        }

        public void Reset()
        {
            _traitController.Reset();

            if (_selectedPlantModels == null || _selectedPlantModels.Count == 0)
            {
                return;
            }

            var statuses = new List<StatusItem>();
            foreach(var model in _selectedPlantModels)
            {
                if (model.Plant.PlantStatuses == null || model.Plant.PlantStatuses.Count == 0)
                {
                    continue;
                }

                if (statuses.Count == 0)
                {
                    statuses.AddRange(model.Plant.PlantStatuses);
                    continue;
                }

                var dublicates = (from state in model.Plant.PlantStatuses
                                 where statuses.Contains(state)
                                  select state).ToList();

                if (dublicates != null && dublicates.Count() > 0)
                {
                    foreach (var dublicateState in dublicates)
                    {
                        var clone = new StatusItem { State = dublicateState.State, TimeStamp = dublicateState.TimeStamp };
                        model.Plant.PlantStatuses.Remove(dublicateState);
                        model.Plant.PlantStatuses.Add(clone);
                    }
                }

                statuses.AddRange(model.Plant.PlantStatuses);
            }

            while (_selectedPlantModels.Count > 0)
            {
                _selectedPlantModels.RemoveAt(0);
            }
        }
    }
}
