using Offers.Models.Common;
using System.Collections.Generic;

namespace Offers.Models
{
    public class FlightResponse_v3
    {
        public string id { get; set; }
        public string duration { get; set; }
        public Departure_v3 departure { get; set; }
        public Arrival_v3 arrival { get; set; }
        public Carrier_v3 carrier { get; set; }
        public Operating_v3 operating { get; set; }
        public Aircraft_v3 aircraft { get; set; }
        public int numberOfStops { get; set; }
        public string number { get; set; }
        public string cabinClass { get; set; }
        public double cost { get; set; }
        public List<ErrorRs> ErrorList { get; set; }
    }
    public class Aircraft_v3
    {
        public string code { get; set; }
        public string name { get; set; }
    }
    public class Departure_v3
    {
        public string code { get; set; }
        public object name { get; set; }
        public object city { get; set; }
        public object country { get; set; }
        public string at { get; set; }
    }

    public class Arrival_v3
    {
        public string code { get; set; }
        public object name { get; set; }
        public object city { get; set; }
        public object country { get; set; }
        public string at { get; set; }
    }

    public class Carrier_v3
    {
        public string code { get; set; }
        public object name { get; set; }
    }

    public class Operating_v3
    {
        public string code { get; set; }
        public object name { get; set; }
    }
}
