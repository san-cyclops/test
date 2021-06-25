using Offers.Models.BaseEntities;
using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Locations
{
    public interface ILocationsService<T> where T : LocationsBaseEntity
    {
        public Task<List<LocationsRs>> GetLocations(string[] ItaCode,string TransactionId);
        public Task<LocationsRs> GetLocation(string ItaCode, string TransactionId);
    }
}
