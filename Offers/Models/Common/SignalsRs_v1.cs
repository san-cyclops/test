using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Models.Common
{
    public class SignalsRs_v1
    {
        public List<Destination> destinations { get; set; }
        public List<Alliance> alliances { get; set; }
        public List<Airline> airlines { get; set; }
        public List<DateFlexibility> dateFlexibility { get; set; }
        public MixedCabin mixedCabin { get; set; }
        public List<Cabin> cabins { get; set; }
        public string[] offerIds { get; set; }
        public double BaselinePrice { get; set; }
        public double BaselineRating { get; set; }

    }

    public class Destination
    {
        public string code { get; set; }
        public double minPriceDiff { get; set; }
        public double maxPriceDiff { get; set; }
        public double minRatingDiff { get; set; }
        public double maxRatingDiff { get; set; }
        public double minValueDiff { get; set; }
        public double maxValueDiff { get; set; }
    }

    public class Alliance
    {
        public string code { get; set; }
        public double minPriceDiff { get; set; }
        public double maxPriceDiff { get; set; }
        public double minRatingDiff { get; set; }
        public double maxRatingDiff { get; set; }
        public double minValueDiff { get; set; }
        public double maxValueDiff { get; set; }
    }

    public class Airline
    {
        public string code { get; set; }
        public double minPriceDiff { get; set; }
        public double maxPriceDiff { get; set; }
        public double minRatingDiff { get; set; }
        public double maxRatingDiff { get; set; }
        public double minValueDiff { get; set; }
        public double? maxValueDiff { get; set; }
    }

    public class DateFlexibility
    {
        public int code { get; set; }
        public double minPriceDiff { get; set; }
        public double maxPriceDiff { get; set; }
        public double minRatingDiff { get; set; }
        public double maxRatingDiff { get; set; }
        public double minValueDiff { get; set; }
        public double maxValueDiff { get; set; }
    }

    public class MixedCabin
    {
        public double minPriceDiff { get; set; }
        public double maxPriceDiff { get; set; }
        public double minRatingDiff { get; set; }
        public double maxRatingDiff { get; set; }
        public double minValueDiff { get; set; }
        public double maxValueDiff { get; set; }
    }

    public class Cabin
    {
        public string code { get; set; }
        public double minPriceDiff { get; set; }
        public double maxPriceDiff { get; set; }
        public double minRatingDiff { get; set; }
        public double maxRatingDiff { get; set; }
        public double minValueDiff { get; set; }
        public double maxValueDiff { get; set; }
    }
}
