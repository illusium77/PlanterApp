using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Waf.Foundation;
using System.Windows.Data;

namespace PlanterApp.Applications.ViewModels
{
    internal class StaticticLineModel : Model
    {
        private string _species;
        public string Species
        {
            get { return _species; }
        }

        private string _population;
        public string Population
        {
            get { return _population; }
        }

        private string _toolTip;
        public string ToolTip
        {
            get { return _toolTip; }
            set { SetProperty(ref _toolTip, value); }
        }

        private Dictionary<string, StatisticValue> _values;
        public Dictionary<string, StatisticValue> Values
        {
            get { return _values; }
            set { SetProperty(ref _values, value); }
        }

        private CollectionViewSource _filteredModelView;
        public IEnumerable<PlantViewModel> PlantModels
        {
            get { return _filteredModelView.View.OfType<PlantViewModel>(); }
        }


        public StaticticLineModel(string species, string population, CollectionViewSource filteredModelView)
        {
            _species = species;
            _population = population;
            _filteredModelView = filteredModelView;

            Initialize();
        }

        public void Refresh()
        {
            _filteredModelView.View.Refresh();

            Update();
        }

        private void Initialize()
        {
            Values = new Dictionary<string, StatisticValue>();

            foreach (var model in PlantModels)
            {
                PropertyChangedEventManager.AddHandler(model, OnPlantCurrentStateChanged, "CurrentState");
                model.ArchitectureUpdated += OnArchitectureRefreshRequired;
            }

            ToolTip = Species + " " + Population;

            Values.Clear();
            Values.Add("Germinated", new GerminatedStatisticValue(PlantModels));
            Values.Add("Alive", new AliveStatisticValue(PlantModels));
            Values.Add("Buds", new BudsStatisticValue(PlantModels));
            Values.Add("Flower", new FlowerStatisticValue(PlantModels));
            Values.Add("MeristemCount", new MeristemCountStatisticValue(PlantModels));
            Values.Add("Stem", new StemStatisticValue(PlantModels));
            Values.Add("Branch", new BranchStatisticValue(PlantModels));
            Values.Add("Secondary", new SecondaryStatisticValue(PlantModels));
            Values.Add("MeanGerminationDays", new MeanGerminationStatisticValue(PlantModels));
            Values.Add("MeanBudsDays", new MeanBudsStatisticValue(PlantModels));
            Values.Add("MeanFloweringDays", new MeanFloweringStatisticValue(PlantModels));
            Values.Add("MeanSeedsDays", new MeanSeedsStatisticValue(PlantModels));
            Values.Add("MeanFloweringSeedsDays", new MeanFloweringSeedsStatisticValue(PlantModels));
            Values.Add("MeanBudsFloweringDays", new MeanBudsFloweringStatisticValue(PlantModels));
            Values.Add("BasalBranchPercentage", new BasalBranchStatisticValue(PlantModels));
            Values.Add("FlowerMeristemPercentage", new FlowerMeristemStatisticValue(PlantModels));

            Update();
        }

        void OnArchitectureRefreshRequired(object sender, EventArgs e)
        {
            Update();
        }

        private void Update()
        {
            foreach (StatisticValue value in Values.Values)
            {
                value.Update();
            }
        }

        private void OnPlantCurrentStateChanged(object sender, PropertyChangedEventArgs e)
        {
            Update();
        }

        public void Unsubscribe()
        {
            foreach (var model in PlantModels)
            {
                PropertyChangedEventManager.RemoveHandler(model, OnPlantCurrentStateChanged, "CurrentState");
                model.ArchitectureUpdated -= OnArchitectureRefreshRequired;
            }

            _filteredModelView.View.Filter = null;
        }
    }
}
