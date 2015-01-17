using System.ComponentModel.Composition;
using System.Waf.Applications;
using PlanterApp.Applications.Views;
using PlanterApp.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Globalization;
using System;
using System.Windows.Media;
using PlanterApp.Applications.Services;

namespace PlanterApp.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class TrayViewModel : ViewModel<ITrayView>
    {
        private ICommandService _commandService;
        public ICommandService CommandService
        {
            get { return _commandService; }
            set { SetProperty(ref _commandService, value); }
        }

        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }

        private Brush _headerColour;
        public Brush HeaderColour
        {
            get { return _headerColour; }
            set { SetProperty(ref _headerColour, value); }
        }


        private Tray _tray;
        public Tray Tray
        {
            get { return _tray; }
            set { SetProperty(ref _tray, value); }
        }

        private SynchronizingCollection<PlantViewModel, Plant> _plants;
        public SynchronizingCollection<PlantViewModel, Plant> Plants
        {
            get { return _plants; }
            set
            {
                if (SetProperty(ref _plants, value) && Tray != null)
                {
                    UpdateHeader();
                }
            }
        }

        private ChamberViewModel _parentChamber;
        public ChamberViewModel ParentChamber
        {
            get { return _parentChamber; }
            set { SetProperty(ref _parentChamber, value); }
        }

        public void UpdateHeader()
        {
            if (Tray.ShuffleTimeStamp == DateTime.MinValue)
            {
                Header = Tray.Notes;
                HeaderColour = Brushes.Black;
                return;
            }

            Header = Tray.Notes + " Last shuffled " + Tray.ShuffleTimeStamp.ToString("ddd dd MMM yyyy", new CultureInfo("EN-us"));

            if ((DateTime.Now - Tray.ShuffleTimeStamp).Days >= 7)
            {
                HeaderColour = Brushes.Red;
            }
            else
            {
                HeaderColour = Brushes.Black;
            }

        }

        [ImportingConstructor]
        public TrayViewModel(ITrayView view, ICommandService commandService)
            : base(view)
        {
            _commandService = commandService;
        }

        internal void Reset()
        {
            if (_plants == null)
            {
                return;
            }

            foreach(var plant in _plants)
            {
                plant.Reset();
            }

            _plants = null;
            
        }
    }
}
