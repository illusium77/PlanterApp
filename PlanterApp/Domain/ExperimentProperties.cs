using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Waf.Foundation;

namespace PlanterApp.Domain
{
    [DataContract]
    internal class ExperimentProperties : Model
    {
        public enum Names
        {
            Species,
            Population,
            Family
        };

        private ObservableCollection<Species> _species;
        private ObservableCollection<TraitName> _plantTraitNames;

        //http://stackoverflow.com/questions/1062045/how-can-i-bind-an-observablecollection-to-textboxes-in-a-datatemplate
        [DataMember]
        public ObservableCollection<Species> Species
        {
            get { return _species; }
            set { SetProperty(ref _species, value); }
        }

        [DataMember]
        public ObservableCollection<TraitName> PlantTraitNames
        {
            get { return _plantTraitNames; }
            set { SetProperty(ref _plantTraitNames, value); }
        }


        public string DefaultSpecies
        {
            get
            {
                return Species == null || Species.Count == 0 ? null : Species.First().Name;
            }
        }

        public ExperimentProperties()
        {
            Species = new ObservableCollection<Species>();
        }

        public Species FindSpeciesByName(string name)
        {
            return Species != null ? Species.FirstOrDefault(s => s.Name == name) : null;
        }

        public Species FindSpeciesById(string id)
        {
            return Species != null ? Species.FirstOrDefault(s => s.Id == id) : null;
        }

        public IDictionary<Names, string> GetNames(Plant plant)
        {
            var nameDict = new Dictionary<Names, string>
            {
                { Names.Species, string.Empty },
                { Names.Population, string.Empty },
                { Names.Family, string.Empty }
            };

            if (plant != null)
            {
                var species = FindSpeciesById(plant.Species);
                if (species != null)
                {
                    nameDict[Names.Species] = species.Name;

                    var population = species.FindPopulationById(plant.Population);
                    if (population != null)
                    {
                        nameDict[Names.Population] = population.Name;

                        var family = population.FindFamilyById(plant.Family);
                        if (family != null)
                        {
                            nameDict[Names.Family] = family.Name;
                        }
                    }
                }
            }

            return nameDict;
        }
    }
}
