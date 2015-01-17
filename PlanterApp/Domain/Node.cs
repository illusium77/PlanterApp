using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Waf.Foundation;

namespace PlanterApp.Domain
{
    internal enum NodeType
    {
        TBD,
        Cotyledons, // First node in plant
        Basal, // Next to ground
        Stem,
        Secondary // in branches
    }

    [DataContract]
    internal class Node : Model
    {
        [DataMember]
        private NodeType _type;
        public NodeType Type
        {
            get { return _type; }
            set
            {
                if (SetProperty(ref _type, value))
                {
                    TypeUpdated = DateTime.Now;
                }
            }
        }

        [DataMember]
        private DateTime _created;
        public DateTime Created
        {
            get { return _created; }
            set { SetProperty(ref _created, value); }
        }

        [DataMember]
        private DateTime _typeUpdated;
        public DateTime TypeUpdated
        {
            get { return _typeUpdated; }
            set { SetProperty(ref _typeUpdated, value); }
        }

        [DataMember]
        private NotifyingCollection<Meristem> _meristems;
        public NotifyingCollection<Meristem> Meristems
        {
            get { return _meristems; }
            set { SetProperty(ref _meristems, value); }
        }

        public Meristem MeristemA { get { return _meristems[0]; } }
        public Meristem MeristemB { get { return _meristems[1]; } }

        private Node()
        {
            Created = DateTime.Now;
            InitializeMeristems();
        }

        private void InitializeMeristems()
        {
            if (_meristems == null)
            {
                _meristems = new NotifyingCollection<Meristem>
                {
                    new Meristem { Type = MeristemType.TBD },
                    new Meristem { Type = MeristemType.TBD }
                };
            }
        }

        public static Node CreateNewNode(NodeType type)
        {
            return new Node { Type = type };
        }
    }
}
