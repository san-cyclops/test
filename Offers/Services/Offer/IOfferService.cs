using Offers.Models;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Services.Offer
{
    public interface IOfferService<T> where T : OfferBaseEntity
    {
        public Task<List<FlightResponse_v2>> OfferBaseCaller(string[] uids, string TransactionId, string Context);

    }
}
