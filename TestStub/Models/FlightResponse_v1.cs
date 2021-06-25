namespace TestStub
{
    public class FlightResponse_v1
    {
        public string id { get; set; }
        public Outbound outbound { get; set; }
        public Inbound inbound { get; set; }
        public Cost cost { get; set; }
    }
    public class Cost
    {
        public double amount { get; set; }
        public string unit { get; set; }

    }
    public class Ratings
    {
        public double? seatWidth { get; set; }
        public double? seatPitch { get; set; }
        public int? videoType { get; set; }
        public double? laptopPower { get; set; }
        public int? wiFi { get; set; }
        public double? seatType { get; set; }
        public double? seatPrivacy { get; set; }
    }

    public class Outbound
    {
        public Departure_v1 departure { get; set; }
        public Arrival_v1 arrival { get; set; }
        public string @class { get; set; }
        public string flight { get; set; }
        public Carrier_v1 carrier { get; set; }
        public Operating_v1 operating { get; set; }
        public Aircraft_v1 aircraft { get; set; }
        public Ratings ratings { get; set; }
        public string duration { get; set; }

    }

    public class Inbound
    {
        public Departure_v1 departure { get; set; }
        public Arrival_v1 arrival { get; set; }
        public string @class { get; set; }
        public string flight { get; set; }
        public Carrier_v1 carrier { get; set; }
        public Operating_v1 operating { get; set; }
        public Aircraft_v1 aircraft { get; set; }
        public Ratings ratings { get; set; }
        public string duration { get; set; }
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