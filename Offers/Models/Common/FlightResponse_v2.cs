using Offers.Models.Common;
using System.Collections.Generic;

namespace Offers.Models
{
    public class FlightResponse_v2
    {
        public string id { get; set; }
        public DepartureFlight_v2 outbound { get; set; }
        public ReturnFlight_v2 inbound { get; set; }
        public double cost { get; set; }
        public List<Booking> booking { get; set; }
        public List<ErrorRs> ErrorList { get; set; }
    }

    public class Departure_v2
    {
        public string code { get; set; }
        public object name { get; set; }
        public object city { get; set; }
        public object country { get; set; }
        public string at { get; set; }
    }

    public class Arrival_v2
    {
        public string code { get; set; }
        public object name { get; set; }
        public object city { get; set; }
        public object country { get; set; }
        public string at { get; set; }
    }

    public class Carrier_v2
    {
        public string code { get; set; }
        public object name { get; set; }
    }

    public class Operating_v2
    {
        public string code { get; set; }
        public object name { get; set; }
    }

    public class Amenity_v2
    {
        public string name { get; set; }
        public string unit { get; set; }
        public object value { get; set; }
        public double rating { get; set; }
        public string key { get; set; }
    }


    public class Cabins_v2
    {
        public bool random { get; set; }
        public string cabinClass { get; set; }
        public List<string> pictures { get; set; }
        public List<Amenity_v2> amenities { get; set; }
        public double rating { get; set; }
        public string video { get; set; }
    }

    public class Aircraft_v2
    {
        public string code { get; set; }
        public Cabins_v2 cabin { get; set; }
    }

    public class Cost_v2
    {
        public string @class { get; set; }
        public double price { get; set; }
        public object rating { get; set; }
    }

    public class DepartureFlight_v2
    {
        public string duration { get; set; }
        public Departure_v2 departure { get; set; }
        public Arrival_v2 arrival { get; set; }
        public Carrier_v2 carrier { get; set; }
        public Operating_v2 operating { get; set; }
        public Aircraft_v2 aircraft { get; set; }
        public int numberOfStops { get; set; }
        public int number { get; set; }
    }

    public class ReturnFlight_v2
    {
        public string duration { get; set; }
        public Departure_v2 departure { get; set; }
        public Arrival_v2 arrival { get; set; }
        public Carrier_v2 carrier { get; set; }
        public Operating_v2 operating { get; set; }
        public Aircraft_v2 aircraft { get; set; }
        public int numberOfStops { get; set; }
        public int number { get; set; }
    }

    public class Booking
    {
        public string provider { get; set; }
        public string url { get; set; }
    }

}
