using PlanterApp.Domain;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Collections.ObjectModel;
using System.Text;

namespace PlanterApp.Applications.Services
{
    [Export(typeof(ICommandService)), PartCreationPolicy(CreationPolicy.Shared)]
    internal class CommandService : Model, ICommandService
    {
        private DelegateCommand _shuffleCommand;
        public DelegateCommand ShuffleCommand
        {
            get { return _shuffleCommand; }
            set { SetProperty(ref _shuffleCommand, value); }
        }

        private DelegateCommand _plantStatusUpdateCommand;
        public DelegateCommand PlantStatusUpdateCommand
        {
            get { return _plantStatusUpdateCommand; }
            set { SetProperty(ref _plantStatusUpdateCommand, value); }
        }

        #region Architecture Commands

        private DelegateCommand _addNodeCommand;
        public DelegateCommand AddNodeCommand
        {
            get { return _addNodeCommand; }
            set { SetProperty(ref _addNodeCommand, value); }
        }

        private DelegateCommand _changeMeristemTypeCommand;
        public DelegateCommand ChangeMeristemTypeCommand
        {
            get { return _changeMeristemTypeCommand; }
            set { SetProperty(ref _changeMeristemTypeCommand, value); }
        }

        private DelegateCommand _removeNodeCommand;
        public DelegateCommand RemoveNodeCommand
        {
            get { return _removeNodeCommand; }
            set { SetProperty(ref _removeNodeCommand, value); }
        }

        private DelegateCommand _changeNodeTypeCommand;
        public DelegateCommand ChangeNodeTypeCommand
        {
            get { return _changeNodeTypeCommand; }
            set { SetProperty(ref _changeNodeTypeCommand, value); }
        }


        #endregion

        #region Selection Commands

        private DelegateCommand _ctrlDownCommand;
        public DelegateCommand CtrlDownCommand
        {
            get { return _ctrlDownCommand; }
            set { SetProperty(ref _ctrlDownCommand, value); }
        }

        private DelegateCommand _selectPlantCommand;
        public DelegateCommand SelectPlantCommand
        {
            get { return _selectPlantCommand; }
            set { SetProperty(ref _selectPlantCommand, value); }
        }

        private DelegateCommand _selectMultiplePlantsCommand;
        public DelegateCommand SelectMultiplePlantsCommand
        {
            get { return _selectMultiplePlantsCommand; }
            set { SetProperty(ref _selectMultiplePlantsCommand, value); }
        }
        
        #endregion
        
        #region Application Commands
        
        private DelegateCommand _resetTimeMachineCommand;
        public DelegateCommand ResetTimeMachineCommand
        {
            get { return _resetTimeMachineCommand; }
            set { SetProperty(ref _resetTimeMachineCommand, value); }
        }
        
        private DelegateCommand _exitCommand;
        public DelegateCommand ExitCommand
        {
            get { return _exitCommand; }
            set { SetProperty(ref _exitCommand, value); }
        }

        private DelegateCommand _loadCommand;
        public DelegateCommand LoadCommand
        {
            get { return _loadCommand; }
            set { SetProperty(ref _loadCommand, value); }
        }

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand
        {
            get { return _saveCommand; }
            set { SetProperty(ref _saveCommand, value); }
        }

        private DelegateCommand _saveAsCommand;
        public DelegateCommand SaveAsCommand
        {
            get { return _saveAsCommand; }
            set { SetProperty(ref _saveAsCommand, value); }
        }

        private DelegateCommand _importCommand;
        public DelegateCommand ImportCommand
        {
            get { return _importCommand; }
            set { SetProperty(ref _importCommand, value); }
        }

        private DelegateCommand _exportCommand;
        public DelegateCommand ExportCommand
        {
            get { return _exportCommand; }
            set { SetProperty(ref _exportCommand, value); }
        }

        private DelegateCommand _printCommand;
        public DelegateCommand PrintCommand
        {
            get { return _printCommand; }
            set { SetProperty(ref _printCommand, value); }
        }
        
        #endregion


        #region Speech Commands

        private DelegateCommand _speechControlCommand;
        public DelegateCommand SpeechControlCommand
        {
            get { return _speechControlCommand; }
            set { SetProperty(ref _speechControlCommand, value); }
        }

        #endregion
    }
}
