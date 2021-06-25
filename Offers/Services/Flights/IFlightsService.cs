using Microsoft.AspNetCore.Mvc;
using Offers.Models;
using Offers.Models.Amadeus.Search;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Services.Flights
{
    public interface IFlightsService<T> where T : OffersBaseEntity
    {
        Task<List<FlightResponse_v3>> FlightsSearchBaseCaller ([FromQuery] FlightSearchQuery_v3 RequestQuery);        
    }
}
