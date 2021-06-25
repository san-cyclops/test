using Offers.Models;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Services.Cache
{
    public interface ICacheService<T> where T : CacheBaseEntity
    {
        Task<FlightResponse_v1> GetOffer(String Key);
        Task<FlightResponse_v3> GetFlight(String Key);
        Task<List<FlightResponse_v1>> GetOffers(String Key);
        Task<List<FlightResponse_v3>> GetFlights(String Key);
        Task<bool> PostOffers(List<FlightResponse_v1> FlightResponse_v1Obj,String BaseKey);
        Task<bool> PostFlights(List<FlightResponse_v3> FlightResponse_v3Obj, String BaseKey);
        Task<string> GetCache(String Key);
        Task<bool> PostCache(String Key,object Obj);
        Task<SeatsRs> GetSeat(String Key);
        Task<bool> PostSeat(String Key, object Obj);
        Task<bool> ClearCache(String Key);
        Task<bool> FlushCache();
        Task<List<SeatsRs>> GetSeats(String Key);
        Task<bool> PostSeats(String Key, object Obj);
    }
}
