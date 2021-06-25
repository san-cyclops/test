using Newtonsoft.Json;
using Offers.Models;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Offers.Services.Cache
{
    public class CacheService : ICacheService<CacheBaseEntity>
    {
        /*
         * This class uses for store Redis cache and get Redis cache
         */

        private readonly IDatabase _Database;

        public CacheService(IDatabase Database)
        {
            _Database = Database;
        }

        /*
         * This Method get cache data for single offer
         */
        async Task<FlightResponse_v1> ICacheService<CacheBaseEntity>.GetOffer(string Key)
        {
            return JsonConvert.DeserializeObject<FlightResponse_v1>(await _Database.StringGetAsync(Key));
        }

        /*
        * This Method return cache data instead of Amadeus Offers it will return as List of offers
        */
        async Task<List<FlightResponse_v1>> ICacheService<CacheBaseEntity>.GetOffers(string Key)
        {
            return JsonConvert.DeserializeObject<List<FlightResponse_v1>>(await _Database.StringGetAsync(Key));
        }
        async Task<List<FlightResponse_v3>> ICacheService<CacheBaseEntity>.GetFlights(string Key)
        {
            return JsonConvert.DeserializeObject<List<FlightResponse_v3>>(await _Database.StringGetAsync(Key));
        }

        public async Task<bool> PostOffers(List<FlightResponse_v1> FlightResponse_v1Obj,String BaseKey)
        {
            DateTime CurrentTime = DateTime.Now.AddHours(2.00);
            TimeSpan ExpireTime = new TimeSpan(CurrentTime.Ticks);
            await _Database.StringSetAsync(BaseKey, JsonConvert.SerializeObject(FlightResponse_v1Obj),ExpireTime);
           
            if (FlightResponse_v1Obj == null)
            {
                return false;
            }

            foreach (var item in FlightResponse_v1Obj)
            {
                await _Database.StringSetAsync(item.id, JsonConvert.SerializeObject(item), ExpireTime);
            }
            return true;
        }
        public async Task<bool> PostFlights(List<FlightResponse_v3> FlightResponse_v3Obj, String BaseKey)
        {
            DateTime CurrentTime = DateTime.Now.AddHours(2.00);
            TimeSpan ExpireTime = new TimeSpan(CurrentTime.Ticks);
            await _Database.StringSetAsync(BaseKey, JsonConvert.SerializeObject(FlightResponse_v3Obj), ExpireTime);

            if (FlightResponse_v3Obj == null)
            {
                return false;
            }

            foreach (var item in FlightResponse_v3Obj)
            {
                await _Database.StringSetAsync(item.id, JsonConvert.SerializeObject(item), ExpireTime);
            }
            return true;
        }

        //Common Implementation to get and view cache
        public async Task<string> GetCache(string Key)
        {
            return await _Database.StringGetAsync(Key);
        }

        //Common Implementation to Post cache
        public async Task<bool> PostCache(string Key, Object obj)
        {
            try
            {
                DateTime CurrentTime = DateTime.Now.AddHours(2.00);
                TimeSpan ExpireTime = new TimeSpan(CurrentTime.Ticks);
                await _Database.StringSetAsync(Key, JsonConvert.SerializeObject(obj), ExpireTime);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<SeatsRs>> GetSeats(string Key)
        {
            return JsonConvert.DeserializeObject<List<SeatsRs>>(await _Database.StringGetAsync(Key));
        }

        public async Task<bool> PostSeats(string Key, object Obj)
        {
            try
            {
                DateTime CurrentTime = DateTime.Now.AddHours(2.00);
                TimeSpan ExpireTime = new TimeSpan(CurrentTime.Ticks);
                await _Database.StringSetAsync(Key, JsonConvert.SerializeObject(Obj), ExpireTime);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<SeatsRs> GetSeat(string Key)
        {
            return JsonConvert.DeserializeObject<SeatsRs>(await _Database.StringGetAsync(Key));
        }

        public async Task<bool> PostSeat(string Key, object Obj)
        {
            try
            {
                DateTime CurrentTime = DateTime.Now.AddHours(2.00);
                TimeSpan ExpireTime = new TimeSpan(CurrentTime.Ticks);
                await _Database.StringSetAsync(Key, JsonConvert.SerializeObject(Obj), ExpireTime);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /* clear cache using key */
        public async Task<bool> ClearCache(string Key)
        {
            _Database.KeyDelete(Key);
            return await Task.FromResult(true);
        }

    /* clear all cacche in cache storage */
    public async Task<bool> FlushCache()
        {
            var options = ConfigurationOptions.Parse("cache:6379");
            options.ConnectRetry = 5;
            options.AllowAdmin = true;
            var RedisConnection= ConnectionMultiplexer.Connect(options);

            var server = RedisConnection.GetServer("cache:6379");
            server.FlushAllDatabases();

            return await Task.FromResult(true);
        }

        async Task<FlightResponse_v3> ICacheService<CacheBaseEntity>.GetFlight(string Key)
        {
            return JsonConvert.DeserializeObject<FlightResponse_v3>(await _Database.StringGetAsync(Key));
        }
    }
}
