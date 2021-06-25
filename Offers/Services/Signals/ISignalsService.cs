using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Signals
{
    public interface ISignalsService <T> where T : SignalsBaseEntity
    {
        public Task<QuadrantsRS> GetSignals(FlightSearchQuery_v2 Query);
    }
}
