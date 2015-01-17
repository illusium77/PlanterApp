using System;
using System.Runtime.Serialization;
using System.Waf.Foundation;

namespace PlanterApp.Domain
{
    [DataContract]
    internal class StatusItem : Model
    {
        private PlantState _state;
        private DateTime _timeStamp;

        [DataMember]
        public PlantState State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        [DataMember]
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { SetProperty(ref _timeStamp, value.Date); }
        }

        public StatusItem()
        {
            TimeStamp = DateTime.Now;
        }

        public static int GetTimeDelta(StatusItem itemA, StatusItem itemB)
        {
            if (itemA == null || itemB == null)
            {
                throw new Exception("Cannot get time delta if item is null");
            }

            return (itemB.TimeStamp - itemA.TimeStamp).Days;
        }
    }
}
