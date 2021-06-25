using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Offers.Models.Auth0;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Auth0;
using Offers.Services.Cache;
using Offers.Services.FileSave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Offers.Services.Locations
{
    public class LocationsService : ILocationsService<LocationsBaseEntity>
    {
        #region Fileds
        public IAuthService<AuthBaseEntity> _AuthRepositoryCaller;
        private ILogger<LocationsService> _logger;
        private IFileSaveService<FileSaveBaseEntity> _FileSave;
        private ICacheService<CacheBaseEntity> _CacheCaller;
        #endregion

        #region Constructor
        public LocationsService(IAuthService<AuthBaseEntity> AuthRepositoryCaller,ILogger<LocationsService> logger,IFileSaveService<FileSaveBaseEntity> FileSave,ICacheService<CacheBaseEntity> CacheCaller)
        {
            _AuthRepositoryCaller = AuthRepositoryCaller;
            _logger = logger;
            _FileSave = FileSave;
            _CacheCaller = CacheCaller;
        }
        #endregion

        #region GetLocations
        public Task<List<LocationsRs>> GetLocations(string[] ItaCode, string TransactionId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetLocation
        public async Task<LocationsRs> GetLocation(string ItaCode,string TransactionId)
        {
            AuthRq AuthRequestObj = new AuthRq();
            AuthRequestObj.client_id = "lcjcPju7JT5uJWkoD2rDJtoYbK0tEBeG";
            AuthRequestObj.client_secret = "O59TIb82ypGt3oiGXkx1Xm5uK3IjZ8dCMDuX4IcqvlLQ9hrKrZP_X9xFsn8jXoaA";
            AuthRequestObj.audience = "https://upgradeengine.com";
            AuthRequestObj.grant_type = "client_credentials";

            AuthRs AuthResponseObj = await _AuthRepositoryCaller.AuthCallerAsync(AuthRequestObj);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://staging--upgradeengine.netlify.app/api/v1/locations?iata=");
                await _FileSave.Save_v1(client.BaseAddress,"LocationRQ",TransactionId);
                try
                {
                    if (AuthResponseObj.access_token != null)
                    {
                        string Tokenfinal = "Bearer " + AuthResponseObj.access_token;
                        client.DefaultRequestHeaders.Add("Authorization", Tokenfinal);
                    }
                }
                catch
                {
                    _logger.LogError("OAuth Token Error : Token null for this tancation Id "+TransactionId);
                }
      
                StringBuilder IdParameter = new StringBuilder();

                var GetTask = await client.GetAsync(client.BaseAddress.ToString() + ItaCode);

                if (GetTask.IsSuccessStatusCode)
                {
                    try
                    {
                        LocationsRs LocationsRsList = JsonConvert.DeserializeObject<LocationsRs>(GetTask.Content.ReadAsStringAsync().Result);
                        await _FileSave.Save_v1(LocationsRsList, "LocationRS", TransactionId);
                        return await Task.FromResult(LocationsRsList);
                    }
                    catch (Exception ErrorMessage)
                    {
                        _logger.LogError(ErrorMessage.StackTrace);
                       _logger.LogInformation("Location Respose null for this transaction Id " + TransactionId);
                        return null;
                    }
                }

            }
            _logger.LogInformation("Location Respose null for this transaction Id " + TransactionId +"Because of OAuth");
            return null;
        }
        #endregion

    }
}
