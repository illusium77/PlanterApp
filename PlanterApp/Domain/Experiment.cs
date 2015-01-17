using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace PlanterApp.Domain
{
    [DataContract]
    internal class Experiment
    {
        [DataMember]
        public ExperimentProperties Properties { get; set; }

        [DataMember]
        public ObservableCollection<Chamber> Chambers { get; set; }

        public bool HasData { get { return Chambers != null && Chambers.Count > 0; } }

        public Experiment()
        {
            Chambers = new ObservableCollection<Chamber>();
        }

        public Plant FindPlant(int id)
        {
            foreach (var chamber in Chambers)
            {
                var plant = chamber.FindPlant(id);
                if (plant != null)
                {
                    return plant;
                }
            }

            return null;
        }

        public IEnumerable<Plant> GetPlants()
        {
            var plants = new List<Plant>();

            foreach (var chamber in Chambers)
            {
                plants.AddRange(chamber.GetPlants());
            }

            return plants;
        }
    }
}
