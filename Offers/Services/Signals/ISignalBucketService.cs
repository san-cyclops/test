using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Signals
{
    public interface ISignalBucketService <T> where T : SignalsBaseEntity
    {
        public Task<SignalsRs_v1> SignalBucketBaseCaller(List<FlightResponse_v1> FlightResponse_v1List,String BaselineOffer,String TransactionID,String Context);
    }
}
