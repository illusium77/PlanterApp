using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Waf.Foundation;

namespace PlanterApp.Domain
{

    [DataContract]
    internal abstract class ExperimentProperty : Model
    {
        [DataMember]
        private string _id;

        [DataMember]
        private string _name;

        public string Id
        {
            get { return _id; }
            private set { SetProperty(ref _id, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public ExperimentProperty(string id)
        {
            _id = id;
        }
    }

    [DataContract]
    internal class TraitName : ExperimentProperty
    {
        public TraitName(string id)
            : base(id)
        {
        }
    }

    [DataContract]
    internal class TraitValue : ExperimentProperty
    {
        public TraitValue(string id)
            : base(id)
        {
        }
    }

    [DataContract]
    internal class Species : ExperimentProperty
    {
        [DataMember]
        public ObservableCollection<Population> Populations { get; set; }

        public Species(string id)
            : base(id)
        {
            Populations = new ObservableCollection<Population>();
        }

        public Population FindPopulationByName(string name)
        {
            return Populations != null ? Populations.FirstOrDefault(p => p.Name == name) : null;
        }

        public Population FindPopulationById(string id)
        {
            return Populations != null ? Populations.FirstOrDefault(p => p.Id == id) : null;
        }
    }

    [DataContract]
    internal class Population : ExperimentProperty
    {
        [DataMember]
        public ObservableCollection<Family> Families { get; set; }

        public Population(string id)
            : base(id)
        {
            Families = new ObservableCollection<Family>();
        }

        public Family FindFamilyByName(string name)
        {
            return Families != null ? Families.FirstOrDefault(f => f.Name == name) : null;
        }

        public Family FindFamilyById(string id)
        {
            return Families != null ? Families.FirstOrDefault(f => f.Id == id) : null;
        }
    }

    [DataContract]
    internal class Family : ExperimentProperty
    {
        public Family(string id)
            : base(id)
        {
        }
    }
}
