using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Models.Common
{
    public class SignalsBucketRs_v1
    {
        public string OfferID { get; set; }
        public double Price { get; set; }
        public int Date { get; set; }
        public string Destination { get; set; }
        public string Airline { get; set; }
        public string Flight { get; set; }
        public string Cabin { get; set; }
        public string Alliances { get; set; }
        public double Rating { get; set; }
        public double ValueRatio { get; set; }
        public double DeltaPrice { get; set; }
        public double DeltaRating { get; set; }
        public double DeltaValue { get; set; }

        public List<ErrorRs> ErrorList { get; set; }
    }
}
