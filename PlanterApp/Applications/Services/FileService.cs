using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace PlanterApp.Applications.Services
{
    [Export(typeof(IFileService))]
    internal class FileService : IFileService
    {
        public Experiment LoadExperiment(string fileName)
        {
            Experiment experiment = null;

            if (File.Exists(fileName))
            {
                try
                {
                    using (var reader = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        var ser = new DataContractSerializer(typeof(Experiment));
                        experiment = (Experiment)ser.ReadObject(reader);
                    }
                }
                catch {}           
            }

            return experiment;
        }

        public Experiment ImportExperiment(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var cs = new CsvSerializer<PlantCvsLine>() { UseLineNumbers = false, Separator = ';' };
                    var extraColumns = new List<string>();
                    var csvPlants = cs.Deserialize(stream, ref extraColumns);
                    return InitializeExperimentFromCsv(csvPlants);
                }
            }

            return null ;
        }

        private Experiment InitializeExperimentFromCsv(IEnumerable<PlantCvsLine> csvPlants)
        {
            if (csvPlants == null && csvPlants.Count() == 0)
            {
                return null;
            }

            var experiment = new Experiment();  
            experiment.Properties = new ExperimentProperties();

            InitializeProperties(csvPlants, experiment.Properties);
            //InitializeTraits(traits, experiment.Properties);
            CreateChambersTraysAndPlants(csvPlants, experiment);

            return experiment;
        }

        //private void InitializeTraits(List<string> traits, ExperimentProperties experimentProperties)
        //{
        //    traits.Add("Trait1:Height");
        //    traits.Add("Trait 2: Height");
        //    traits.Add("trait3: Height");
        //    traits.Add("trait 100: height");


        //    if (traits == null || traits.Count == 0)
        //    {
        //        return;
        //    }

        //    foreach (var trait in traits)
        //    {
        //        if (string.IsNullOrEmpty(trait))
        //        {
        //            continue;
        //        }

        //        // Trait1:Height
        //        // Trait 2: Height
        //        // trait3: Height
        //        // trait 100: Height
        //        // ([A-Za-z]+)|(\d+)|(:)

        //        var reg = Regex.Matches(trait, @"([A-Za-z]+)|(\d+)|(:)");
        //        if (reg.Count == 4)
        //        {
        //        }
        //    }

        //}

        private void CreateChambersTraysAndPlants(IEnumerable<PlantCvsLine> csvPlants, Experiment experiment)
        {
            foreach (var csvPlant in csvPlants)
            {
                var chamber = experiment.Chambers.FirstOrDefault(c => c.Treatment == csvPlant.Chamber);
                if (chamber == null)
                {
                    chamber = new Chamber { Treatment = csvPlant.Chamber };
                    experiment.Chambers.Add(chamber);
                }

                var tray = chamber.Trays.FirstOrDefault(t => t.Notes == csvPlant.Tray);
                if (tray == null)
                {
                    tray = new Tray { Notes = csvPlant.Tray };
                    chamber.Trays.Add(tray);
                }

                tray.Plants.Add(csvPlant.ToPlant(experiment.Properties));
            }
        }

        private void InitializeProperties(IEnumerable<PlantCvsLine> csvPlants, ExperimentProperties experimentProperties)
        {
            foreach (var csvPlant in csvPlants)
            {
                var species = experimentProperties.FindSpeciesByName(csvPlant.Species);
                if (species == null)
                {
                    species = new Species(experimentProperties.Species.Count.ToString()) { Name = csvPlant.Species };
                    experimentProperties.Species.Add(species);
                }

                var population = species.FindPopulationByName(csvPlant.Population);
                if (population == null)
                {
                    population = new Population(species.Populations.Count.ToString()) { Name = csvPlant.Population };
                    species.Populations.Add(population);
                }

                var family = population.FindFamilyByName(csvPlant.Family);
                if (family == null)
                {
                    family = new Family(population.Families.Count.ToString()) { Name = csvPlant.Family };
                    population.Families.Add(family);
                }
            }
        }

        private void InitializeChamberAndTray(PlantCvsLine csvPlant, Experiment experiment)
        {
            var chamber = experiment.Chambers.FirstOrDefault(c => c.Treatment == csvPlant.Chamber);
            if (chamber == null)
            {
                chamber = new Chamber { Treatment = csvPlant.Chamber };
                experiment.Chambers.Add(chamber);
            }

            var tray = chamber.Trays.FirstOrDefault(t => t.Notes == csvPlant.Tray);
            if (tray == null)
            {
                tray = new Tray { Notes = csvPlant.Tray };
                chamber.Trays.Add(tray);
            }
        }

        public bool SaveExperiment(string fileName, Experiment experiment)
        {
            using (var writer = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                var ser = new DataContractSerializer(typeof(Experiment));
                ser.WriteObject(writer, experiment);
            }
            return true;
        }

        public bool ExportExperiment(string fileName, Experiment experiment)
        {
            var csvLines = GetLines(experiment);
            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                var cs = new CsvSerializer<PlantCvsLine> { UseLineNumbers = false, Separator = ';'};
                cs.Serialize(stream, csvLines);
            }

            return true;
        }

        private IEnumerable<PlantCvsLine> GetLines(Experiment experiment)
        {
            var lines = new List<PlantCvsLine>();

            foreach (var chamber in experiment.Chambers)
            {
                var chamberName = chamber.Treatment;

                foreach (var tray in chamber.Trays)
                {
                    var trayName = tray.Notes;

                    foreach (var plant in tray.Plants)
                    {
                        lines.Add(PlantCvsLine.CreateLine(
                            chamberName, trayName, plant, experiment.Properties));
                    }
                }
            }

            return lines;
        }

        public void Log(string logLine)
        {
            int tryCounter = 10;
            while (tryCounter > 0)
            {
                try
                {
                    using (StreamWriter outfile = new StreamWriter("planter_log.txt", true))
                    {
                        // "dd/MM/yyyy hh:mm:ss"
                        outfile.WriteLineAsync(DateTime.Now.ToString() + " " + logLine);
                    }

                    tryCounter = 0;
                }
                catch (Exception)
                {
                    tryCounter--;

                    if (tryCounter == 0)
                    {
                        throw;
                    }
                    else
                    {
                        Thread.Sleep(50);
                    }
                }
            }
        }

        public void Log(IList<string> loglines)
        {
            foreach (var line in loglines)
            {
                Log(line);
            }
        }

        //public Experiment InitializeExperiment()
        //{
        //    var experiment = new Experiment();

        //    int plantCounter = 0;
        //    foreach (var chamberName in AppSetting.Chambers)
        //    {
        //        var chamber = new Chamber { Treatment = chamberName };

        //        for (int i = 0; i < AppSetting.NumberOfTrays; i++)
        //        {
        //            var tray = new Tray { Notes = chamberName + " #" + (i + 1) };

        //            for (int j = 0; j < AppSetting.NumberOfPlants; j++)
        //            {
        //                plantCounter++;

        //                var plant = new Plant(plantCounter);
        //                plant.SetDefaults(AppSetting);

        //                tray.Plants.Add(plant);
        //            }

        //            chamber.AddTray(tray);
        //        }

        //        experiment.Chambers.Add(chamber);
        //    }

        //    ImportFromCsv(experiment, "plants_23-10-13.csv");

        //    return experiment;
        //}

        //private void ImportFromCsv(Experiment experiment, string csvFileName)
        //{
        //    if (File.Exists(csvFileName) == false)
        //    {
        //        return;
        //    }
            
        //    IList<CsvImportLine> data = null;
        //    using (var stream = new FileStream(csvFileName, FileMode.Open, FileAccess.Read))
        //    {
        //        var cs = new CsvSerializer<CsvImportLine> { Separator = ';', UseLineNumbers = false};
        //        data = cs.Deserialize(stream);
        //    }

        //    if (data != null)
        //    {
        //        foreach (var csvLine in data)
        //        {
        //            int id;

        //            if (int.TryParse(csvLine.Id, out id))
        //            {
        //                var plant = experiment.FindPlant(id);

        //                if (plant != null)
        //                {
        //                    MapToPlant(csvLine, plant);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void MapToPlant(CsvImportLine csvLine, Plant plant)
        //{
        //    // TODO:
        //    bool mappingOk = true;

        //    if (string.IsNullOrEmpty(csvLine.Species) == false && AppSetting.Species.Contains(csvLine.Species.ToUpper()))
        //    {
        //        plant.Species = int.Parse(csvLine.Species);
        //    }
        //    else
        //    {
        //        mappingOk = false;
        //    }

        //    if (mappingOk && string.IsNullOrEmpty(csvLine.Population) == false && AppSetting.Populations[plant.Species].Contains(csvLine.Population))
        //    {
        //        plant.Population = csvLine.Population;
        //    }
        //    else
        //    {
        //        mappingOk = false;
        //    }

        //    var familyIndex = GetLastNumber(csvLine.Family) - 1;
        //    if (familyIndex >= 0 && familyIndex < AppSetting.Families[plant.Population].Length)
        //    {
        //        plant.Family = AppSetting.Families[plant.Population][familyIndex];
        //    }
        //    else
        //    {
        //        mappingOk = false;
        //    }

        //    var individual = GetLastNumber(csvLine.Individual);
        //    if (individual >= 0 && individual < 50)
        //    {
        //        plant.Individual = individual.ToString();
        //    }
        //    else
        //    {
        //        mappingOk = false;
        //    }


        //    if (mappingOk == false)
        //    {
        //        plant.Statuses.SetStatus(PlantState.InitFailed);
        //    }
        //    else
        //    {
        //        plant.Statuses.SetStatus(PlantState.Planted);
        //    }

        //}

        //private int GetLastNumber(string number)
        //{
        //    var num = number.Substring(number.LastIndexOf('-') + 1);
        //    int n;
        //    if (int.TryParse(num, out n))
        //    {
        //        return n;
        //    }

        //    return -1;
        //}

        //private IList<Chamber> InitWithTestData()
        //{
        //    int plantIndex = 1;

        //    var chambers = new ObservableCollection<Chamber> 
        //    {
        //        GetTestChamber("11H00", ref plantIndex),
        //        GetTestChamber("11H45", ref plantIndex),
        //        GetTestChamber("12H30", ref plantIndex),
        //        GetTestChamber("13H15", ref plantIndex),
        //        GetTestChamber("14H00", ref plantIndex),
        //        GetTestChamber("14H45", ref plantIndex)
        //    };

        //    return chambers;
        //}

        //private static Chamber GetTestChamber(string treatment, ref int plantIndex)
        //{
        //    var testChamber = new Chamber { Treatment = treatment };

        //    for (int i = 0; i < 6; i++)
        //    {
        //        var testTray = GetTestTray(ref plantIndex);
        //        testChamber.AddTray(testTray);
        //    }

        //    return testChamber;
        //}

        //private static Tray GetTestTray(ref int plantIndex)
        //{

        //    var testTray = new Tray { Notes = "Tray #" + (plantIndex / 32 + 1) };
        //    for (int i = 0; i < 32; i++)
        //    {
        //        var plant = new Plant(plantIndex);
        //        InitTestPlant(plant);

        //        testTray.AddPlant(plant);

        //        plantIndex += 1;
        //    }

        //    return testTray;
        //}

        //private static int PlantStateTracker = 0;
        //private static Array PlantStates = Enum.GetValues(typeof(PlantState));

        //private static int SpeciesTracker = 0;
        //private static ObservableCollection<string> Species = new AppSettings().Species;

        //private static int PopulationTracker= 0;
        //private static ObservableCollection<string> Populations = new AppSettings().Populations;

        //private static int FamilyTracker = 0;
        //private static ObservableCollection<string> Families = new AppSettings().Families;

        //private static void InitTestPlant(Plant plant)
        //{
        //    if (PlantStateTracker >= PlantStates.Length) PlantStateTracker = 0;

        //    if (SpeciesTracker >= Species.Count) SpeciesTracker = 0;
        //    if (PopulationTracker >= Populations.Count) PopulationTracker = 0;
        //    if (FamilyTracker >= Families.Count) FamilyTracker = 0;

        //    var state = (PlantState)PlantStateTracker;
        //    if (state != plant.Statuses.CurrentState)
        //    {
        //        plant.Statuses.SetStatus(state);
        //    }

        //    plant.Species = Species[SpeciesTracker];
        //    plant.Population = Populations[SpeciesTracker];
        //    plant.Family = Families[SpeciesTracker];

        //    PlantStateTracker++;
        //    SpeciesTracker++;
        //    PopulationTracker++;
        //    FamilyTracker++;
        //}

    }
}

