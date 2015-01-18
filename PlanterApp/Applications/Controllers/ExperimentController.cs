using Microsoft.Win32;
using PlanterApp.Applications.Services;
using PlanterApp.Applications.ViewModels;
using PlanterApp.Domain;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Text;
using System.Globalization;
using System.IO;

namespace PlanterApp.Applications.Controllers
{
    internal enum ExperimentLoadResult
    {
        Failed,
        DuplicateIdentifiers,
        Ok
    }

    [Export]
    internal class ExperimentController : IController
    {
        private readonly IExperimentService _experimentService;
        private readonly ICommandService _commandService;
        private readonly IFileService _fileService;
        private readonly IViewService _viewService;

        private readonly SelectionController _selectionController;
        //private readonly SpeechController _speechController;

        private readonly ExportFactory<ChamberViewModel> _chamberViewFactory;
        private readonly ExportFactory<TrayViewModel> _trayViewModelFactory;
        private readonly ExportFactory<PlantViewModel> _plantViewModelFactory;

        private readonly MainViewModel _mainViewModel;
        private readonly StatisticViewModel _statisticViewModel;

        private PlantViewModel _currentPlant;

        private string _fileName;

        public bool IsSaveEnabled
        {
            get { return HasUnsavedData() && string.IsNullOrEmpty(_fileName) == false; }
        }

        public bool IsSaveAsEnabled
        {
            get { return HasUnsavedData(); }
        }

        public string DefaultFileName
        {
            get
            {
                var date = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
                var defaultName = "experiment_data_" + date;

                if (string.IsNullOrEmpty(_fileName))
                {
                    return defaultName;
                }

                var file = Path.GetFileNameWithoutExtension(_fileName);
                defaultName = file + "_" + date;

                var parts = file.Split(' ', '_');

                int result;
                foreach (var part in parts)
                {
                    // try to figure if we have default pattern in filename (experiment name + date)
                    if (part.Length == 4 && int.TryParse(part, out result))
                    {
                        var dateIndex = file.IndexOf(part);
                        defaultName = file.Substring(0, dateIndex) + date;
                        break;
                    }
                }

                return defaultName;
            }
        }

        [ImportingConstructor]
        public ExperimentController(
            IExperimentService experimentService,
            IFileService fileService,
            ICommandService commandService,
            IViewService viewService,

            SelectionController selectionController,
            //SpeechController speechController,

            ExportFactory<ChamberViewModel> chamberViewFactory,
            ExportFactory<TrayViewModel> trayViewModelFactory,
            ExportFactory<PlantViewModel> plantViewModelFactory,

            MainViewModel mainViewModel,
            StatisticViewModel statisticViewModel)
        {
            _experimentService = experimentService;
            _commandService = commandService;
            _fileService = fileService;
            _viewService = viewService;

            _selectionController = selectionController;
            //_speechController = speechController;

            _chamberViewFactory = chamberViewFactory;
            _trayViewModelFactory = trayViewModelFactory;
            _plantViewModelFactory = plantViewModelFactory;

            _mainViewModel = mainViewModel;
            _statisticViewModel = statisticViewModel;

            _commandService.ShuffleCommand = new DelegateCommand(OnTrayShuffle);
            _commandService.ShowCoordinatesCommand = new DelegateCommand(OnShowCoordinatesCommand);
            _commandService.PlantGridCommand = new DelegateCommand(o => _experimentService.ShowPlantGrid = (bool)o);
            _commandService.PlantStatusUpdateCommand = new DelegateCommand(OnPlantStatusUpdate);
        }

        public void Initialize()
        {
            _viewService.StatisticView = _statisticViewModel.View;

            _commandService.ResetTimeMachineCommand = new DelegateCommand((o) => _mainViewModel.CurrentDate = DateTime.Now.Date);

            PropertyChangedEventManager.AddHandler(_mainViewModel, OnSelectedChamberViewChanged, "SelectedChamberView");
            PropertyChangedEventManager.AddHandler(_experimentService, OnSelectedPlantChanged, "SelectedPlant");

            Reset();

            _selectionController.Initialize();
            //_speechController.Initialize();
        }

        private void OnSelectedPlantChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_currentPlant != null)
            {
                PropertyChangedEventManager.RemoveHandler(_currentPlant, OnPlantCurrentStateChanged, "CurrentState");
            }

            _currentPlant = _experimentService.SelectedPlant as PlantViewModel;
            if (_currentPlant == null)
            {
                return;
            }

            PropertyChangedEventManager.AddHandler(_currentPlant, OnPlantCurrentStateChanged, "CurrentState");
        }

        private void OnPlantCurrentStateChanged(object sender, PropertyChangedEventArgs e)
        {
            //var param = obj as PlanterMenuCommandParam;
            //if (param == null || param.TargetState == null || param.Source == null)
            //{
            //    return;
            //}

            //var targetState = (PlantState)param.TargetState;
            //var model = param.Source as PlantViewModel;
            //if (model == null)
            //{
            //    return;
            //}

            if (_currentPlant == null)
            {
                return;
            }

            var text = "You have more than one plant selected. Do you want to other plants into state '" + _currentPlant.CurrentState + "' as well?";
            var caption = "Updating multiple statuses";
            var selectedPlants = from p in _selectionController.SelectedPlants
                                 where p != _experimentService.SelectedPlant
                                 select p;

            if (selectedPlants.Count() > 0
                && MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                return;
            }

            foreach (var plantModel in selectedPlants)
            {
                plantModel.AddStatus(_currentPlant.PlantStatuses.OfType<StatusItem>().LastOrDefault());
            }
        }

        private void OnSelectedChamberViewChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_mainViewModel.SelectedChamberView != null)
            {
                _selectionController.Reset();

                var plantModels = _mainViewModel.SelectedChamberView.Plants;
                _statisticViewModel.PlantModels = plantModels;
                //_speechController.PrepareSpeech(plantModels);
            }
        }

        public bool HasUnsavedData()
        {
            return _experimentService.HasExperiment;
        }

        public bool CanSave(object arg)
        {
            return IsSaveEnabled;
        }

        public bool CanSaveAs(object arg)
        {
            return IsSaveAsEnabled;
        }

        public bool CanExport(object arg)
        {
            return IsSaveAsEnabled;
        }

        public bool CanPrint(object arg)
        {
            return IsSaveAsEnabled;
        }

        public bool CanSpeak(object arg)
        {
            return IsSaveAsEnabled;
        }

        public void SaveExperiment()
        {
            _fileService.SaveExperiment(_fileName, _experimentService.Experiment);
            _mainViewModel.Title = ApplicationInfo.ProductName + " - " + _fileName;
        }

        public void SaveExperimentAs(string fileName)
        {
            _fileName = fileName;
            SaveExperiment();
        }

        public void ExportExperimentAs(string fileName)
        {
            _fileService.ExportExperiment(fileName, _experimentService.Experiment);
        }

        public ExperimentLoadResult LoadExperiment(string fileName)
        {
            Reset();

            _fileName = fileName;

            return InitializeExperiment(_fileService.LoadExperiment(fileName));


            //_selectionController.ClearSelected();

            //_experimentService.Experiment = null;

            //var experiment = _fileService.LoadExperiment(fileName);
            //if (experiment == null || experiment.HasData == false)
            //{
            //    return ExperimentLoadResult.Failed;
            //}

            //_experimentService.Experiment = experiment;
            //_fileName = fileName;

            //var errors = _experimentService.Validate();
            //_experimentService.ProcessTemporaryHacks();

            //InitializeMainView();

            //PrepareChamberViews();

            //if (errors.Count != 0)
            //{
            //    _fileService.Log(errors);
            //    return ExperimentLoadResult.DuplicateIdentifiers;
            //}

            //return ExperimentLoadResult.Ok;
        }

        public ExperimentLoadResult ImportExperiment(string fileName)
        {
            Reset();

            _fileName = null;

            return InitializeExperiment(_fileService.ImportExperiment(fileName));

            //_selectionController.ClearSelected();
            //_experimentService.Experiment = null;

            //var experiment = _fileService.ImportExperiment(fileName);
            //if (experiment == null || experiment.HasData == false)
            //{
            //    return ExperimentLoadResult.Failed;
            //}

            //_experimentService.Experiment = experiment;
            //_fileName = null;

            ////var errors = _experimentService.Validate();
            ////_experimentService.ProcessTemporaryHacks();

            //InitializeMainView();

            //PrepareChamberViews();

            ////if (errors.Count != 0)
            ////{
            ////    _fileService.Log(errors);
            ////    return ExperimentLoadResult.DuplicateIdentifiers;
            ////}

            //return ExperimentLoadResult.Ok;
        }

        private ExperimentLoadResult InitializeExperiment(Experiment experiment)
        {
            if (experiment == null || experiment.HasData == false)
            {
                return ExperimentLoadResult.Failed;
            }

            _experimentService.Experiment = experiment;
            _experimentService.TrayType = GetTrayType(experiment);

            var errors = _experimentService.Validate();
            _experimentService.ProcessTemporaryHacks();

            _mainViewModel.Title = string.IsNullOrEmpty(_fileName) ? ApplicationInfo.ProductName : ApplicationInfo.ProductName + " - " + _fileName;

            FindTimeMachineStartDate();
            PrepareChamberViews();

            //if (errors.Count != 0)
            //{
            //    _fileService.Log(errors);
            //    return ExperimentLoadResult.DuplicateIdentifiers;
            //}

            return ExperimentLoadResult.Ok;
        }

        public void Reset()
        {
            _selectionController.Reset();
            _statisticViewModel.Reset();
            _mainViewModel.Reset();
            _experimentService.Reset();
        }

        private void FindTimeMachineStartDate()
        {
            var startDate = DateTime.MaxValue;
            foreach (var plant in _experimentService.Experiment.GetPlants())
            {
                if (plant.PlantStatuses != null && plant.PlantStatuses.Count > 0)
                {
                    var firstStatus = plant.PlantStatuses.First();
                    if (firstStatus.TimeStamp < startDate)
                    {
                        startDate = firstStatus.TimeStamp.Date;
                    }
                }
            }

            if (startDate == DateTime.MaxValue)
            {
                startDate = DateTime.Now.AddYears(-5);
            }

            _mainViewModel.StartDate = startDate;
        }

        private void PrepareChamberViews()
        {
            var chamberViews = new SynchronizingCollection<ChamberViewModel, Chamber>(_experimentService.Experiment.Chambers, chamber =>
            {
                var chamberModel = _chamberViewFactory.CreateExport().Value;
                chamberModel.Chamber = chamber;
                chamberModel.Trays = PrepareTrayViews(chamberModel);

                return chamberModel;
            });

            _mainViewModel.ChamberViews = chamberViews;
            _mainViewModel.SelectedChamberView = chamberViews[0];
        }

        private SynchronizingCollection<TrayViewModel, Tray> PrepareTrayViews(ChamberViewModel chamberModel)
        {

            var trayModels = new SynchronizingCollection<TrayViewModel, Tray>(chamberModel.Chamber.Trays, tray =>
            {
                var trayModel = _trayViewModelFactory.CreateExport().Value;
                trayModel.ParentChamber = chamberModel;
                trayModel.Tray = tray;
                trayModel.Plants = PreparePlantViews(trayModel);

                return trayModel;
            });

            return trayModels;
        }

        private SynchronizingCollection<PlantViewModel, Plant> PreparePlantViews(TrayViewModel trayModel)
        {
            var plantModels = new SynchronizingCollection<PlantViewModel, Plant>(trayModel.Tray.Plants, plant =>
            {
                var plantModel = _plantViewModelFactory.CreateExport().Value;
                plantModel.ParentTray = trayModel;
                plantModel.Plant = plant;

                return plantModel;
            });

            return plantModels;
        }

        private TrayType GetTrayType(Experiment experiment)
        {
            var maxPlants = (from c in experiment.Chambers
                             select c.Trays).SelectMany(t => t).Max(t => t.Plants.Count);

            if (maxPlants <= 4 * 8)
            {
                return TrayType.Small;
            }
            else if (maxPlants <= 8 * 12)
            {
                return TrayType.Medium;
            }

            return TrayType.Huge;
        }

        private void OnTrayShuffle(object obj)
        {
            var trayModel = obj as TrayViewModel;

            if (trayModel != null && MessageBox.Show("Are you sure you want to randomize plant locations on the tray '" + trayModel.Tray.Notes + "'?",
                "Shuffling tray " + trayModel.Tray.Notes, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _selectionController.Reset();

                trayModel.Tray.Shuffle();
                trayModel.Tray.ShuffleTimeStamp = DateTime.Now;
                trayModel.UpdateHeader();
            }
        }

        private void OnShowCoordinatesCommand(object obj)
        {
            var trays = (from c in _mainViewModel.ChamberViews
                         select c.Trays).SelectMany(t => t);

            foreach (var tray in trays)
            {
                tray.ShowCoordinates((bool)obj);
            }
        }

        public void Show()
        {
            _mainViewModel.Show();
        }

        public void Close()
        {
            _mainViewModel.Close();
        }

        private void OnPlantStatusUpdate(object obj)
        {
            var param = obj as PlanterMenuCommandParam;
            if (param == null || param.TargetState == null || param.Source == null)
            {
                return;
            }

            var targetState = (PlantState)param.TargetState;
            var model = param.Source as PlantViewModel;
            if (model == null)
            {
                return;
            }

            model.AddStatus(new StatusItem { State = targetState, TimeStamp = DateTime.Now.Date });
        }

        //private void OnPlantStatusUpdate(object obj)
        //{
        //    var param = obj as PlanterMenuCommandParam;
        //    if (param == null || param.TargetState == null || param.Source == null)
        //    {
        //        return;
        //    }

        //    var targetState = (PlantState)param.TargetState;
        //    var model = param.Source as PlantViewModel;
        //    if (model == null)
        //    {
        //        return;
        //    }

        //    var text = "Do you want to update all selected plants into state '" + targetState + "'?";
        //    var caption = "Updating multiple statuses";
        //    var selectedPlants = _selectionController.SelectedPlants;

        //    if (selectedPlants.Count() > 1
        //        && MessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
        //    {
        //        return;
        //    }

        //    foreach (var plantModel in selectedPlants)
        //    {
        //        plantModel.AddStatus(targetState);
        //    }


        //    //var targetState = PlantState.InitFailed;

        //    //if (Enum.TryParse<PlantState>(obj.ToString(), true, out targetState))
        //    //{
        //    //}
        //}

        public IEnumerable<string> GetPlantTexts()
        {
            var texts = new List<string>();

            if (_experimentService == null || _experimentService.HasExperiment == false || _experimentService.Experiment.Properties == null)
            {
                texts.Add("No data to print!");
                return texts;
            }

            var sb = new StringBuilder();

            foreach (var plant in _experimentService.Experiment.GetPlants())
            {
                sb.Clear();

                var names = _experimentService.Experiment.Properties.GetNames(plant);
                var lastStatus = plant.PlantStatuses.LastOrDefault();

                sb.Append(plant.Id);
                sb.Append(" ");
                sb.Append(names[ExperimentProperties.Names.Species]);
                sb.Append(" ");
                sb.Append(names[ExperimentProperties.Names.Population]);
                sb.Append(" ");
                sb.Append(names[ExperimentProperties.Names.Family]);
                sb.Append(" ");
                sb.Append(plant.Individual);

                if (lastStatus != null)
                {
                    sb.Append(" ");
                    sb.Append(lastStatus.State);
                    sb.Append(" ");
                    sb.Append(lastStatus.TimeStamp.ToString("ddd dd MMM yyyy", new CultureInfo("EN-us")));
                }

                texts.Add(sb.ToString());
            }

            return texts;
        }
    }
}
