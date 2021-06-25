using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Signals
{
    public interface IGetOffersService <T> where T : SignalsBaseEntity
    {
        public Task<List<FlightResponse_v1>> GetOffers(FlightSearchQuery_v2 Query);

    }
}
