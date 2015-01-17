using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;

namespace PlanterApp.Applications.Services
{
    internal interface ICommandService
    {
        DelegateCommand ShuffleCommand { get; set; }

        DelegateCommand PlantStatusUpdateCommand { get; set; }

        DelegateCommand ResetTimeMachineCommand { get; set; }

        DelegateCommand CtrlDownCommand { get; set; }

        DelegateCommand SelectPlantCommand { get; set; }

        DelegateCommand SelectMultiplePlantsCommand { get; set; }

        DelegateCommand SaveCommand { get; set; }

        DelegateCommand SaveAsCommand { get; set; }

        DelegateCommand ExportCommand { get; set; }

        DelegateCommand PrintCommand { get; set; }

        DelegateCommand LoadCommand { get; set; }

        DelegateCommand ImportCommand { get; set; }

        DelegateCommand ExitCommand { get; set; }

        DelegateCommand AddNodeCommand { get; set; }

        DelegateCommand ChangeMeristemTypeCommand { get; set; }

        DelegateCommand RemoveNodeCommand { get; set; }

        DelegateCommand ChangeNodeTypeCommand { get; set; }

        //DelegateCommand SpeechControlCommand { get; set; }
    }
}
