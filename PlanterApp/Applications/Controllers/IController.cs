using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanterApp.Applications.Controllers
{
    interface IController
    {
        void Initialize();
        void Reset();
    }
}
