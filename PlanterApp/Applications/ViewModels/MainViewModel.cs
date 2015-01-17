using PlanterApp.Applications.Services;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;

namespace PlanterApp.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.Shared)]
    internal class MainViewModel : ViewModel<IMainView>
    {
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

        private IViewService _viewService;
        public IViewService ViewService
        {
            get { return _viewService; }
            set { SetProperty(ref _viewService, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private ChamberViewModel _selectedChamberView;
        public ChamberViewModel SelectedChamberView
        {
            get { return _selectedChamberView; }
            set
            {
                if (SetProperty(ref _selectedChamberView, value))
                {
                    ApplyCurrentDate();
                }
            }
        }

        private SynchronizingCollection<ChamberViewModel, Chamber> _chamberViews;
        public SynchronizingCollection<ChamberViewModel, Chamber> ChamberViews
        {
            get { return _chamberViews; }
            set { SetProperty(ref _chamberViews, value); }
        }
        
        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); }
        }

        private DateTime _currentDate;
        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set
            {
                if (SetProperty(ref _currentDate, value))
                {
                    ApplyCurrentDate();
                }
            }
        }

        private bool _showCoordinates;
        public bool ShowCoordinates
        {
            get { return _showCoordinates; }
            set { SetProperty(ref _showCoordinates, value); }
        }

        [ImportingConstructor]
        public MainViewModel(IMainView view, IExperimentService experimentService, ICommandService commandService, IViewService viewService)
            : base(view)
        {
            _experimentService = experimentService;
            _commandService = commandService;
            _viewService = viewService;

            _currentDate = DateTime.Now;
        }

        private void ApplyCurrentDate()
        {
            if (_selectedChamberView != null)
            {
                foreach (var plantModel in _selectedChamberView.Plants)
                {
                    plantModel.CurrentDate = _currentDate;
                }
            }

            _experimentService.IsTimeMachineOn = _currentDate.Date < DateTime.Now.Date;
        }

        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()
        {
            ViewCore.Close();
        }
        
        public void Reset()
        {
            Title = ApplicationInfo.ProductName;
            StartDate = DateTime.Now.AddYears(-5);
            CurrentDate = DateTime.Now.Date;
            EndDate = DateTime.Now.Date;
            SelectedChamberView = null;

            if (_chamberViews == null)
            {
                return;
            }

            foreach (var chamber in _chamberViews)
            {
                chamber.Reset();
            }

            _chamberViews = null;

            ShowCoordinates = false;
        }
    }
}
