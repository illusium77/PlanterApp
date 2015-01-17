using PlanterApp.Domain;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Waf.Foundation;
using System.Collections.ObjectModel;
using System.Text;

namespace PlanterApp.Applications.Services
{
    [Export(typeof(IExperimentService)), PartCreationPolicy(CreationPolicy.Shared)]
    internal class ExperimentService : Model, IExperimentService
    {
        //private ObsoleteSettings _settings;
        //public ObsoleteSettings ObSettings
        //{
        //    get { return _settings; }
        //    set { SetProperty(ref _settings, value); }
        //}

        private Experiment _experiment;
        public Experiment Experiment
        {
            get { return _experiment; }
            set
            {
                SetProperty(ref _experiment, value);
                HasExperiment = _experiment != null && _experiment.HasData;
            }
        }

        private object _selectedPlant;
        public object SelectedPlant
        {
            get { return _selectedPlant; }
            set { SetProperty(ref _selectedPlant, value); }
        }

        private object _selectedPlantArchitecture;
        public object SelectedPlantArchitecture
        {
            get { return _selectedPlantArchitecture; }
            set { SetProperty(ref _selectedPlantArchitecture, value); }
        }

        private bool _hasExperiment;
        public bool HasExperiment
        {
            get { return _hasExperiment; }
            set { SetProperty(ref _hasExperiment, value); }
        }

        private bool _isTimeMachineOn;
        public bool IsTimeMachineOn
        {
            get { return _isTimeMachineOn; }
            set { SetProperty(ref _isTimeMachineOn, value); }
        }

        private TrayType _trayType;
        public TrayType TrayType
        {
            get { return _trayType; }
            set { SetProperty(ref _trayType, value); }
        }

        //public ExperimentService()
        //{
        //    _settings = new ObsoleteSettings();
        //}

        public IList<string> Validate()
        {
            if (Experiment == null)
            {
                throw new InvalidOperationException("Experiment cannot be null!");
            }

            var duplicateNotifications = new List<string>();
            var reservedIndividuals = new List<string>();
            bool individualUpdated = false;


            return duplicateNotifications; // TODO

            var orderedPlants = Experiment.GetPlants().OrderBy(p => p.Id);

            foreach (var plant in orderedPlants)
            {
                var indPrefix = plant.Species + "-" + plant.Population + "-" + plant.Family + "-";

                int ind;
                if (string.IsNullOrEmpty(plant.Individual) || int.TryParse(plant.Individual, out ind) == false)
                {
                    plant.Individual = "1";
                }

                var originalIndividual = plant.Individual;
                while (reservedIndividuals.Contains(indPrefix + plant.Individual))
                {
                    individualUpdated = true;

                    ind = int.Parse(plant.Individual) + 1;
                    plant.Individual = ind.ToString();

                }

                if (individualUpdated)
                {
                    individualUpdated = false;

                    duplicateNotifications.Add("Plant individual conflict found, plant id " + plant.Id + "'s individual updated: '"
                        + indPrefix + originalIndividual + "' -> '" + indPrefix + plant.Individual);
                }
                reservedIndividuals.Add(indPrefix + plant.Individual);
            }

            return duplicateNotifications;
        }

        public IList<string> CompareTo(Experiment other)
        {
            var changes = new List<string>();

            var originalPlants = new List<Plant>(Experiment.GetPlants());
            var changedPlants = new List<Plant>(other.GetPlants());

            int numPlants = originalPlants.Count;

            if (numPlants != changedPlants.Count)
            {
                changes.Add("Orginal has " + numPlants + " plants while other has " + changedPlants.Count + " plants.");
                numPlants = Math.Min(numPlants, changedPlants.Count);
            }

            for (int i = 0; i < numPlants; i++)
            {
                var difference = ComparePlants(originalPlants[i], changedPlants[i]);
                if (difference != null)
                {
                    changes.Add(difference);
                }
            }


            return changes;
        }

        private string ComparePlants(Plant plantA, Plant plantB)
        {
            var differences = new StringBuilder();

            if (plantA.Id != plantB.Id)
            {
                differences.AppendLine("\tPlant A ID=" + plantA.Id + " Plant B ID=" + plantB.Id);
            }

            if (plantA.Species != plantB.Species)
            {
                differences.AppendLine("\tPlant A Species=" + plantA.Species + " Plant B Species=" + plantB.Species);
            }

            if (plantA.Population != plantB.Population)
            {
                differences.AppendLine("\tPlant A Population=" + plantA.Population + " Plant B Population=" + plantB.Population);
            }

            if (plantA.Family != plantB.Family)
            {
                differences.AppendLine("\tPlant A Family=" + plantA.Family + " Plant B Family=" + plantB.Family);
            }

            if (plantA.Individual != plantB.Individual)
            {
                differences.AppendLine("\tPlant A Individual=" + plantA.Individual + " Plant B Individual=" + plantB.Individual);
            }

            if (plantA.Notes != plantB.Notes)
            {
                differences.AppendLine("\tPlant A Notes=" + plantA.Notes + " Plant B Notes=" + plantB.Notes);
            }

            if (plantA.IsExcluded != plantB.IsExcluded)
            {
                differences.AppendLine("\tPlant A IsExcluded=" + plantA.IsExcluded + " Plant B IsExcluded=" + plantB.IsExcluded);
            }

            if (plantA.IsTransplanted != plantB.IsTransplanted)
            {
                differences.AppendLine("\tPlant A IsTransplanted=" + plantA.IsTransplanted + " Plant B IsTransplanted=" + plantB.IsTransplanted);
            }

            var statusCount = plantA.PlantStatuses.Count;
            if (statusCount != plantB.PlantStatuses.Count)
            {
                differences.AppendLine("\tPlant A has " + statusCount + " statuses Plant B has " + plantB.PlantStatuses.Count);
                statusCount = Math.Max(statusCount, plantB.PlantStatuses.Count);
            }

            for (int i = 0; i < statusCount; i++)
            {
                if (i < plantA.PlantStatuses.Count && i < plantB.PlantStatuses.Count)
                {
                    if (plantA.PlantStatuses[i].State != plantB.PlantStatuses[i].State
                        || plantA.PlantStatuses[i].TimeStamp != plantB.PlantStatuses[i].TimeStamp)
                    {
                        differences.AppendLine("\t\tPlant A status " + i + " = " + plantA.PlantStatuses[i].State + " " + plantA.PlantStatuses[i].TimeStamp);
                        differences.AppendLine("\t\tPlant B status " + i + " = " + plantB.PlantStatuses[i].State + " " + plantB.PlantStatuses[i].TimeStamp);

                        if (plantA.PlantStatuses[i].State == plantB.PlantStatuses[i].State
                            && plantA.PlantStatuses[i].TimeStamp != plantB.PlantStatuses[i].TimeStamp)
                        {
                            if (plantA.PlantStatuses[i].State == PlantState.Alive
                                && plantA.PlantStatuses[i].TimeStamp.Date == DateTime.Now.Date)
                            {
                                var fixedStatus = new StatusItem { State = PlantState.Alive, TimeStamp = plantB.PlantStatuses[i].TimeStamp };
                                differences.AppendLine("\t\tUpdate incorrect timestamp into " + fixedStatus.TimeStamp);

                                plantA.PlantStatuses[i] = fixedStatus;
                            }
                        }
                    }
                }
                else if (i < plantA.PlantStatuses.Count)
                {
                    differences.AppendLine("\t\tPlant A status " + i + " = " + plantA.PlantStatuses[i].State + " " + plantA.PlantStatuses[i].TimeStamp);
                    differences.AppendLine("\t\tPlant B status is missing");
                }
                else if (i < plantB.PlantStatuses.Count)
                {
                    differences.AppendLine("\t\tPlant A status is missing");
                    differences.AppendLine("\t\tPlant B status " + i + " = " + plantB.PlantStatuses[i].State + " " + plantB.PlantStatuses[i].TimeStamp);
                }
            }

            return differences.Length == 0 ? null : "Plant ID " + plantA.Id + " has following differences:" + Environment.NewLine + differences.ToString();
        }


        public void Reset()
        {
            SelectedPlant = null;
            SelectedPlantArchitecture = null;
            Experiment = null;
        }

        #region TempRuns

        public void ProcessTemporaryHacks()
        {
            if (Experiment == null)
            {
                throw new InvalidOperationException("Experiment cannot be null!");
            }

            AddTraits();

            //HasExperiment = _experiment != null && _experiment.HasData;

            //if (_experiment.Properties == null)
            //{
            //    InitExperimentProperties();
            //    SwitchToIds();
            //}


            //UpdatePlantedDates();
            //RenameChamber6Trays();
            //RemoveArchitecture();
            //ParseTransplanted();
            //MigrateToNewStatuses();
            //UpdatePlantingDates();
            //ConvertToIndexes(experiment);
            //CloneChambers();
            //DumpIds();
            //AddStemNodeToAlivePlants();
            //ImportNodeDump();
            //InitializeNewArchitecture();
            //FixMeristemDates();
        }

        private void AddTraits()
        {
            if (_experiment.Properties.PlantTraitNames == null || _experiment.Properties.PlantTraitNames.Count == 0)
            {
                _experiment.Properties.PlantTraitNames = new ObservableCollection<TraitName>();
                for (int i = 0; i < 10; i++)
                {
                    _experiment.Properties.PlantTraitNames.Add(new TraitName(i.ToString()) { Name = "Trait " + (i + 1) });
                }

                foreach (var plant in _experiment.GetPlants())
                {
                    plant.InitializeTraitValues(_experiment.Properties.PlantTraitNames);
                }
            }
        }

        //private void SwitchToIds()
        //{
        //    foreach (var plant in _experiment.GetPlants())
        //    {
        //        var species = _experiment.Properties.Species.First(s => s.Name == plant.Species);
        //        plant.Species = species.Id;

        //        var population = species.Populations.First(p => p.Name == plant.Population);
        //        plant.Population = population.Id;
        //    }
        //}

        //private void InitExperimentProperties()
        //{
        //    var properties = new ExperimentProperties();

        //    for (int i = 0; i < _settings.Species.Count; i++)
        //    {
        //        var speciesName = _settings.Species[i];
        //        var species = new Species(i.ToString()) { Name = speciesName };

        //        for (int j = 0; j < _settings.Populations[speciesName].Count(); j++)
        //        {
        //            var populationName = _settings.Populations[speciesName][j];
        //            var population = new Population(j.ToString()) { Name = populationName };

        //            for (int k = 0; k < _settings.Families[populationName].Count(); k++)
        //            {
        //                var familyName = _settings.Families[populationName][k];
        //                var family = new Family(k.ToString()) { Name = familyName };

        //                population.Families.Add(family);
        //            }

        //            species.Populations.Add(population);
        //        }

        //        properties.Species.Add(species);
        //    }
        //    _experiment.Properties = properties;
        //}

        private void UpdatePlantedDates()
        {
            foreach (var plant in Experiment.Chambers[5].GetPlants())
            {
                plant.IsTransplanted = true;

                var plantedStatus = plant.PlantStatuses.FirstOrDefault(s => s.State == PlantState.Planted);
                if (plantedStatus != null)
                {
                    plantedStatus.TimeStamp = new DateTime(2013, 12, 25);
                }
            }
        }


        //private void ParseTransplanted()
        //{
        //    StringBuilder log = new StringBuilder();

        //    foreach (var plant in Experiment.GetPlants())
        //    {
        //        if (plant.Individual.EndsWith("t"))
        //        {
        //            plant.IsTransplanted = true;

        //            var newIndividual = plant.Individual.Remove(plant.Individual.Length - 1);

        //            log.AppendLine("Plant id '" + plant.Id + "' is now transplanted. Old individual: " + plant.Individual + "' => '" + newIndividual + "'.");
        //            plant.Individual = newIndividual;
        //        }
        //        else if (string.IsNullOrEmpty(plant.Notes) == false && plant.Notes.StartsWith("tp "))
        //        {
        //            plant.IsTransplanted = true;
        //            log.AppendLine("Plant id '" + plant.Id + "' is now transplanted.");
        //        }
        //    }

        //    System.IO.File.WriteAllText("transplanted_log.txt", log.ToString());
        //}

        //private void RemoveArchitecture()
        //{
        //    RemoveArchitecture(Experiment.Chambers[2].Trays[0]);
        //    RemoveArchitecture(Experiment.Chambers[2].Trays[1]);
        //    RemoveArchitecture(Experiment.Chambers[2].Trays[2]);
        //    RemoveArchitecture(Experiment.Chambers[2].Trays[5]);

        //    RemoveArchitecture(Experiment.Chambers[3].Trays[0]);
        //    RemoveArchitecture(Experiment.Chambers[3].Trays[1]);
        //    RemoveArchitecture(Experiment.Chambers[3].Trays[2]);
        //    RemoveArchitecture(Experiment.Chambers[3].Trays[5]);

        //    RemoveArchitecture(Experiment.Chambers[4].Trays[0]);
        //    RemoveArchitecture(Experiment.Chambers[4].Trays[1]);
        //    RemoveArchitecture(Experiment.Chambers[4].Trays[2]);
        //}

        //private void RemoveArchitecture(Tray tray)
        //{
        //    foreach (var plant in tray.Plants)
        //    {
        //        if (plant.PlantArchitecture != null)
        //        {
        //            if (plant.PlantArchitecture.Root != null)
        //            {
        //                if (plant.PlantArchitecture.Root.Nodes != null)
        //                {
        //                    if (plant.PlantArchitecture.Root.Nodes.Count > 0)
        //                    {
        //                        plant.PlantArchitecture.Root.Nodes.Clear();
        //                    }
        //                    else
        //                    {
        //                        //throw new Exception("Plant should have children");
        //                    }
        //                }
        //                else
        //                {
        //                    throw new Exception("Plant should have node");
        //                }
        //            }
        //            else
        //            {
        //                //throw new Exception("Plant should have root");
        //            }
        //        }
        //        else
        //        {
        //            //throw new Exception("Plant should have architecture");
        //        }

        //    }
        //}

        //private void RenameChamber6Trays()
        //{
        //    Experiment.Chambers[5].Treatment = "16HGH";

        //    foreach (var tray in Experiment.Chambers[5].Trays)
        //    {
        //        tray.Notes = tray.Notes.Replace("14H45", "16HGH");
        //    }
        //}

        //private void MigrateToNewStatuses()
        //{
        //    foreach (var plant in Experiment.GetPlants())
        //    {
        //        plant.PlantStatuses = new NotifyingCollection<StatusItem>(plant.Statuses.AllStatuses);
        //    }
        //}

        //private void UpdatePlantingDates()
        //{
        //    ChangePlantedDate(Experiment.Chambers[0], new DateTime(2013, 10, 24), new DateTime(2013, 10, 31));
        //    ChangePlantedDate(Experiment.Chambers[1], new DateTime(2013, 10, 25), new DateTime(2013, 11, 01));
        //    ChangePlantedDate(Experiment.Chambers[2], new DateTime(2013, 11, 07), new DateTime(2013, 11, 15));
        //    ChangePlantedDate(Experiment.Chambers[3], new DateTime(2013, 11, 07), new DateTime(2013, 11, 15));
        //    ChangePlantedDate(Experiment.Chambers[4], new DateTime(2013, 11, 07), new DateTime(2013, 12, 04));
        //}

        //private void ChangePlantedDate(Chamber chamber, DateTime expectedDate, DateTime newDate)
        //{
        //    foreach (var plant in chamber.GetPlants())
        //    {
        //        if (plant.PlantStatuses == null ||  plant.PlantStatuses.Count == 0)
        //        {
        //            AddInitError(plant, "PlantStatuses is null or empty");
        //            continue;
        //        }
                
        //        var plantedStatus = plant.PlantStatuses[0];

        //        if (plantedStatus.State != PlantState.Planted)
        //        {
        //            AddInitError(plant, "PlantStatuses first state was not 'Planted'");
        //        }
        //        else if (plantedStatus.TimeStamp.Date != expectedDate)
        //        {
        //            AddInitError(plant, "Unexpected Planted-date, was expecting " + expectedDate.Date);
        //        }
        //        else
        //        {
        //            plantedStatus.TimeStamp = newDate;
        //        }
        //    }
        //}

        //private void AddInitError(Plant plant, string message)
        //{
        //    plant.PlantStatuses.Add(new StatusItem { State = PlantState.InitFailed, TimeStamp = DateTime.Now });

        //    if (string.IsNullOrEmpty(plant.Notes) == false)
        //    {
        //        plant.Notes += Environment.NewLine;
        //    }

        //    plant.Notes += "Planted date update failed: " + message;
        //}

        //private void FixMeristemDates()
        //{
        //    var chamber = Experiment.Chambers[0];
        //    foreach (var plant in chamber.GetPlants())
        //    {
        //        foreach (var meristem in plant.PlantArchitecture.AllMeristems)
        //        {
        //            if (meristem.TimeStamp != DateTime.MinValue)
        //            {
        //                meristem.TimeStamp = new DateTime(2013, 11, 21);
        //            }
        //        }
        //    }

        //    chamber = Experiment.Chambers[1];
        //    foreach (var plant in chamber.GetPlants())
        //    {
        //        foreach (var meristem in plant.PlantArchitecture.AllMeristems)
        //        {
        //            if (meristem.TimeStamp != DateTime.MinValue)
        //            {
        //                meristem.TimeStamp = new DateTime(2013, 11, 23);
        //            }
        //        }
        //    }
        //}

        //private void InitializeNewArchitecture()
        //{
        //    foreach (var plant in Experiment.GetPlants())
        //    {
        //        plant.PlantArchitecture = new Architecture { Root = plant.Architecture };
        //    }
        //}

        //private void ImportNodeDump()
        //{
        //    if (System.IO.File.Exists("nodes_to_import.txt"))
        //    {
        //        StringBuilder errors = new StringBuilder();
        //        var lines = System.IO.File.ReadAllLines("nodes_to_import.txt");

        //        string error = null;
        //        foreach (var line in lines)
        //        {
        //            var split = line.Split();
        //            if (split.Length == 2)
        //            {
        //                error = UpdateNode(split[0], split[1]);
        //            }
        //            else
        //            {
        //                error = "Could not read id and node count from line '" + line + "', nodes are not updated.";
        //            }

        //            if (string.IsNullOrEmpty(error) == false)
        //            {
        //                errors.AppendLine(error);
        //            }
        //        }

        //        if (errors.Length > 0)
        //        {
        //            System.IO.File.WriteAllText("node_import_error_log.txt", errors.ToString());
        //        }
        //    }
        //}

        //private string UpdateNode(string idString, string nodeCountString)
        //{
        //    int id;
        //    int nodeCount;

        //    if (int.TryParse(idString, out id) == false)
        //    {
        //        return "Could not read id '" + idString + "', nodes are NOT updated!";
        //    }

        //    if (int.TryParse(nodeCountString, out nodeCount) == false)
        //    {
        //        return "Could not read node count '" + nodeCountString + "', nodes are NOT updated!";
        //    }

        //    var plant = Experiment.FindPlant(id);
        //    if (plant == null)
        //    {
        //        return "Could not find plant with id '" + id + "'!";
        //    }

        //    if (nodeCount > 0 && plant.PlantArchitecture.Root == null)
        //    {
        //        plant.Notes += Environment.NewLine + "Could not add imported nodes to architecture!";
        //        plant.Statuses.SetStatus(PlantState.InitFailed);
        //    }
        //    else
        //    {
        //        for (int i = 0; i < nodeCount; i++)
        //        {
        //            var nodeType = i == 0 ? NodeType.Cotyledons : NodeType.Basal;
        //            var newNode = Node.CreateNewNode(nodeType);

        //            newNode.Created = DateTime.Now.AddDays(-2);
        //            newNode.TypeUpdated = DateTime.Now.AddDays(-2);

        //            plant.PlantArchitecture.Root.Nodes.Add(newNode);
        //        }
        //    }

        //    return null;
        //}

        //private void AddStemNodeToAlivePlants()
        //{
        //    foreach (var plant in Experiment.GetPlants())
        //    {
        //        if (plant.Statuses.HasState(PlantState.Alive) && plant.Architecture == null)
        //        {
        //            plant.Architecture = new Meristem { Type = MeristemType.Stem, TimeStamp = DateTime.Now.AddDays(-3) };
        //        }
        //    }
        //}

        //private void DumpIds()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var plant in Experiment.Chambers[2].GetPlants())
        //    {
        //        sb.AppendLine(plant.Id + " ");
        //    }
        //    foreach (var plant in Experiment.Chambers[3].GetPlants())
        //    {
        //        sb.AppendLine(plant.Id + " ");
        //    }

        //    System.IO.File.WriteAllText("plant_ids_chamber_3_and_4.txt", sb.ToString());
        //}

        //private void CloneChambers()
        //{
        //    var sortedSource = new List<Plant>(Experiment.Chambers[1].GetPlants().OrderBy(p => p.Id));

        //    DoCopyChamber(sortedSource, new List<Plant>(Experiment.Chambers[2].GetPlants()), 1 * 192);
        //    DoCopyChamber(sortedSource, new List<Plant>(Experiment.Chambers[3].GetPlants()), 2 * 192);
        //    DoCopyChamber(sortedSource, new List<Plant>(Experiment.Chambers[4].GetPlants()), 3 * 192);
        //    DoCopyChamber(sortedSource, new List<Plant>(Experiment.Chambers[5].GetPlants()), 4 * 192);
        //}

        //private void DoCopyChamber(List<Plant> sourcePlants, List<Plant> targetPlants, int offset)
        //{
        //    for (int i = 0; i < sourcePlants.Count; i++)
        //    {
        //        var sourcePlant = sourcePlants[i];
        //        var targetPlant = targetPlants[i];

        //        if (sourcePlant.Id + offset != targetPlant.Id)
        //        {
        //            throw new InvalidOperationException("Unexpected plant ID");
        //        }

        //        targetPlant.Species = sourcePlant.Species;
        //        targetPlant.Population = sourcePlant.Population;
        //        targetPlant.Family = sourcePlant.Family;
        //        targetPlant.Individual = sourcePlant.Individual;

        //        targetPlant.Statuses.AllStatuses = new ObservableCollection<StatusItem>();
        //        targetPlant.Statuses.AllStatuses.Add(new StatusItem { State = PlantState.Planted, TimeStamp = new DateTime(2013, 11, 7) });
        //    }
        //}

        //    chamber = experiment.Chambers[1];
        //    foreach (var plant in chamber.GetPlants())
        //    {
        //        plant.Statuses.AllStatuses.Clear();
        //        plant.Statuses.AllStatuses.Add(new StatusItem {State = PlantState.Planted, TimeStamp = new DateTime(2013, 10, 25)});
        //    }
        //}

        //private void ConvertToIndexes(Experiment experiment, AppSettings settings)
        //{
        //    foreach (var plant in experiment.GetPlants())
        //    {
        //        var families = new List<string>(settings.Families[plant.Population]); ;
        //        var familyIndex = families.IndexOf(plant.Family.ToString());
        //        plant.Family = familyIndex;
        //    }
        //}

        #endregion
    }
}
