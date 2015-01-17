using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanterApp.Applications.Services
{
    internal interface IViewService
    {
        object PlantPropertyView { get; set; }

        object TraitView { get; set; }

        object StatisticView { get; set; }
    }
}
