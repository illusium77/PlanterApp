using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace PlanterApp.Domain
{
    [DataContract]
    internal class Tray
    {
        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public DateTime ShuffleTimeStamp { get; set; }

        [DataMember]
        public ObservableCollection<Plant> Plants { get; set; }

        public Tray()
        {
            Plants = new ObservableCollection<Plant>();
        }

        public void AddPlant(Plant plant)
        {
            if (FindPlant(plant.Id) != null)
            {
                throw new InvalidOperationException("Plant with id " + plant.Id + " already exists!");
            }

            Plants.Add(plant);
        }

        public bool RemovePlant(Plant plant)
        {
            return Plants.Remove(plant);
        }

        public Plant FindPlant(int id)
        {
            return Plants.FirstOrDefault(p => p.Id == id);
        }

        public void Shuffle()
        {
            // http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
            
            var random = new Random();
            var n = Plants.Count;

            while (n > 1)
            {
                n--;

                var k = random.Next(n + 1);
                if (k == n)
                {
                    continue;
                }

                Plants.Move(n, k);
                Plants.Move(k + 1, n);
            }
        }
    }
}
