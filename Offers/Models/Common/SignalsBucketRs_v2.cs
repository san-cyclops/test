using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Models.Common
{
    public class SignalsBucketRs_v2
    {
        public string Id { get; set; }
        public string Alliances { get; set; }
        public double OfferPrice { get; set; }
        public string CabinClass { get; set; }
        public int Date { get; set; }
        public double UserRating { get; set; }
        public double RatingDifference { get; set; }
        public double PriceDifference { get; set; }
        public double PriceThreshold { get; set; }
        public double ValueRatio { get; set; }
        public double ValueDifference { get; set; }
        public bool IsCheaper { get; set; }
        public bool IsSimilar { get; set; }
        public bool IsDearer { get; set; }
        public bool IsUpgrade { get; set; }
        public bool SavingsDowngrade { get; set; }
        public bool BetterCheaper { get; set; }
        public bool BetterSimilar { get; set; }
        public bool Trash { get; set; }
        public bool IsBetterValue { get; set; }
        public bool Include { get; set; }
        public string IsValidDestination { get; set; }
        public SignalsBucketRs_Outbound Outbound { get; set; }
        public SignalsBucketRs_Inbound Inbound { get; set; }

    }

    public class SignalsBucketRs_Outbound
    {
        public string Date { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string OperatingAirline { get; set; }
        public string Flight { get; set; }
        public string Cabin { get; set; }
        public double UserRating { get; set; }

    }
    public class SignalsBucketRs_Inbound
    {
        public string Date { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string OperatingAirline { get; set; }
        public string Flight { get; set; }
        public string Cabin { get; set; }
        public double UserRating { get; set; }

    }

}
