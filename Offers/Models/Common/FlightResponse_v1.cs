using System.Collections.Generic;

namespace Offers.Models.Common
{
    /*
     * Offers response model v1 curruent in use this version for offers
     * 
     */
    public class FlightResponse_v1
    {
        public string id { get; set; }
        public Outbound outbound { get; set; }
        public Inbound inbound { get; set; }
        public Cost cost { get; set; }
        public List<ErrorRs> ErrorList { get; set; }
    }
    public class Cost
    {
        public double amount { get; set; }
        public string unit { get; set; }

    }
    public class Ratings
    {
        public bool random { get; set; }
        public double seatWidth { get; set; }
        public double seatRecline { get; set; }
        public double seatPitch { get; set; }
        public double videoType { get; set; }
        public double laptopPower { get; set; }
        public double wiFi { get; set; }
        public double seatType { get; set; }
        public double seatPrivacy { get; set; }
        public double bedLenth { get; set; }
    }

    public class Outbound
    {
        public string @class { get; set; }
        public string flight { get; set; }
        public string duration { get; set; }
        public Departure_v1 departure { get; set; }
        public Arrival_v1 arrival { get; set; }
        public Carrier_v1 carrier { get; set; }
        public Operating_v1 operating { get; set; }
        public Aircraft_v1 aircraft { get; set; }
        public Ratings ratings { get; set; }

    }

    public class Inbound
    {
        public string @class { get; set; }
        public string flight { get; set; }
        public string duration { get; set; }
        public Departure_v1 departure { get; set; }
        public Arrival_v1 arrival { get; set; }
        public Carrier_v1 carrier { get; set; }
        public Operating_v1 operating { get; set; }
        public Aircraft_v1 aircraft { get; set; }
        public Ratings ratings { get; set; }

    }

    public class Departure_v1
    {
        public string code { get; set; }
        public string name { get; set; }
        public string at { get; set; }
    }

    public class Arrival_v1
    {
        public string code { get; set; }
        public string name { get; set; }
        public string at { get; set; }
    }

    public class Operating_v1
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Carrier_v1
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Aircraft_v1
    {
        public string code { get; set; }
        public string name { get; set; }
    }

}