using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Signals
{
    public interface IGetAlternattiveOffers<T> where T : SignalsBaseEntity
    {
        public Task<List<FlightResponse_v1>> GetOffers(string Iata,FlightSearchQuery_v2 Query);
    }
}
