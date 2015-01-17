using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PlanterApp.Domain
{
    [DataContract]
    internal class Chamber
    {
        [DataMember]
        public string Treatment { get; set; }

        [DataMember]
        public ObservableCollection<Tray> Trays { get; set; }

        public Chamber()
        {
            Trays = new ObservableCollection<Tray>();
        }

        public void AddTray(Tray tray)
        {
            Trays.Add(tray);
        }

        public void RemoveTray(Tray tray)
        {
            Trays.Add(tray);
        }

        public Plant FindPlant(int id)
        {
            foreach (var tray in Trays)
            {
                var plant = tray.FindPlant(id);
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

            foreach (var tray in Trays)
            {
                plants.AddRange(tray.Plants);
            }

            return plants;
        }
    }
}
