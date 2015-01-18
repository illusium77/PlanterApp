using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PlanterApp.Applications.Services
{
    internal interface IExperimentService : INotifyPropertyChanged
    {
        Experiment Experiment { get; set; }
        bool HasExperiment { get; set; }
        bool IsTimeMachineOn { get; set; }
        
        IList<string> Validate();
        void ProcessTemporaryHacks();
        IList<string> CompareTo(Experiment other);

        void Reset();

        object SelectedPlant { get; set; }
        object SelectedPlantArchitecture { get; set; }
        TrayType TrayType { get; set; }
        bool ShowPlantGrid { get; set; }
    }
}
