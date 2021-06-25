using Offers.Models;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Flight
{
    public interface IFlightService<T> where T : OfferBaseEntity
    {
        public Task<FlightResponse_v4>FlightBaseCaller(string[] uids, string TransactionId, string Context);

    }
}
 