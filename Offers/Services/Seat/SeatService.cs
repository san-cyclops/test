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

namespace Offers.Services.Seat
{
    public class SeatService : ISeatService<SeatBaseEntity>
    {
        #region fields
        private readonly IFileSaveService<FileSaveBaseEntity> _FileSaveCaller;
        private readonly IAuthService<AuthBaseEntity> _AuthRepositoryCaller;
        private readonly ILogger<SeatService> _Logger;
        private readonly ICacheService<CacheBaseEntity> _Cachecaller;
        #endregion

        #region Constructor
        public SeatService(IAuthService<AuthBaseEntity> AuthRepositoryCaller,IFileSaveService<FileSaveBaseEntity> FileSaveCaller,ILogger<SeatService> Logger,ICacheService<CacheBaseEntity> Cachecaller)
        {
            _FileSaveCaller = FileSaveCaller;
            _AuthRepositoryCaller = AuthRepositoryCaller;
            _Logger = Logger;
            _Cachecaller = Cachecaller;
        }
        #endregion

        #region GetSeats
        public async Task<List<SeatsRs>> GetSeats(String[] Id, string TransactionId,string Context)
        {
            //Set and Get Auth Token 
            AuthRq AuthRequestObj = new AuthRq();
            AuthRequestObj.client_id = "lcjcPju7JT5uJWkoD2rDJtoYbK0tEBeG";
            AuthRequestObj.client_secret = "O59TIb82ypGt3oiGXkx1Xm5uK3IjZ8dCMDuX4IcqvlLQ9hrKrZP_X9xFsn8jXoaA";
            AuthRequestObj.audience = "https://upgradeengine.com";
            AuthRequestObj.grant_type = "client_credentials";

            AuthRs AuthResponseObj = await _AuthRepositoryCaller.AuthCallerAsync(AuthRequestObj);

            await _FileSaveCaller.Save_v1(AuthRequestObj, "Seats-AuthRq", TransactionId);
            await _FileSaveCaller.Save_v1(AuthResponseObj,"Seats-AuthRs", TransactionId);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://staging--upgradeengine.netlify.app/api/v1/seats?seats=");

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
                    _Logger.LogInformation("Error in BeareToken for this transaction Id"+TransactionId);
                    _Logger.LogError(Error.StackTrace);
                }

                StringBuilder IdParameter = new StringBuilder();
                foreach (var item in Id)
                {
                    IdParameter.Append(item + ",");
                }

                string ContextParam = "&context=" + Context;
                var Request = client.BaseAddress.ToString() + IdParameter + ContextParam;

                await _FileSaveCaller.Save_v1(Request, "Seats-RQ", TransactionId);
                Console.WriteLine("Seat Request >> "+ Request);

                var GetTask = await client.GetAsync(Request);

                if (GetTask.IsSuccessStatusCode)
                {
                    try
                    {
                        List<SeatsRs> SeatDataRSList = JsonConvert.DeserializeObject<List<SeatsRs>>(GetTask.Content.ReadAsStringAsync().Result);
                        await _FileSaveCaller.Save_v1(SeatDataRSList, "Seats-RS", TransactionId);
                        return await Task.FromResult(SeatDataRSList);
                    }
                    catch (Exception ErrorMessage)
                    {
                        await _FileSaveCaller.Save_v1(ErrorMessage, "Error", TransactionId);
                        _Logger.LogInformation("Error in BeareToken for this transaction Id" + TransactionId);
                        _Logger.LogError(ErrorMessage.StackTrace);
                    }
                }

            }
            return null;
        }
        #endregion

        #region GetSeat
        public async Task<SeatsRs> GetSeat(String Id, string TransactionId, string Context)
        {
            //Set and Get Auth Token 
            AuthRq AuthRequestObj = new AuthRq();
            AuthRequestObj.client_id = "lcjcPju7JT5uJWkoD2rDJtoYbK0tEBeG";
            AuthRequestObj.client_secret = "O59TIb82ypGt3oiGXkx1Xm5uK3IjZ8dCMDuX4IcqvlLQ9hrKrZP_X9xFsn8jXoaA";
            AuthRequestObj.audience = "https://upgradeengine.com";
            AuthRequestObj.grant_type = "client_credentials";

            AuthRs AuthResponseObj = await _AuthRepositoryCaller.AuthCallerAsync(AuthRequestObj);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://staging--upgradeengine.netlify.app/api/v1/seats?seats=");

                try
                {
                    string Tokenfinal = "Bearer " + AuthResponseObj.access_token;
                    client.DefaultRequestHeaders.Add("Authorization", Tokenfinal);
                }
                catch (Exception Error)
                {
                    _Logger.LogInformation("Error in Beare token for this transaction id "+TransactionId);
                    _Logger.LogError(Error.StackTrace);
                }
              
                String IdParameter = Id;
                string ContextParam = "&context=" + Context;

                String Key = client.BaseAddress.ToString() + IdParameter + ContextParam;

                SeatsRs SeatDataRSObjCache = new SeatsRs();
                try
                {
                    SeatDataRSObjCache = await _Cachecaller.GetSeat(Key);
                    await _FileSaveCaller.Save_v1(Key, "Cache-Seat-RQ", TransactionId);
                    await _FileSaveCaller.Save_v1(SeatDataRSObjCache, "Cache-Seat-RS", TransactionId);
                    return await Task.FromResult(SeatDataRSObjCache);
                }
                catch 
                {
                    var Request = client.BaseAddress.ToString() + IdParameter + ContextParam;
                    await _FileSaveCaller.Save_v1(Request, "Seat-RQ", TransactionId);
                    var GetTask = await client.GetAsync(Request);

                    if (GetTask.IsSuccessStatusCode)
                    {
                        try
                        {
                            try
                            {
                                SeatsRs SeatDataRSObj = new SeatsRs();
                                List<SeatsRs> SeatsList = JsonConvert.DeserializeObject<List<SeatsRs>>(GetTask.Content.ReadAsStringAsync().Result);
                                SeatDataRSObj = SeatsList.Where(s => s.id == IdParameter).FirstOrDefault();
                                await _FileSaveCaller.Save_v1(SeatDataRSObj, "Seat-RS", TransactionId);
                                await _Cachecaller.PostSeat(Key, SeatDataRSObj);
                                return await Task.FromResult(SeatDataRSObj);

                            }
                            catch (Exception ErrorMessage)
                            {
                                _Logger.LogInformation("Error in decoding seat data" + TransactionId);
                                _Logger.LogError(ErrorMessage.StackTrace);
                            }

                        }
                        catch (Exception ErrorMessage)
                        {
                            _Logger.LogInformation("Error in Calling seat data" + TransactionId);
                            _Logger.LogError(ErrorMessage.StackTrace);
                        }
                    }

                }
            }
            return null;
        }
        #endregion

    }
}
