using PlanterApp.Applications.Services;
using PlanterApp.Applications.ViewModels;
using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Media;

namespace PlanterApp.Applications.Controllers
{
    [Export]
    internal class TraitController : IController
    {
        private readonly IExperimentService _experimentService;
        private readonly ICommandService _commandService;
        private readonly IViewService _viewService;

        private readonly ExportFactory<ArchitectureMeristemViewModel> _meristemModelFactory;
        private readonly ExportFactory<ArchitectureNodeViewModel> _nodeViewFactory;

        //private readonly TraitViewModel _traitViewModel;
        //private readonly PlantPropertyViewModel _plantPropertyModel;
        //private readonly ArchitectureViewModel _architectureViewModel;

        private PlantViewModel SelectedPlant
        {
            get { return _experimentService.SelectedPlant == null ? null : _experimentService.SelectedPlant as PlantViewModel; }
            set { _experimentService.SelectedPlant = value; }
        }

        //public object PlantPropertyView { get { return _plantPropertyModel != null ? _plantPropertyModel.View : null; } }
        //public object TraitView { get { return _traitViewModel != null ? _traitViewModel.View : null; } }

        //private PlantViewModel _selectedPlant;

        [ImportingConstructor]
        public TraitController(
            IExperimentService experimentService,
            ICommandService commandService,
            IViewService viewService,
            TraitViewModel traitViewModel, 
            PlantPropertyViewModel plantPropertyViewModel,
            ArchitectureViewModel architectureViewModel,
            ExportFactory<ArchitectureMeristemViewModel> meristemModelFactory,
            ExportFactory<ArchitectureNodeViewModel> nodeViewFactory)
        {
            _experimentService = experimentService;
            _commandService = commandService;
            _viewService = viewService;

            //_traitViewModel = traitViewModel;
            //_plantPropertyModel = plantPropertyViewModel;
            //_architectureViewModel = architectureViewModel;

            _meristemModelFactory = meristemModelFactory;
            _nodeViewFactory = nodeViewFactory;

            _commandService.AddNodeCommand = new DelegateCommand(OnAddNode);
            _commandService.ChangeMeristemTypeCommand = new DelegateCommand(OnChangeMeristemType);
            _commandService.RemoveNodeCommand = new DelegateCommand(OnRemoveNode);
            _commandService.ChangeNodeTypeCommand = new DelegateCommand(OnChangeNodeType);

            _viewService.PlantPropertyView = plantPropertyViewModel.View;
            _viewService.TraitView = traitViewModel.View;
            traitViewModel.ArchitectureView = architectureViewModel.View;
        }

        public void Initialize()
        {
            //_viewService.PlantPropertyView = _plantPropertyModel.View;
            //_viewService.TraitView = _traitViewModel.View;

            //_traitViewModel.ArchitectureView = _architectureViewModel.View;

            PropertyChangedEventManager.AddHandler(_experimentService, OnExperimentServicePropertyChanged, string.Empty);
        }

        private void OnExperimentServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedPlant")
            {
                Unsubscribe();
                Subscribe();
                UpdateArchitecture();
            }
        }

        //private void OnTimeMachineStatusChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    _traitViewModel.IsEnabled = !_experimentService.IsTimeMachineOn;
        //    _plantPropertyModel.IsEnabled = !_experimentService.IsTimeMachineOn;
        //}

        //internal void SelectPlant(PlantViewModel plantViewModel)
        //{
        //    Unsubscribe();
        //    SelectedPlant = plantViewModel;
        //    Subscribe();
        //    UpdateArchitecture();
        //}

        private void Unsubscribe()
        {                
            if (SelectedPlant != null)
            {
                PropertyChangedEventManager.RemoveHandler(SelectedPlant.Plant, OnPlantPropertyChanged, string.Empty);
                PropertyChangedEventManager.RemoveHandler(SelectedPlant, OnHasArchitectureChanged, "HasArchitecture");
            }
        }

        private void Subscribe()
        {
            if (SelectedPlant != null)
            {
                PropertyChangedEventManager.AddHandler(SelectedPlant.Plant, OnPlantPropertyChanged, string.Empty);
                PropertyChangedEventManager.AddHandler(SelectedPlant, OnHasArchitectureChanged, "HasArchitecture");
            }
        }

        //public void SwichSelectedPlant(PlantViewModel model, bool isSingleSelected)
        //{
        //    if (_selectedPlant != null)
        //    {
        //        PropertyChangedEventManager.RemoveHandler(_selectedPlant.Plant, OnPlantPropertyChanged, string.Empty);
        //        PropertyChangedEventManager.RemoveHandler(_selectedPlant, OnHasArchitectureChanged, "HasArchitecture");
        //    }

        //    _selectedPlant = model;

        //    _plantPropertyModel.FocusedPlant = null;
        //    _traitViewModel.FocusedPlant = null;
        //    _architectureViewModel.Architecture = null;

        //    if (_selectedPlant != null)
        //    {
        //        PropertyChangedEventManager.AddHandler(_selectedPlant.Plant, OnPlantPropertyChanged, string.Empty);
        //        PropertyChangedEventManager.AddHandler(_selectedPlant, OnHasArchitectureChanged, "HasArchitecture");

        //        _plantPropertyModel.FocusedPlant = _selectedPlant;
        //        _traitViewModel.FocusedPlant = _selectedPlant;
        //        UpdateArchitecture();
        //    }

        //    var enabled = _selectedPlant != null && isSingleSelected && !_experimentService.IsTimeMachineOn;

        //    _plantPropertyModel.IsEnabled = enabled;
        //    _traitViewModel.IsEnabled = enabled;
        //}
        
        private void OnHasArchitectureChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateArchitecture();
        }


        private void OnPlantPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var plant = sender as Plant;
            if (plant == null)
            {
                return;
            }

            else if (e.PropertyName == "PlantArchitecture")
            {
                UpdateArchitecture();
            }
        }

        private void UpdateArchitecture()
        {
            if (SelectedPlant == null || SelectedPlant.Plant.PlantArchitecture == null || SelectedPlant.HasArchitecture == false)
            {
                _experimentService.SelectedPlantArchitecture = null;
                return;
            }

            var rootModel = PrepareMeristemModel(SelectedPlant.Plant.PlantArchitecture.Root, null);
            _experimentService.SelectedPlantArchitecture = new ObservableCollection<ArchitectureMeristemViewModel>(
                new ArchitectureMeristemViewModel[]
                    {
                        rootModel
                    });
        }
        
        private ArchitectureMeristemViewModel PrepareMeristemModel(Meristem meristem, IArchitectureItemModel parent)
        {
            var meristemModel = _meristemModelFactory.CreateExport().Value;
            meristemModel.Initialize(meristem);

            meristemModel.Parent = parent;
            meristemModel.IsExpanded = true;
            meristemModel.Focusable = true;

            if (meristem != null)
            {
                foreach (var node in meristem.Nodes)
                {
                    var nodeModel = PrepareNodeModel(node, meristemModel);
                    meristemModel.Children.Add(nodeModel);
                }
            }

            return meristemModel;
        }

        private ArchitectureNodeViewModel PrepareNodeModel(Node node, IArchitectureItemModel parent)
        {
            var nodeModel = _nodeViewFactory.CreateExport().Value;
            nodeModel.Initialize(node);

            nodeModel.Parent = parent;
            nodeModel.IsExpanded = true;
            nodeModel.Focusable = true;

            foreach (var meristem in node.Meristems)
            {
                var meristemModel = PrepareMeristemModel(meristem, nodeModel);

                if (node.MeristemA == meristem)
                {
                    meristemModel.ForeGround = Brushes.White;
                }
                else
                {
                    meristemModel.ForeGround = Brushes.Black;
                }

                nodeModel.Children.Add(meristemModel);
            }

            return nodeModel;
        }

        private void OnAddNode(object obj)
        {
            var param = obj as PlanterMenuCommandParam;
            if (param == null || param.TargetState == null || param.Source == null)
            {
                return;
            }

            var nodeType = (NodeType)param.TargetState;
            var model = param.Source as ArchitectureMeristemViewModel;
            if (model == null)
            {
                return;
            }

            var newNode = Node.CreateNewNode(nodeType);
            model.Meristem.Nodes.Add(newNode);

            var newNodeModel = PrepareNodeModel(newNode, model);
            newNodeModel.IsExpanded = true;

            model.Children.Add(newNodeModel);
        }

        private void OnRemoveNode(object obj)
        {
            var param = obj as PlanterMenuCommandParam;
            if (param == null || param.Source == null)
            {
                return;
            }

            var model = param.Source as ArchitectureNodeViewModel;
            if (model == null)
            {
                return;
            }

            var parent = model.Parent as ArchitectureMeristemViewModel;
            if (parent == null)
            {
                return;
            }

            var text = "Are you sure you want to remove node?";
            var caption = "Removing the node";

            if (MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                parent.Meristem.Nodes.Remove(model.Node);
                parent.Children.Remove(model);
            }
        }

        private void OnChangeMeristemType(object obj)
        {
            var param = obj as PlanterMenuCommandParam;
            if (param == null || param.TargetState == null || param.Source == null)
            {
                return;
            }

            var meristemType = (MeristemType)param.TargetState;
            var model = param.Source as ArchitectureMeristemViewModel;
            if (model == null)
            {
                return;
            }

            if (model.Meristem.Type == MeristemType.Branch && model.Meristem.Nodes.Count > 0)
            {
                var message = "Are you sure you want to change type? All branch nodes (" + model.Meristem.Nodes.Count + ") will be lost.";
                var caption = "Subnode removal confirmation";

                if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }

                model.Meristem.Nodes.Clear();
                model.Children.Clear();
            }

            model.Meristem.Type = meristemType;
        }

        private void OnChangeNodeType(object obj)
        {
            var param = obj as PlanterMenuCommandParam;
            if (param == null || param.TargetState == null || param.Source == null)
            {
                return;
            }

            var nodeType = (NodeType)param.TargetState;
            var model = param.Source as ArchitectureNodeViewModel;
            if (model == null)
            {
                return;
            }

            model.Node.Type = nodeType;
        }

        public void Reset()
        {
            Unsubscribe();
            UpdateArchitecture();
        }
    }
}
