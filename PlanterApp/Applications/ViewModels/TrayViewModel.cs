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
                    InitRowAndColumnNames();
                }
            }
        }

        private ChamberViewModel _parentChamber;
        public ChamberViewModel ParentChamber
        {
            get { return _parentChamber; }
            set { SetProperty(ref _parentChamber, value); }
        }

        private int _columnCount;
        public int ColumnCount
        {
            get { return _columnCount; }
            set { SetProperty(ref _columnCount, value); }
        }

        private ObservableCollection<string> _columnNames;
        public ObservableCollection<string> ColumnNames
        {
            get { return _columnNames; }
            set { SetProperty(ref _columnNames, value); }
        }

        private ObservableCollection<string> _rowNames;
        public ObservableCollection<string> RowNames
        {
            get { return _rowNames; }
            set { SetProperty(ref _rowNames, value); }
        }

        private Visibility _coordinateVisibility;
        public Visibility CoordinateVisibility
        {
            get { return _coordinateVisibility; }
            set { SetProperty(ref _coordinateVisibility, value); }
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
        public TrayViewModel(ITrayView view, ICommandService commandService, IExperimentService experimentService)
            : base(view)
        {
            _commandService = commandService;

            switch (experimentService.TrayType)
            {
                case TrayType.Huge:
                    ColumnCount = 10;
                    break;
                case TrayType.Small:
                case TrayType.Medium:
                default:
                    ColumnCount = 8;
                    break;
            }

            CoordinateVisibility = Visibility.Collapsed;
        }

        private void InitRowAndColumnNames()
        {
            ColumnNames = new ObservableCollection<string>();
            for (int i = 0; i < ColumnCount; i++)
            {
                var ch = (char)('A' + i);
                ColumnNames.Add(ch.ToString());
            }

            RowNames = new ObservableCollection<string>();
            for (int i = 0; i <= Plants.Count / ColumnCount; i++)
            {
                RowNames.Add(i != 0 ? i.ToString() : string.Empty);
            }
        }

        internal void Reset()
        {
            if (_plants == null)
            {
                return;
            }

            foreach (var plant in _plants)
            {
                plant.Reset();
            }

            CoordinateVisibility = Visibility.Collapsed;
            _plants = null;
        }

        internal void ShowCoordinates(bool showCoordinates)
        {
            CoordinateVisibility = showCoordinates ? Visibility.Visible : Visibility.Collapsed;
        }

        internal void UpdateSelected(PlantViewModel lastPlant)
        {
            if (CoordinateVisibility == Visibility.Visible && lastPlant != null)
            {
                var index = Plants.IndexOf(lastPlant);

                var x = index % ColumnCount;
                var y = index / ColumnCount + 1;

                _rowNames[0] = "(" + _columnNames[x] + ", " + _rowNames[y] + ")";
            }
            else
            {
                _rowNames[0] = string.Empty;
            }
        }
    }
}
