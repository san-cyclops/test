using Offers.Models.Common;
using System.Collections.Generic;

namespace Offers.Models
{
    public class FlightResponse_v4
    {
        public string id { get; set; }
        public string duration { get; set; }
        public Departure_v4 departure { get; set; }
        public Arrival_v4 arrival { get; set; }
        public Carrier_v4 carrier { get; set; }
        public Operating_v4 operating { get; set; }
        public Aircraft_v4 aircraft { get; set; }
        public int numberOfStops { get; set; }
        public string number { get; set; }
        public double cost { get; set; }
        public List<ErrorRs> ErrorList { get; set; }
    }
 
    public class Departure_v4
    {
        public string code { get; set; }
        public object name { get; set; }
        public object city { get; set; }
        public object country { get; set; }
        public string at { get; set; }
    }

    public class Arrival_v4
    {
        public string code { get; set; }
        public object name { get; set; }
        public object city { get; set; }
        public object country { get; set; }
        public string at { get; set; }
    }

    public class Carrier_v4
    {
        public string code { get; set; }
        public object name { get; set; }
    }

    public class Operating_v4
    {
        public string code { get; set; }
        public object name { get; set; }
    }

    public class Amenity_v4
    {
        public string name { get; set; }
        public string unit { get; set; }
        public object value { get; set; }
        public double rating { get; set; }
        public string key { get; set; }
    }

    public class Cabin_v4
    {
        public string cabinClass { get; set; }
        public List<string> pictures { get; set; }
        public List<Amenity_v4> amenities { get; set; }
        public double rating { get; set; }
    }

    public class Aircraft_v4
    {
        public string code { get; set; }
        public object name { get; set; }
        public Cabin_v4 cabin { get; set; }
    }

   


}
