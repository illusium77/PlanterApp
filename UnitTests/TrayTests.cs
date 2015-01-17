using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlanterApp.Domain;

namespace UnitTests
{
    [TestClass]
    public class TrayTests
    {
        [TestMethod]
        public void TestAddPlant()
        {
            var tray = new Tray();
            var plant = new Plant(1);

            tray.AddPlant(plant);

            foreach (var p in tray.Plants)
            {
                Assert.AreSame(plant, p);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestDoubleAddPlant()
        {
            var tray = new Tray();
            var plant = new Plant(1);

            tray.AddPlant(plant);
            tray.AddPlant(plant);
        }

        [TestMethod]
        public void TestRemovePlant()
        {
            var tray = new Tray();
            var plant = new Plant(1);

            Assert.IsFalse(tray.RemovePlant(plant));

            tray.AddPlant(plant);
            Assert.IsTrue(tray.RemovePlant(plant));

            Assert.IsFalse(tray.RemovePlant(plant));
        }

        [TestMethod]
        public void TestShuffle()
        {
            var tray = new Tray();
            tray.AddPlant(new Plant(1));
            tray.AddPlant(new Plant(2));
            tray.AddPlant(new Plant(3));
            tray.AddPlant(new Plant(4));
            tray.AddPlant(new Plant(5));
            tray.Shuffle();
        }
    }
}
