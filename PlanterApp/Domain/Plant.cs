using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Waf.Foundation;

namespace PlanterApp.Domain
{
    [DataContract]
    internal class Plant : Model
    {
        private int _id;
        private string _species;
        //private SpeciesType _species;
        private string _population;
        //private PopulationType _population;
        private string _family;
        private string _individual;
        private string _notes;
        //private PlantStatusHistory _statuses;
        //private PlantMeasurements _measurements;
        private Architecture _plantArchitecture;
        private bool _isExcluded;
        private bool _isTransplanted;
        private NotifyingCollection<StatusItem> _plantStatuses;
        private ObservableCollection<TraitValue> _plantTraitValues;

        [DataMember]
        public int Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        //[DataMember]
        //public SpeciesType Species
        //{
        //    get { return _species; }
        //    set { SetProperty(ref _species, value); }
        //}
        [DataMember]
        public string Species
        {
            get { return _species; }
            set { SetProperty(ref _species, value); }
        }

        [DataMember]
        public string Population
        {
            get { return _population; }
            set { SetProperty(ref _population, value); }
        }

        [DataMember]
        public string Family
        {
            get { return _family; }
            set { SetProperty(ref _family, value); }
        }

        [DataMember]
        public string Individual
        {
            get { return _individual; }
            set { SetProperty(ref _individual, value); }
        }

        [DataMember]
        public string Notes
        {
            get { return _notes; }
            set { SetProperty(ref _notes, value); }
        }

        [DataMember]
        public bool IsExcluded
        {
            get { return _isExcluded; }
            set { SetProperty(ref _isExcluded, value); }
        }

        [DataMember]
        public bool IsTransplanted
        {
            get { return _isTransplanted; }
            set { SetProperty(ref _isTransplanted, value); }
        }

        //[DataMember]
        //public PlantStatusHistory Statuses
        //{
        //    get { return _statuses; }
        //    set { SetProperty(ref _statuses, value); }
        //}

        [DataMember]
        public NotifyingCollection<StatusItem> PlantStatuses
        {
            get { return _plantStatuses; }
            set { SetProperty(ref _plantStatuses, value); }
        }

        //[DataMember]
        //public PlantMeasurements Measurements
        //{
        //    get { return _measurements; }
        //    set { SetProperty(ref _measurements, value); }
        //}

        [DataMember]
        public Architecture PlantArchitecture
        {
            get { return _plantArchitecture; }
            set { SetProperty(ref _plantArchitecture, value); }
        }

        [DataMember]
        public ObservableCollection<TraitValue> PlantTraitValues
        {
            get { return _plantTraitValues; }
            set { SetProperty(ref _plantTraitValues, value); }
        }

        public Plant(int id)
        {
            Id = id;

            //Statuses = new PlantStatusHistory();
            //Measurements = new PlantMeasurements();
            PlantStatuses = new NotifyingCollection<StatusItem>();
        }

        public void InitializeTraitValues(IEnumerable<TraitName> traits)
        {
            if (traits == null)
            {
                return;
            }

            if (_plantTraitValues == null)
            {
                PlantTraitValues = new ObservableCollection<TraitValue>();
            }
            _plantTraitValues.Add(new TraitValue("2") { Name = string.Empty });

            foreach (var trait in traits)
            {
                var existingValue = from value in _plantTraitValues
                                    where value.Id == trait.Id
                                    select value;


                if (existingValue.Count() == 0)
                {
                    _plantTraitValues.Add(new TraitValue(trait.Id) { Name = string.Empty });
                }
            }
        }
    }
}
