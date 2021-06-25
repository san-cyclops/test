using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Models.Common.Weightnings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Signals
{
    public interface ISignalsPaidService <T> where T : SignalsBaseEntity
    {
        Task<QuadrantsRS> SignalsPaidServiceBaseCaller (List<FlightResponse_v1> FlightResponse_v1, string Context, string TransactionId, string BaseOffer);
    }
}
