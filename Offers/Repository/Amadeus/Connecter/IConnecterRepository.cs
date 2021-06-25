using Offers.Models.Amadeus.Search;
using Offers.Models.BaseEntities;
using System.Threading.Tasks;

namespace Offers.Repository.Connecter
{
    public interface IConnecterRepository<T> where T : ConnecterBaseEntity
    {
        Task<FlightOffersSearchRs> GetOffers(FlightOffersSearchRq FlightSearchRQObj,string TransactionId);
    }

}
