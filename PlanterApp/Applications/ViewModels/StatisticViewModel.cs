using PlanterApp.Applications.Services;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Data;

namespace PlanterApp.Applications.ViewModels
{
    [Export]
    internal class StatisticViewModel : ViewModel<IStatisticView>
    {
        private readonly IExperimentService _experimentService;

        private ICommandService _commandService;
        public ICommandService CommandService
        {
            get { return _commandService; }
            set { SetProperty(ref _commandService, value); }
        }

        public bool IsEnabled
        {
            get { return StatisticLines != null ? true : false; }
        }

        private ObservableCollection<StaticticLineModel> _statisticLines;
        public ObservableCollection<StaticticLineModel> StatisticLines
        {
            get { return _statisticLines; }
        }

        private IEnumerable<PlantViewModel> _plantModels;
        public IEnumerable<PlantViewModel> PlantModels
        {
            get { return _plantModels; }
            set
            {
                Unsubscribe();

                if (SetProperty(ref _plantModels, value) && _plantModels != null)
                {
                    PrepareStatistics();
                    SubscribeToPropertyChanges();
                }

                RaisePropertyChanged("IsEnabled");
            }
        }

        [ImportingConstructor]
        public StatisticViewModel(IStatisticView view, IExperimentService experimentService, ICommandService commandService)
            : base(view)
        {
            _experimentService = experimentService;
            _commandService = commandService;

            _statisticLines = new ObservableCollection<StaticticLineModel>();
        }

        private void PrepareStatistics()
        {
            StatisticLines.Clear();

            foreach (var species in _experimentService.Experiment.Properties.Species)
            {
                foreach (var population in species.Populations)
                {
                    var filteredModelView = new CollectionViewSource { Source = _plantModels };
                    filteredModelView.View.Filter = (m) =>
                    {
                        var model = m as PlantViewModel;
                        return model != null ? model.Plant.Species == species.Id && model.Plant.Population == population.Id : false;
                    };

                    StatisticLines.Add(new StaticticLineModel(species.Name, population.Name, filteredModelView));
                }
            }
        }

        private void SubscribeToPropertyChanges()
        {
            foreach (var model in _plantModels)
            {
                PropertyChangedEventManager.AddHandler(model.Plant, OnPlantPropertyChanged, string.Empty);
            }
        }

        private void Unsubscribe()
        {
            if (_plantModels != null && _plantModels.Count() > 0)
            {
                foreach (var model in _plantModels)
                {
                    PropertyChangedEventManager.RemoveHandler(model.Plant, OnPlantPropertyChanged, string.Empty);
                }
            }

            if (_statisticLines != null && _statisticLines.Count > 0)
            {
                foreach (var line in _statisticLines)
                {
                    line.Unsubscribe();
                }
            }
        }

        private void OnPlantPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Species" || e.PropertyName == "Population")
            {
                var plant = sender as Plant;
                if (plant == null)
                {
                    return;
                }

                var oldLine = (from line in _statisticLines
                               where line.PlantModels.Any(m => m.Plant == plant)
                               select line).FirstOrDefault();

                if (oldLine != null)
                {
                    oldLine.Refresh();
                }

                var names = _experimentService.Experiment.Properties.GetNames(plant);
                var newLine = (from line in _statisticLines
                               where line.Species == names[ExperimentProperties.Names.Species] && line.Population == names[ExperimentProperties.Names.Population]
                               select line).FirstOrDefault();

                if (newLine != null)
                {
                    newLine.Refresh();
                }
            }
        }

        internal void Reset()
        {
            PlantModels = null;
        }
    }
}
