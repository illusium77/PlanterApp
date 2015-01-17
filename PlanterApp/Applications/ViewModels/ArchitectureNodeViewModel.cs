using PlanterApp.Applications.Services;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace PlanterApp.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class ArchitectureNodeViewModel : ArchitectureItemModelBase<IArchitectureNodeView>, IArchitectureItemModel
    {
        private Node _node;
        public Node Node
        {
            get { return _node; }
            private set { SetProperty(ref _node, value); }
        }

        private PlanterMenuItem _removeNodeMenuItem;
        public PlanterMenuItem RemoveNodeMenuItem
        {
            get { return _removeNodeMenuItem; }
            private set { SetProperty(ref _removeNodeMenuItem, value); }
        }

        private void UpdateChangeTypeMenu()
        {
            if (ChangeTypeMenu == null)
            {
                ChangeTypeMenu = new ObservableCollection<PlanterMenuItem>();
            }
            else
            {
                ChangeTypeMenu.Clear();
            }

            foreach (NodeType type in Enum.GetValues(typeof(NodeType)))
            {
                if (type == _node.Type)
                {
                    continue;
                }

                switch (type)
                {
                    case NodeType.TBD:
                        //ChangeTypeMenu.Add(CreateMenuItem(_changeTypeCommand, NodeType.TBD, Key.T, this));
                        ChangeTypeMenu.Add(CreateChangeTypeMenuItem(NodeType.TBD, Key.T));
                        break;
                    case NodeType.Cotyledons:
                        //ChangeTypeMenu.Add(CreateMenuItem(_changeTypeCommand, NodeType.Cotyledons, Key.C, this));
                        ChangeTypeMenu.Add(CreateChangeTypeMenuItem(NodeType.Cotyledons, Key.C));
                        break;
                    case NodeType.Basal:
                        //ChangeTypeMenu.Add(CreateMenuItem(_changeTypeCommand, NodeType.Basal, Key.B, this));
                        ChangeTypeMenu.Add(CreateChangeTypeMenuItem(NodeType.Basal, Key.B));
                        break;
                    case NodeType.Stem:
                        //ChangeTypeMenu.Add(CreateMenuItem(_changeTypeCommand, NodeType.Stem, Key.M, this));
                        ChangeTypeMenu.Add(CreateChangeTypeMenuItem(NodeType.Stem, Key.M));
                        break;
                    case NodeType.Secondary:
                        //ChangeTypeMenu.Add(CreateMenuItem(_changeTypeCommand, NodeType.Secondary, Key.S, this));
                        ChangeTypeMenu.Add(CreateChangeTypeMenuItem(NodeType.Secondary, Key.S));
                        break;
                }
            }
        }

        [ImportingConstructor]
        internal ArchitectureNodeViewModel(IArchitectureNodeView view, ICommandService commandService)
            : base(view, commandService)
        {
        }

        public void Initialize(Node node)
        {
            if (_node != null)
            {
                PropertyChangedEventManager.RemoveHandler(_node, OnTypeChanged, "Type");
            }

            Node = node;
            if (_node != null)
            {
                PropertyChangedEventManager.AddHandler(_node, OnTypeChanged, "Type");
            }

            _changeTypeCommand = _commandService.ChangeNodeTypeCommand;
            UpdateChangeTypeMenu();

            RemoveNodeMenuItem = new PlanterMenuItem(_commandService.RemoveNodeCommand, GetCommandParam(null), Key.Delete, ModifierKeys.None);

            RaisePropertyChanged("PlanterMenuItems");
        }

        private void OnTypeChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateChangeTypeMenu();
            RaisePropertyChanged("PlanterMenuItems");
        }

        protected override ICollection GetPlanterMenuItems()
        {
            var bindings = new List<PlanterMenuItem>();

            bindings.AddRange(ChangeTypeMenu);
            bindings.Add(RemoveNodeMenuItem);

            return bindings;
        }

    }
}
