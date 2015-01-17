using PlanterApp.Applications.Services;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace PlanterApp.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class ArchitectureMeristemViewModel : ArchitectureItemModelBase<IArchitectureMeristemView>, IArchitectureItemModel
    {
        private Meristem _meristem;
        public Meristem Meristem
        {
            get { return _meristem; }
            private set { SetProperty(ref _meristem, value); }
        }

        private ObservableCollection<PlanterMenuItem> _addNodeMenu;
        public ObservableCollection<PlanterMenuItem> AddNodeMenu
        {
            get { return _addNodeMenu; }
            private set { SetProperty(ref _addNodeMenu, value); }
        }

        private object _foreGround;
        public object ForeGround
        {
            get { return _foreGround; }
            set { SetProperty(ref _foreGround, value); }
        }

        [ImportingConstructor]
        public ArchitectureMeristemViewModel(IArchitectureMeristemView view, ICommandService commandService)
            : base(view, commandService)
        {
            ForeGround = Brushes.Black;
        }

        public void Initialize(Meristem meristem)
        {
            if (_meristem != null)
            {
                PropertyChangedEventManager.RemoveHandler(_meristem, OnTypeChanged, "Type");

                if (_meristem.Nodes != null)
                {
                    _meristem.Nodes.RefreshRequired -= OnNodeRefreshRequired;
                }
            }

            Meristem = meristem;
            if (_meristem != null)
            {
                PropertyChangedEventManager.AddHandler(_meristem, OnTypeChanged, "Type");

                if (meristem.Nodes != null)
                {
                    _meristem.Nodes.RefreshRequired += OnNodeRefreshRequired;
                }
            }

            _changeTypeCommand = _commandService.ChangeMeristemTypeCommand;
            UpdateChangeTypeMenu();

            UpdateAddNodeMenu();
            RaisePropertyChanged("PlanterMenuItems");
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

            if (_meristem.Type == MeristemType.Stem)
            {
                return;
            }

            foreach (MeristemType type in Enum.GetValues(typeof(MeristemType)))
            {
                if (type == _meristem.Type)
                {
                    continue;
                }

                switch (type)
                {
                    case MeristemType.TBD:
                        //ChangeTypeMenu.Add(CreateMenuItem(_changeTypeCommand, MeristemType.TBD, Key.T, this));
                        ChangeTypeMenu.Add(CreateChangeTypeMenuItem(MeristemType.TBD, Key.T));
                        break;
                    case MeristemType.Branch:
                        //ChangeTypeMenu.Add(CreateMenuItem(_changeTypeCommand, MeristemType.Branch, Key.B, this));
                        ChangeTypeMenu.Add(CreateChangeTypeMenuItem(MeristemType.Branch, Key.B));
                        break;
                    case MeristemType.Flower:
                        //ChangeTypeMenu.Add(CreateMenuItem(_changeTypeCommand, MeristemType.Flower, Key.F, this));
                        ChangeTypeMenu.Add(CreateChangeTypeMenuItem(MeristemType.Flower, Key.F));
                        break;
                    case MeristemType.Stem:
                    default:
                        break;
                }
            }
        }

        private void UpdateAddNodeMenu()
        {
            if (AddNodeMenu == null)
            {
                AddNodeMenu = new ObservableCollection<PlanterMenuItem>();
            }
            else
            {
                AddNodeMenu.Clear();
            }

            switch (_meristem.Type)
            {
                case MeristemType.Stem:
                    if (_meristem.Nodes.Count(n => n.Type == NodeType.Cotyledons) == 0)
                    {
                        AddNodeMenu.Add(CreateAddNodeMenuItem(NodeType.Cotyledons, Key.C));
                    }
                    AddNodeMenu.Add(CreateAddNodeMenuItem(NodeType.Basal, Key.B));
                    AddNodeMenu.Add(CreateAddNodeMenuItem(NodeType.Stem, Key.S));
                    AddNodeMenu.Add(CreateAddNodeMenuItem(NodeType.TBD, Key.T));
                    break;
                case MeristemType.Branch:
                    AddNodeMenu.Add(CreateAddNodeMenuItem(NodeType.Secondary, Key.S));
                    break;
                case MeristemType.TBD:
                case MeristemType.Flower:
                    break;
            }
        }

        protected PlanterMenuItem CreateAddNodeMenuItem(object targetState, Key key)
        {
            return new PlanterMenuItem(_commandService.AddNodeCommand, GetCommandParam(targetState), key, ModifierKeys.Alt);
        }

        protected override ICollection GetPlanterMenuItems()
        {
            var bindings = new List<InputBinding>();

            bindings.AddRange(AddNodeMenu);
            bindings.AddRange(ChangeTypeMenu);

            return bindings;
        }

        private void OnTypeChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateChangeTypeMenu();
            UpdateAddNodeMenu();

            RaisePropertyChanged("PlanterMenuItems");
            RaisePropertyChanged("AddCommand");
        }

        void OnNodeRefreshRequired(object sender, EventArgs e)
        {
            UpdateAddNodeMenu();
            RaisePropertyChanged("PlanterMenuItems");
        }
    }
}
