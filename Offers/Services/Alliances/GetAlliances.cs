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

namespace Offers.Services.Alliances
{
    public class GetAlliances : IGetAlliances
    {

        #region GetAlliancesFields
        private readonly IAuthService<AuthBaseEntity> _AuthRepositoryCaller;
        private readonly ICacheService<CacheBaseEntity> _CacheCaller;
        private readonly ILogger<GetAlliances> _Logger;
        private readonly IFileSaveService<FileSaveBaseEntity> _FileSaveCaller;
        private readonly ICacheService<CacheBaseEntity> _Cachecaller;

        #endregion

        #region GetAlliancesConstuctor
        public GetAlliances(ICacheService<CacheBaseEntity> Cachecaller, IAuthService<AuthBaseEntity> AuthRepositoryCaller, IFileSaveService<FileSaveBaseEntity> FileSaveCaller, ICacheService<CacheBaseEntity> CacheCaller, ILogger<GetAlliances> Logger, IFileSaveService<FileSaveBaseEntity> IFileSave)
        {
            _Cachecaller = Cachecaller;
            _AuthRepositoryCaller = AuthRepositoryCaller;
            _CacheCaller = CacheCaller;
            _Logger = Logger;
            _FileSaveCaller = IFileSave;
        }
        #endregion
        async Task<List<AirlineRS>> IGetAlliances.GetAlliances(string[] Airline)
        {
            //Set and Get Auth Token 
            AuthRq AuthRequestObj = new AuthRq();
            AuthRequestObj.client_id = "lcjcPju7JT5uJWkoD2rDJtoYbK0tEBeG";
            AuthRequestObj.client_secret = "O59TIb82ypGt3oiGXkx1Xm5uK3IjZ8dCMDuX4IcqvlLQ9hrKrZP_X9xFsn8jXoaA";
            AuthRequestObj.audience = "https://upgradeengine.com";
            AuthRequestObj.grant_type = "client_credentials";

            AuthRs AuthResponseObj = await _AuthRepositoryCaller.AuthCallerAsync(AuthRequestObj);

            await _FileSaveCaller.Save_v1(AuthRequestObj, "Alliances-AuthRq", "");
            await _FileSaveCaller.Save_v1(AuthResponseObj, "Alliances-AuthRs", "");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://staging--upgradeengine.netlify.app/api/v1/airlines?airlines=");

                try
                {
                    if (AuthResponseObj != null)
                    {
                        string Tokenfinal = "Bearer " + AuthResponseObj.access_token;
                        client.DefaultRequestHeaders.Add("Authorization", Tokenfinal);
                    }
                    else
                    {
                        _Logger.LogWarning("Authkey - Bearer token return null from upgradeengine.com");
                    }

                }
                catch (Exception Error)
                {
                    _Logger.LogInformation("Error in BeareToken for this transaction Id" + "");
                    _Logger.LogError(Error.StackTrace);
                }

                StringBuilder AirlineParameter = new StringBuilder();
                foreach (var item in Airline)
                {
                    AirlineParameter.Append(item + ",");
                }

                var Request = client.BaseAddress.ToString() + AirlineParameter.ToString().TrimEnd(',');

                await _FileSaveCaller.Save_v1(Request, "Alliance-RQ", "");
                Console.WriteLine("Alliance Request >> " + Request);

                var GetTask = await client.GetAsync(Request);

                if (GetTask.IsSuccessStatusCode)
                {
                    try
                    {
                        try
                        {
                            List<AirlineRS> AirlineList = JsonConvert.DeserializeObject<List<AirlineRS>>(await _Cachecaller.GetCache(Request));
                            await _FileSaveCaller.Save_v1(AirlineList, "Seats-RS", "");
                            return await Task.FromResult(AirlineList);
                        }
                        catch (Exception)
                        {
                            List<AirlineRS> AirlineList = JsonConvert.DeserializeObject<List<AirlineRS>>(GetTask.Content.ReadAsStringAsync().Result);
                            await _Cachecaller.PostCache(Request, AirlineList);
                            await _FileSaveCaller.Save_v1(AirlineList, "Seats-RS", "");
                            return await Task.FromResult(AirlineList);
                        }

                    }
                    catch (Exception ErrorMessage)
                    {
                        await _FileSaveCaller.Save_v1(ErrorMessage, "Error", "");
                        _Logger.LogInformation("Error in BeareToken for this transaction Id" + "");
                        _Logger.LogError(ErrorMessage.StackTrace);
                    }
                }

            }
            return null;

        }
    }
}
