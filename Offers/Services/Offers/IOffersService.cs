using Microsoft.AspNetCore.Mvc;
using Offers.Models.Amadeus.Search;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Services.Offers
{
    public interface IOffersService<T> where T : OffersBaseEntity
    {
        Task<List<FlightResponse_v1>> FlightOffersSearch([FromQuery] FlightSearchQuery_v1 RequestQuery);
        Task<List<FlightOffersSearchRs>> FlightOffersSearchock([FromQuery] FlightSearchQuery_v1 RequestQuery);
    }
}
