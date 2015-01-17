using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PlanterApp.Domain
{
    //[DataContract]
    //internal class PlantMeasurements
    //{
    //    [DataMember]
    //    public Dictionary<string, double> Measurements { get; set; }

    //    public PlantMeasurements()
    //    {
    //        Measurements = new Dictionary<string, double>();
    //    }

    //    public void AddMeasurement(string measurement, double value)
    //    {
    //        if (Measurements.ContainsKey(measurement) == false)
    //        {
    //            Measurements.Add(measurement, value);
    //            return;
    //        }

    //        Measurements[measurement] = value;
    //    }

    //    public double GetValue(string measurement)
    //    {
    //        if (Measurements.ContainsKey(measurement) == false)
    //        {
    //            throw new InvalidOperationException("Measurement '" + measurement + "' not found!");
    //        }

    //        return Measurements[measurement];
    //    }
    //}
}
