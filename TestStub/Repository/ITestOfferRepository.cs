using System.Collections.Generic;
using System.Threading.Tasks;
using TestStub.Models;

namespace TestStub.Repository
{
    public interface ITestOfferRepository<T> where T : IOffersBaseEntity
    {

        public Task<List<OfferCompareResponse>> OfferCompareCaller();

    }
}
