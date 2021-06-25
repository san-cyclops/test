using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Models.Common.Weightnings
{
    
    public class Outbound_UserRating
    {
        public double seatWidth { get; set; }
        public double seatPitch { get; set; }
        public double seatType { get; set; }
        public double seatPrivacy { get; set; }
        public double seatRecline { get; set; }
        public double bedLength { get; set; }
    }

    public class Inbound_UserRating
    {
        public double seatWidth { get; set; }
        public double seatPitch { get; set; }
        public double seatType { get; set; }
        public double seatPrivacy { get; set; }
        public double seatRecline { get; set; }
        public double bedLength { get; set; }
    }

    public class Weightings
    {
        public Outbound_UserRating outbound { get; set; }
        public Inbound_UserRating inbound { get; set; }
    }

    public class Base64Weightings
    {
        public Weightings weightings { get; set; }
        public List<string> airlinePlans { get; set; }
        public List<ErrorRs> ErrorList { get; set; }
    }
}
