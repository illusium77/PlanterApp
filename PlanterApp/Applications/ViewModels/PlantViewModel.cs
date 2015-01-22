using PlanterApp.Applications.Services;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;


namespace PlanterApp.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class PlantViewModel : ViewModel<IPlantView>
    {
        public event EventHandler ArchitectureUpdated;

        private PlantLabelType _currentLabel;

        private Plant _plant;
        public Plant Plant
        {
            get { return _plant; }
            set
            {
                if (SetProperty(ref _plant, value) && _plant != null)
                {
                    PropertyChangedEventManager.AddHandler(_plant, OnPlantPropertyChanged, string.Empty);

                    SubscribeToPlantStatuses();
                    SubscribeToPlantArchitecture();

                    RefreshLabel();
                    RefreshToolTip();
                }
            }
        }

        private void OnPlantPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PlantStatuses")
            {
                SubscribeToPlantStatuses();
            }
            else if (e.PropertyName == "PlantArchitecture")
            {
                SubscribeToPlantArchitecture();
            }
            else if (e.PropertyName == "IsExcluded")
            {
                RefreshArchitecture();
                RefreshToolTip();
            }
            else if (e.PropertyName == "IsTransplanted")
            {
                RefreshArchitecture();
                RefreshToolTip();
            }
            else if (e.PropertyName == "Id"
                || e.PropertyName == "Species"
                || e.PropertyName == "Population"
                || e.PropertyName == "Family"
                || e.PropertyName == "Individual")
            {
                RefreshLabel();
                RefreshToolTip();
            }
            else if (e.PropertyName == "Notes")
            {
                RefreshToolTip();
            }
        }

        private void SubscribeToPlantStatuses()
        {
            if (_plant.PlantStatuses != null)
            {
                _plantStatuses = CollectionViewSource.GetDefaultView(_plant.PlantStatuses);
                _plantStatuses.Filter = (o) =>
                {
                    if (_experimentService.IsTimeMachineOn == false)
                    {
                        return true;
                    }

                    var item = o as StatusItem;
                    return item != null ? item.TimeStamp.Date <= CurrentDate.Date : false;
                };

                _plant.PlantStatuses.RefreshRequired += OnPlantStatusesUpdated;
            }

            RefreshStatus();
        }

        private void OnPlantStatusesUpdated(object sender, EventArgs e)
        {
            _plant.PlantStatuses.RefreshRequired -= OnPlantStatusesUpdated;
            _plant.PlantStatuses.Sort(item => item.TimeStamp);
            _plant.PlantStatuses.RefreshRequired += OnPlantStatusesUpdated;

            RefreshStatus();

            if (HasState(PlantState.Alive) && HasArchitecture == false)
            {
                Plant.PlantArchitecture = new Architecture { Root = new Meristem { Type = MeristemType.Stem } };
                RefreshArchitecture();
            }
            else if (HasState(PlantState.Alive) == false)
            {
                Plant.PlantArchitecture = null;
                RefreshArchitecture();
            }
        }

        private void RefreshStatus()
        {
            if (_plantStatuses != null)
            {
                var status = _plantStatuses.OfType<StatusItem>().LastOrDefault();
                CurrentState = status != null ? status.State : PlantState.InitFailed;
            }

            //RaisePropertyChanged("AvailableStates");
            RefreshChangeStateMenu();
        }

        private void SubscribeToPlantArchitecture()
        {
            if (_plant.PlantArchitecture != null)
            {
                _plant.PlantArchitecture.RefreshRequired += (s, o) =>
                    {
                        RefreshArchitecture();
                    };
            }

            RefreshArchitecture();
        }

        private void RefreshArchitecture()
        {
            HasArchitecture = _plant.IsExcluded == false
                && _plant.PlantArchitecture != null
                && _plant.PlantArchitecture.Root != null
                && _plant.PlantArchitecture.Root.TimeStamp.Date <= _currentDate.Date;

            var handler = ArchitectureUpdated;
            if (handler != null)
            {
                ArchitectureUpdated(this, EventArgs.Empty);
            }
        }

        private ICollectionView _plantStatuses;
        public ICollectionView PlantStatuses
        {
            get { return _plantStatuses; }
        }

        //private StatusItem _currentStatus;
        //public StatusItem CurrentStatus
        //{
        //    get { return _currentStatus; }
        //    set
        //    {
        //        SetProperty(ref _currentStatus, value);
        //        RefreshToolTip();
        //    }
        //}

        private PlantState _currentState;
        public PlantState CurrentState
        {
            get { return _currentState; }
            set
            {
                SetProperty(ref _currentState, value);
                RefreshToolTip();
            }
        }

        private bool _hasArchitecture;
        public bool HasArchitecture
        {
            get { return _hasArchitecture; }
            set { SetProperty(ref _hasArchitecture, value); }
        }

        private DateTime _currentDate;
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                if (SetProperty(ref _currentDate, value))
                {
                    _plantStatuses.Refresh();

                    RefreshStatus();
                    RefreshArchitecture();
                }
            }
        }

        private ObservableCollection<PlanterMenuItem> _changeStateMenu;
        public ObservableCollection<PlanterMenuItem> ChangeStateMenu
        {
            get { return _changeStateMenu; }
            private set { SetProperty(ref _changeStateMenu, value); }
        }

        private void RefreshChangeStateMenu()
        {
            if (ChangeStateMenu == null)
            {
                ChangeStateMenu = new ObservableCollection<PlanterMenuItem>();
            }
            else
            {
                ChangeStateMenu.Clear();
            }

            var allStates = Enum.GetValues(typeof(PlantState)).Cast<PlantState>();
            var existingStates = from item in _plantStatuses.OfType<StatusItem>()
                                 select item.State;
            var availableStates = from state in allStates
                                  where existingStates.Contains(state) == false && state != PlantState.InitFailed
                                  select state;

            foreach (var state in availableStates)
            {
                var param = new PlanterMenuCommandParam { Source = this, TargetState = state };
                ChangeStateMenu.Add(new PlanterMenuItem(_commandService.PlantStatusUpdateCommand, param));
            }
        }

        private IExperimentService _experimentService;
        public IExperimentService ExperimentService
        {
            get { return _experimentService; }
            set { SetProperty(ref _experimentService, value); }
        }

        private ICommandService _commandService;
        public ICommandService CommandService
        {
            get { return _commandService; }
            set { SetProperty(ref _commandService, value); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private string _label;
        public string Label
        {
            get { return _label; }
            set { SetProperty(ref _label, value); }
        }

        private string _toolTip;
        public string ToolTip
        {
            get { return _toolTip; }
            set { SetProperty(ref _toolTip, value); }
        }

        private TrayViewModel _parentTray;
        public TrayViewModel ParentTray
        {
            get { return _parentTray; }
            set { SetProperty(ref _parentTray, value); }
        }

        [ImportingConstructor]
        public PlantViewModel(IPlantView view, IExperimentService experimentService, ICommandService commandService)
            : base(view)
        {
            _experimentService = experimentService;
            _commandService = commandService;

            _currentDate = DateTime.Now.Date;
            _currentLabel = PlantLabelType.Normal;
        }

        private void RefreshLabel()
        {
            if (_plant == null)
            {
                return;
            }

            var names = _experimentService.Experiment.Properties.GetNames(_plant);

            switch (_currentLabel)
            {
                case PlantLabelType.Population:
                    Label = names[ExperimentProperties.Names.Population];
                    break;
                case PlantLabelType.Normal:
                default:
                    Label = _plant.Id + " " + names[ExperimentProperties.Names.Species]
                        + " " + names[ExperimentProperties.Names.Population]
                        + " " + names[ExperimentProperties.Names.Family]
                        + " " + _plant.Individual;
                    break;
            }
        }

        private void RefreshToolTip()
        {
            if (_plant == null)
            {
                return;
            }

            var builder = new StringBuilder();

            builder.AppendLine("Plant id: " + _plant.Id);
            builder.AppendLine("Current state: " + CurrentState);

            var names = _experimentService.Experiment.Properties.GetNames(_plant);

            builder.AppendLine("Species: " + names[ExperimentProperties.Names.Species]);
            builder.AppendLine("Population: " + names[ExperimentProperties.Names.Population]);
            builder.AppendLine("Family: " + names[ExperimentProperties.Names.Family]);
            builder.Append("Individual: " + _plant.Individual);

            if (string.IsNullOrEmpty(_plant.Notes) == false)
            {
                builder.Append(Environment.NewLine + "Notes: " + "\"" + _plant.Notes + "\"");
            }

            if (_plant.IsExcluded)
            {
                builder.Append(Environment.NewLine + Environment.NewLine + "Plant is excluded from statistic calculations.");
            }

            if (_plant.IsTransplanted)
            {
                builder.Append(Environment.NewLine + "Plant has been transplanted.");
            }

            ToolTip = builder.ToString();
        }

        public bool HasState(PlantState state)
        {
            return _plantStatuses != null && _plantStatuses.OfType<StatusItem>().Any(status => status.State == state);
        }

        public int GetDays(PlantState startState, PlantState endState)
        {
            if (HasState(startState) == false || HasState(endState) == false)
            {
                return 0;
            }

            var delta = StatusItem.GetTimeDelta(GetItem(startState), GetItem(endState));

            if (delta < 0)
            {
                delta = 0;
            }

            return delta;
        }

        //public void AddStatus(PlantState state)
        //{
        //    var statusItem = _plant.PlantStatuses.FirstOrDefault(item => item.State == state);

        //    if (statusItem == null)
        //    {
        //        statusItem = new StatusItem { State = state, TimeStamp = DateTime.Now };
        //        _plant.PlantStatuses.Add(statusItem);
        //    }
        //    else
        //    {
        //        statusItem.TimeStamp = DateTime.Now;
        //    }
        //}


        internal void AddStatus(StatusItem newItem)
        {
            if (newItem == null)
            {
                return;
            };

            var statusItem = _plant.PlantStatuses.FirstOrDefault(i => i.State == newItem.State);
            if (statusItem != null)
            {
                _plant.PlantStatuses.Remove(statusItem);
            }

            _plant.PlantStatuses.Add(newItem);

            var emptyStates = (from s in _plant.PlantStatuses
                              where s.State == PlantState.Empty
                              select s).ToList();
            if (newItem.State != PlantState.Empty && emptyStates.Any())
            {
                foreach (var e in emptyStates)
                {
                    _plant.PlantStatuses.Remove(e);
                }
            }

            //if (statusItem == null)
            //{
            //    statusItem = new StatusItem { State = item.State, TimeStamp = item.TimeStamp };
            //    _plant.PlantStatuses.Add(statusItem);
            //}
            //else
            //{
            //    statusItem.TimeStamp = item.TimeStamp;
            //}
        }

        private StatusItem GetItem(PlantState state)
        {
            return _plantStatuses.OfType<StatusItem>().First(item => item.State == state);
        }

        internal void Reset()
        {
        }

        internal void SetLabelType(PlantLabelType plantLabelType)
        {
            _currentLabel = plantLabelType;
            RefreshLabel();
        }
    }
}
