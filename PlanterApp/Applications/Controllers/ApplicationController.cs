using Microsoft.Win32;
using PlanterApp.Applications.Services;
using PlanterApp.Applications.ViewModels;
using PlanterApp.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace PlanterApp.Applications.Controllers
{
    [Export]
    internal class ApplicationController : IController
    {
        private readonly ExperimentController _experimentController;
        private readonly ICommandService _commandService;

        [ImportingConstructor]
        public ApplicationController(ExperimentController experimentController, ICommandService commandService)
        {
            _experimentController = experimentController;
            _commandService = commandService;

            _commandService.SaveCommand = new DelegateCommand(OnSave, _experimentController.CanSave);
            _commandService.SaveAsCommand = new DelegateCommand(OnSaveAs, _experimentController.CanSaveAs);
            _commandService.ExportCommand = new DelegateCommand(OnExport, _experimentController.CanExport);
            _commandService.PrintCommand = new DelegateCommand(OnPrint, _experimentController.CanPrint);
            _commandService.LoadCommand = new DelegateCommand(OnLoad);
            _commandService.ImportCommand = new DelegateCommand(OnImport);
            _commandService.ExitCommand = new DelegateCommand(OnShutdown);

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        public void Initialize()
        {
            _experimentController.Initialize();

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (args[1].EndsWith(".xml"))
                {
                    HandleLoadResult(_experimentController.LoadExperiment(args[1]), args[1]);
                }
                else if (args[1].EndsWith(".csv"))
                {
                    HandleLoadResult(_experimentController.ImportExperiment(args[1]), args[1]);
                }
            }
        }

        public void Run()
        {
            _experimentController.Show();
        }

        public void OnShutdown()
        {
            if (_experimentController.HasUnsavedData() && MessageBox.Show("Do you want to save data before exiting the application?", 
                ApplicationInfo.ProductName, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (_experimentController.IsSaveEnabled)
                {
                    OnSave(null);
                }
                else if (_experimentController.IsSaveAsEnabled)
                {
                    OnSaveAs(null);
                }
            }

            _experimentController.Close();
        }

        public void Reset()
        {
        }

        private void OnLoad(object obj)
        {
            var openFile = new OpenFileDialog { DefaultExt = ".xml", Filter = "XML File (*.xml)|*.xml", Multiselect = false };

            var result = openFile.ShowDialog();
            if (result == true)
            {
                HandleLoadResult(_experimentController.LoadExperiment(openFile.FileName), openFile.FileName);
            }
        }

        private void OnImport(object obj)
        {
            var openFile = new OpenFileDialog { DefaultExt = ".csv", Filter = "CSV File (*.csv)|*.csv", Multiselect = false };

            var result = openFile.ShowDialog();
            if (result == true)
            {
                HandleLoadResult(_experimentController.ImportExperiment(openFile.FileName), openFile.FileName);
                RaiseCanExecuteChanged();
            }
        }

        private void HandleLoadResult(ExperimentLoadResult result, string fileName)
        {
            switch (result)
            {
                case ExperimentLoadResult.Failed:
                    MessageBox.Show("Could not load experiment data from file '" + fileName + "'!",
                        ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case ExperimentLoadResult.DuplicateIdentifiers:
                    MessageBox.Show("Duplicate plant identifiers found and they were fixed. See file 'planter_log.txt' for details.", ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    break;
            }

            RaiseCanExecuteChanged();

        }

        private void OnSave(object obj)
        {
            _experimentController.SaveExperiment();
            RaiseCanExecuteChanged();
        }

        private void OnSaveAs(object obj)
        {
            var saveFile = new SaveFileDialog { FileName = _experimentController.DefaultFileName, DefaultExt = ".xml", Filter = "XML File (*.xml)|*.xml" };

            if (saveFile.ShowDialog() == true)
            {
                _experimentController.SaveExperimentAs(saveFile.FileName);
                RaiseCanExecuteChanged();
            }
        }

        private void OnPrint(object obj)
        {
            var printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                var flowDocument = new FlowDocument();

                foreach (var plantText in _experimentController.GetPlantTexts())
                {
                    Paragraph myParagraph = new Paragraph();
                    myParagraph.Margin = new Thickness(0);
                    myParagraph.Inlines.Add(new Run(plantText));
                    flowDocument.Blocks.Add(myParagraph);
                }
                DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
                printDialog.PrintDocument(paginator, "jes");
            }
        }

        private void RaiseCanExecuteChanged()
        {
            _commandService.SaveCommand.RaiseCanExecuteChanged();
            _commandService.SaveAsCommand.RaiseCanExecuteChanged();
            _commandService.ExportCommand.RaiseCanExecuteChanged();
            _commandService.PlantGridCommand.RaiseCanExecuteChanged();
            _commandService.ShowCoordinatesCommand.RaiseCanExecuteChanged();

            //_commandService.SpeechControlCommand.RaiseCanExecuteChanged();
        }

        private void OnExport(object obj)
        {
            var defaultName = "plant_data_export_" + DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
            var exportFile = new SaveFileDialog { FileName = defaultName, DefaultExt = ".csv", Filter = "CSV File (*.csv)|*.csv" };

            if (exportFile.ShowDialog() == true)
            {
                _experimentController.ExportExperimentAs(exportFile.FileName);
            }
        }
    }
}
