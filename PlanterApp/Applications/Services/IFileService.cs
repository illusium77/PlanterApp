using PlanterApp.Domain;
using PlanterApp.Properties;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PlanterApp.Applications.Services
{
    internal interface IFileService
    {
        Experiment LoadExperiment(string fileName);
        Experiment ImportExperiment(string fileName);
        bool SaveExperiment(string fileName, Experiment experiment);
        bool ExportExperiment(string fileName, Experiment experiment);

        void Log(string logLine);
        void Log(IList<string> logLines);
    }
}
