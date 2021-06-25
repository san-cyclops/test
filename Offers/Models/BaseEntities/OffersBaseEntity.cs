using Offers.Models.Amadeus.Search;
using Offers.Models.Common;

namespace Offers.Models.BaseEntities
{
    public class OffersBaseEntity
    {
        public FlightOffersSearchRs FlightOffersSearchRS { get; set; }
        public FlightSearchQuery_v1 RequestQuery { get; set; }
    }
}
