using PlanterApp.Applications.Services;
using PlanterApp.Applications.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace PlanterApp.Applications.Controllers
{
    [Export]
    internal class SpeechController : IController
    {
        private ISpeechService _speechService;
        private ICommandService _commandService;
        private IExperimentService _experimentService;
        private IEnumerable<PlantViewModel> _plants;

        [ImportingConstructor]
        public SpeechController(ISpeechService speechService, ICommandService commandService, IExperimentService experimentService)
        {
            _speechService = speechService;
            _commandService = commandService;
            _experimentService = experimentService;
        }

        public void Initialize()
        {
            _commandService.SpeechControlCommand = new DelegateCommand(OnToggleSpeech, CanSpeak);

            _speechService.PlantSelected += OnPlantSelected;
        }

        void OnPlantSelected(object sender, PlantSelectedEventArgs e)
        {
            if (_plants != null)
            {
                var plant = (from p in _plants
                             where p.Plant.Id == e.Id
                             select p).FirstOrDefault();

                _commandService.SelectPlantCommand.Execute(plant);
            }
        }

        public void PrepareSpeech(IEnumerable<PlantViewModel> plantModels)
        {
            if (plantModels == null)
            {
                return;
            }

            _plants = plantModels;

            var ids = (from plant in _plants
                      select plant.Plant.Id).ToList();

            _speechService.Initialize(ids);
        }

        private bool CanSpeak(object arg)
        {
            return _experimentService.HasExperiment;
        }

        private void OnToggleSpeech(object obj)
        {
            if (obj is bool)
            {
                var enable = (bool)obj;
                if (enable)
                {
                    _speechService.Start();
                }
                else
                {
                    _speechService.Stop();
                }
            }
        }

        public void Reset()
        {
            _speechService.Dispose();
        }
    }
}
