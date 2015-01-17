using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Foundation;
using System.Windows.Threading;

namespace PlanterApp.Domain
{
    [DataContract]
    internal class Architecture : Model
    {
        [NonSerialized]
        private IEnumerable<Node> _allNodes;

        [NonSerialized]
        private IEnumerable<Meristem> _allMeristems;

        [NonSerialized]
        private EventHandler _refreshRequired;
        public event EventHandler RefreshRequired
        {
            add
            {
                if (_refreshRequired == null || _refreshRequired.GetInvocationList().Contains(value) == false)
                {
                    _refreshRequired += value;
                }
            }
            remove
            {
                _refreshRequired -= value;
            }
        }

        private Meristem _root;

        [DataMember]
        public Meristem Root
        {
            get { return _root; }
            set
            {
                var previous = _root;

                if (SetProperty(ref _root, value))
                {
                    if (previous != null)
                    {
                        ProcessNodeSubscription(previous.Nodes, false);
                    }

                    ProcessNodeSubscription(_root.Nodes, true);
                    RefreshNodesAndMeristems();
                }
            }
        }

        private void ProcessNodeSubscription(NotifyingCollection<Node> nodes, bool subscribe)
        {
            if (nodes == null)
            {
                return;
            }

            if (subscribe)
            {
                nodes.RefreshRequired += OnRefresh;
            }
            else
            {
                nodes.RefreshRequired -= OnRefresh;
            }

            foreach (var node in nodes)
            {
                ProcessMeristemSubscription(node.Meristems, subscribe);
            }
        }

        private void ProcessMeristemSubscription(NotifyingCollection<Meristem> meristems, bool subscribe)
        {
            if (meristems == null)
            {
                return;
            }

            meristems.RefreshRequired += OnRefresh;

            foreach (var meristem in meristems)
            {
                ProcessNodeSubscription(meristem.Nodes, subscribe);
            }
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            ProcessNodeSubscription(_root.Nodes, true);
            RefreshNodesAndMeristems();

            var handler = _refreshRequired;
            if (handler != null)
            {
                Action action = () =>
                {
                    handler(this, EventArgs.Empty);
                };
                Dispatcher.CurrentDispatcher.BeginInvoke(action);
            }
        }

        private void RefreshNodesAndMeristems()
        {
            var nodes = new List<Node>();
            var meristems = new List<Meristem>();

            if (_root != null)
            {
                nodes.AddRange(GetNodes(_root));
                meristems.AddRange(GetMeristems(_root));
            }
            _allNodes = nodes;
            _allMeristems = meristems;
        }

        private IEnumerable<Meristem> GetMeristems(Meristem meristem)
        {
            if (meristem == null || meristem.Nodes == null)
            {
                return null;
            }

            var meristems = new List<Meristem>();
            meristems.Add(meristem);

            foreach (var node in meristem.Nodes)
            {
                foreach (var m in node.Meristems)
                {
                    meristems.AddRange(GetMeristems(m));
                }
            }

            return meristems;
        }

        private IEnumerable<Node> GetNodes(Meristem meristem)
        {
            if (meristem == null || meristem.Nodes == null)
            {
                return null;
            }

            var nodes = new List<Node>();

            foreach (var node in meristem.Nodes)
            {
                nodes.Add(node);

                foreach (var m in node.Meristems)
                {
                    nodes.AddRange(GetNodes(m));
                }
            }

            return nodes;
        }

        public IEnumerable<Node> FindNodes(params NodeType[] types)
        {
            if (types == null)
            {
                return _allNodes;
            }

            return from node in _allNodes
                   where types.Contains(node.Type)
                   select node;
        }

        public IEnumerable<Meristem> FindMeristems(params MeristemType[] types)
        {
            if (types == null)
            {
                return _allMeristems;
            }

            return from meristem in _allMeristems
                   where types.Contains(meristem.Type)
                   select meristem;
        }
    }
}