using System.Collections.Generic;

namespace Offers.Models.Amadeus.Search
{
    /*
     * Request Model for Offers which used to send Amadeus API for take results for particular query
     * 
     */
    public class DepartureDateTimeRange
    {
        public string date { get; set; }
        public string time { get; set; }
        public string dateWindow { get; set; }
    }

    public class OriginDestination
    {
        public string id { get; set; }
        public string originLocationCode { get; set; }
        public string destinationLocationCode { get; set; }
        public DepartureDateTimeRange departureDateTimeRange { get; set; }
    }

    public class Traveler
    {
        public string id { get; set; }
        public string travelerType { get; set; }
    }

    public class CabinRestriction
    {
        public string cabin { get; set; }
        public string coverage { get; set; }
        public List<string> originDestinationIds { get; set; }
    }

    public class FlightFilters
    {
        public List<CabinRestriction> cabinRestrictions { get; set; }
        public connectionRestriction connectionRestriction { get; set; }
    }

    public class connectionRestriction
    {
        public int maxNumberOfConnections { get; set; }
    }

   
    public class SearchCriteria
    {
        public int maxFlightOffers { get; set; }
        public FlightFilters flightFilters { get; set; }
    }

    public class FlightOffersSearchRq
    {
        public string currencyCode { get; set; }
        public List<OriginDestination> originDestinations { get; set; }
        public List<Traveler> travelers { get; set; }
        public List<string> sources { get; set; }
        public SearchCriteria searchCriteria { get; set; }
    }
}
