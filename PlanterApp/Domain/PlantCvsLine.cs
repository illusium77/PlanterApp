using PlanterApp.Applications.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanterApp.Domain
{
    internal class PlantCvsLine
    {
        private const string IsTrue = "1";
        private const string IsFalse = "0";

        // Chamber
        public string Chamber { get; set; }

        // Tray
        public string Tray { get; set; }

        // Plant
        public string Id { get; set; }
        public string Species { get; set; }
        public string Population { get; set; }
        public string Family { get; set; }
        public string Individual { get; set; }
        public string Excluded { get; set; }
        public string Transplanted { get; set; }
        public string Planted { get; set; }
        public string PlantedDate { get; set; }
        public string Alive { get; set; }
        public string AliveDate { get; set; }
        public string FPTA { get; set; }
        public string Buds { get; set; }
        public string BudsDate { get; set; }
        public string FPTB { get; set; }
        public string Flowering { get; set; }
        public string FloweringDate { get; set; }
        public string FPTF { get; set; }
        public string Seeds { get; set; }
        public string SeedsDate { get; set; }
        public string FPTS { get; set; }
        public string Dead { get; set; }
        public string DeadDate { get; set; }
        public string FPTD { get; set; }
        public string Notes { get; set; }

        [CsvCustomPropertyHeadersAttribute]
        public static string[] CustomHeaders { get; set; }

        [CsvCustomPropertyValuesAttribute]
        public string[] CustomValues { get; set; }

        public static PlantCvsLine CreateLine(string chamber, string tray, Plant plant, ExperimentProperties properties)
        {
            var croppedTray = tray.Replace(" ", string.Empty).Replace("#", string.Empty);

            var names = properties.GetNames(plant);

            if (PlantCvsLine.CustomHeaders == null && properties.PlantTraitNames != null)
            {
                PlantCvsLine.CustomHeaders = (from trait in properties.PlantTraitNames
                                              select trait.Name).ToArray();
            }

            var line = new PlantCvsLine
            {
                Id = plant.Id.ToString(),
                Species = names[ExperimentProperties.Names.Species],
                Population = names[ExperimentProperties.Names.Population],
                Family = names[ExperimentProperties.Names.Family],
                Individual = plant.Individual,
                Excluded = plant.IsExcluded ? IsTrue : IsFalse,
                Transplanted = plant.IsTransplanted ? IsTrue : IsFalse,
                Chamber = chamber,
                Tray = croppedTray,
                Planted = IsFalse,
                PlantedDate = "NA",
                Alive = IsFalse,
                AliveDate = "NA",
                FPTA = "NA",
                Buds = IsFalse,
                BudsDate = "NA",
                FPTB = "NA",
                Flowering = IsFalse,
                FloweringDate = "NA",
                FPTF = "NA",
                Seeds = IsFalse,
                SeedsDate = "NA",
                FPTS = "NA",
                Dead = IsFalse,
                DeadDate = "NA",
                FPTD = "NA",
                Notes = plant.Notes,
            };

            if (plant.PlantTraitValues != null)
            {
                line.CustomValues = (from value in plant.PlantTraitValues
                                     select value.Name).ToArray();
            }

            var dateFormat = "yyyyMMdd";
            if (plant.PlantStatuses != null && plant.PlantStatuses.Count > 0)
            {
                var planted = (from status in plant.PlantStatuses
                               where status.State == PlantState.Planted
                               select status).FirstOrDefault();

                if (planted != null)
                {
                    line.Planted = IsTrue;
                    line.PlantedDate = planted.TimeStamp.ToString(dateFormat);
                }

                foreach (var status in plant.PlantStatuses)
                {
                    switch (status.State)
                    {
                        case PlantState.Alive:
                            line.Alive = IsTrue;
                            line.AliveDate = status.TimeStamp.ToString(dateFormat);
                            line.FPTA = GetDaysFromPlanted(planted, status);
                            break;
                        case PlantState.Buds:
                            line.Buds = IsTrue;
                            line.BudsDate = status.TimeStamp.ToString(dateFormat);
                            line.FPTB = GetDaysFromPlanted(planted, status);
                            break;
                        case PlantState.Flowering:
                            line.Flowering = IsTrue;
                            line.FloweringDate = status.TimeStamp.ToString(dateFormat);
                            line.FPTF = GetDaysFromPlanted(planted, status);
                            break;
                        case PlantState.Seeds:
                            line.Seeds = IsTrue;
                            line.SeedsDate = status.TimeStamp.ToString(dateFormat);
                            line.FPTS = GetDaysFromPlanted(planted, status);
                            break;
                        case PlantState.Dead:
                            line.Dead = IsTrue;
                            line.DeadDate = status.TimeStamp.ToString(dateFormat);
                            line.FPTD = GetDaysFromPlanted(planted, status);
                            break;
                        case PlantState.Planted:
                        case PlantState.Empty:
                        case PlantState.InitFailed:
                            break;
                        default:
                            break;
                    }
                }
            }

            return line;
        }

        private static string GetDaysFromPlanted(StatusItem planted, StatusItem status)
        {
            if (planted == null || status == null)
            {
                return "NA";
            }

            return StatusItem.GetTimeDelta(planted, status).ToString();
        }

        public Plant ToPlant(ExperimentProperties experimentProperties)
        {
            Plant plant;
            int id;

            if (int.TryParse(Id, out id))
            {
                plant = new Plant(id);
            }
            else
            {
                plant = new Plant(0);
                plant.PlantStatuses.Add(new StatusItem { State = PlantState.InitFailed, TimeStamp = DateTime.Now.Date });

                return plant;
            }

            var species = experimentProperties.FindSpeciesByName(Species);
            if (species == null)
            {
                plant.PlantStatuses.Add(new StatusItem { State = PlantState.InitFailed, TimeStamp = DateTime.Now.Date });

                return plant;
            }
            plant.Species = species.Id;

            var population = species.FindPopulationByName(Population);
            if (population == null)
            {
                plant.PlantStatuses.Add(new StatusItem { State = PlantState.InitFailed, TimeStamp = DateTime.Now.Date });

                return plant;
            }
            plant.Population = population.Id;

            var family = population.FindFamilyByName(Family);
            if (family == null)
            {
                plant.PlantStatuses.Add(new StatusItem { State = PlantState.InitFailed, TimeStamp = DateTime.Now.Date });

                return plant;
            }
            plant.Family = family.Id;

            plant.Individual = Individual;
            plant.IsExcluded = Excluded == IsTrue ? true : false;
            plant.IsTransplanted = Transplanted == IsTrue ? true : false;
            plant.Notes = string.IsNullOrEmpty(Notes) ? null : Notes;

            if (Planted == PlantCvsLine.IsTrue)
            {
                TryAddState(plant, PlantState.Planted, PlantedDate);
            }

            if (Alive == PlantCvsLine.IsTrue)
            {
                TryAddState(plant, PlantState.Alive, AliveDate);
            }

            if (Buds == PlantCvsLine.IsTrue)
            {
                TryAddState(plant, PlantState.Buds, BudsDate);
            }

            if (Flowering == PlantCvsLine.IsTrue)
            {
                TryAddState(plant, PlantState.Flowering, FloweringDate);
            }

            if (Seeds == PlantCvsLine.IsTrue)
            {
                TryAddState(plant, PlantState.Seeds, SeedsDate);
            }

            if (Dead == PlantCvsLine.IsTrue)
            {
                TryAddState(plant, PlantState.Dead, DeadDate);
            }

            if (plant.PlantStatuses.Count == 0)
            {
                plant.PlantStatuses.Add(new StatusItem { State = PlantState.Empty, TimeStamp = DateTime.Now.Date });
            }

            return plant;
        }

        private void TryAddState(Plant plant, PlantState plantState, string dateString)
        {
            try
            {
                var date = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);
                plant.PlantStatuses.Add(new StatusItem { State = plantState, TimeStamp = date });
            }
            catch (Exception)
            {
            }
        }

        public static string[] GetHeaders(string propertyName)
        {
            return new[] { "A", "B" };
        }
    }
}
