using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlanterApp.Domain;

namespace UnitTests
{
    [TestClass]
    public class PlantTests
    {
        [TestMethod]
        public void TestPlantConstructor()
        {
            var plant = new Plant(1);

            //Assert.AreEqual(PlantState.Alive, plant.Statuses.CurrentState);
            //Assert.AreEqual(DateTime.Now.Date, plant.Statuses.CurrentTimeStamp.Date);
        }
    }
}
