using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlanterApp.Domain
{
    public enum PlantState
    {
        Empty,
        Planted,
        Alive,
        Buds,
        Flowering,
        Seeds,
        Dead,
        InitFailed
    }

    public enum TrayType
    {
        Small,  // 4x8 plants
        Medium, // 8x12 plants
        Huge    // 10x22 plants
    }

    public enum PlantLabelType
    {
        Normal,
        Population
    }

    //public enum SpeciesType
    //{
    //    ML,
    //    MG,
    //    Hyb
    //}

    //public enum PopulationType
    //{
    //    // ML
    //    DNK, HHR, OPN, PETl, SHL, SNB, TIGR, TRT, WLF, YCN,

    //    //MG
    //    PTH, SHG,

    //    // Hybrid
    //    F1, F2
    //}

    /*
    [DataContract]
    internal class ObsoleteSettings
    {
        /// <summary>
        /// Number of plant trays per chamber
        /// </summary>
        [DataMember]
        public int NumberOfTrays { get; set; }

        /// <summary>
        /// Number of plants per tray
        /// </summary>
        [DataMember]
        public int NumberOfPlants { get; set; }

        //[DataMember]
        //public ObservableCollection<string> Chambers { get; set; }

        [DataMember]
        //public ObservableCollection<SpeciesType> Species { get; set; }
        public ObservableCollection<string> Species { get; set; }

        //[DataMember]
        //public ObservableCollection<string> Populations { get; set; }
        [DataMember]
        public Dictionary<string, string[]> Populations { get; set; }
        //public Dictionary<SpeciesType, PopulationType[]> Populations { get; set; }

        [DataMember]
        public Dictionary<string, string[]> Families { get; set; }

        [DataMember]
        public ObservableCollection<string> Measurements { get; set; }

        public ObsoleteSettings()
        {
            NumberOfTrays = 6;
            NumberOfPlants = 32;

            //Chambers = new ObservableCollection<string> {
            //    "11H00",
            //    "11H45",
            //    "12H30",
            //    "13H15",
            //    "14H00",
            //    "14H45" };

            //Species = new ObservableCollection<SpeciesType> {
            //    SpeciesType.ML,
            //    SpeciesType.MG,
            //    SpeciesType.Hyb
            //};

            Species = new ObservableCollection<string> {
                "ML",
                "MG",
                "Hyb"
            };

            //Populations = new Dictionary<SpeciesType, PopulationType[]> {
            //    {SpeciesType.ML, new []{PopulationType.SNB, PopulationType.HHR, PopulationType.PETl, PopulationType.TRT, PopulationType.SHL,
            //        PopulationType.DNK, PopulationType.YCN, PopulationType.WLF, PopulationType.OPN, PopulationType.TIGR }},

            //    {SpeciesType.MG, new []{ PopulationType.PTH, PopulationType.SHG }},

            //    {SpeciesType.Hyb, new[]{ PopulationType.F1, PopulationType.F2 }}
            //};
            Populations = new Dictionary<string, string[]> {
                {"ML", new []{"SNB", "HHR", "PETl", "TRT", "SHL", "DNK", "YCN", "WLF", "OPN", "TIGR" }},
                {"MG", new []{ "PTH", "SHG" }},
                {"Hyb", new[]{ "F1", "F2" }}
            };

            // do not change the order of families, only the index is saved - not family name
            Families = new Dictionary<string, string[]> {
                {"SNB", new[]{ "1", "4", "7", "11", "12", "13", "14", "15" }},
                {"HHR", new[]{ "1", "6", "7", "9", "10", "12", "14", "17" }},
                {"PETl", new[]{ "16", "17", "18", "20", "2", "6", "7", "10" }},
                {"TRT", new[]{ "3", "5", "6", "8", "12", "13", "14", "15" }},
                {"SHL", new[]{ "17", "22", "23", "24", "3", "6", "16", "19", "8" }},
                {"DNK", new[]{ "9", "10", "11", "12", "13", "14", "16", "17" }},
                {"YCN", new[]{ "1", "2", "3", "4", "5", "6", "7", "8" }},
                {"WLF", new[]{ "74", "75", "76", "77", "63", "64", "65", "66" }},
                {"OPN", new[]{ "1", "2", "3", "4", "5", "6", "7", "8" }},
                {"TIGR", new[]{ "5", "21", "40", "47", "34", "48", "56", "59" }},
                {"PTH", new[]{ "6", "9", "12", "14", "18", "2", "8", "17" }},
                {"SHG", new[]{ "2", "3", "6", "13", "10", "11", "16" }},
                {"F1", new[]{ "L1", "G1" }},
                {"F2", new[]{ "L1", "G1", "L2", "G2" }},
            };

            Measurements = new ObservableCollection<string> {
                "Trait 1", "Trait 2", "Trait 3", "Trait 4"};

        }
        
        //public SpeciesType GetDefaultSpecies()
        //{
        //    return SpeciesType.MG;
        //}

        public string GetDefaultSpecies()
        {
            return "MG";
        }

        public string GetDefaultPopulation(string species)
        {
            return Populations[species].First();
        }

        public string GetDefaultFamily(string population)
        {
            return Families[population][0];
        }

        public string GetFamilyName(string population, int familyIndex)
        {
            return familyIndex < Families[population].Length ? Families[population][familyIndex] : GetDefaultFamily(population);
        }

        public int GetFamilyIndex(string population, string familyName)
        {
            var index = Families[population].ToList().IndexOf(familyName);
            return index < 0 ? 0 : index;
        }
    }*/
}
