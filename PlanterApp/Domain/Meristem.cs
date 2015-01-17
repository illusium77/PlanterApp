using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Waf.Foundation;

namespace PlanterApp.Domain
{
    public enum MeristemType
    {
        TBD,
        Stem,
        Branch,
        Flower
    }

    [DataContract]
    internal class Meristem : Model
    {
        [DataMember]
        private MeristemType _type;
        public MeristemType Type
        {
            get { return _type; }
            set
            {
                if (SetProperty(ref _type, value))
                {
                    RaisePropertyChanged("CanHaveChildNode");
                    RaisePropertyChanged("Name");
                    
                    TimeStamp = DateTime.Now;
                }
            }
        }

        [DataMember]
        private DateTime _timeStamp;
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { SetProperty(ref _timeStamp, value); }
        }

        [DataMember]
        private NotifyingCollection<Node> _nodes;
        public NotifyingCollection<Node> Nodes
        {
            get { return _nodes; }
            set { SetProperty(ref _nodes, value); }
        }

        public bool CanHaveChildNode
        {
            get
            {
                return _type == MeristemType.Branch || _type == MeristemType.Stem;
            }
        }

        public Meristem()
        {
            _type = MeristemType.TBD;
            InitializeNodes();
        }

        //[OnDeserializing]
        //private void OnDeserializing(StreamingContext context)
        //{
        //    InitializeNodes();
        //}

        private void InitializeNodes()
        {
            if (_nodes == null)
            {
                _nodes = new NotifyingCollection<Node>();
            }
        }
    }

}
