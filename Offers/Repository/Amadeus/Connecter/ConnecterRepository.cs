using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Offers.Models.Amadeus.Search;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Repository.Amadeus.Authorization;
using Offers.Services.Error;
using Offers.Services.FileSave;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Offers.Repository.Connecter
{
    /* This Class used for calling Amadeus .
     * Indetails to send request to Amadeus and get Response From Amadeus .
     */
    public class ConnecterRepository : IConnecterRepository<ConnecterBaseEntity>
    {
        private readonly IConfiguration _Configuration;
        private readonly IFileSaveService<FileSaveBaseEntity> _FileSave;
        private readonly ILogger<ConnecterRepository> _logger;
        private readonly IAuthorizationRepository _IAmadeus_AuthorizationRepository;
        private readonly IErrorService<ErrorBaseEntity> _ErrorLog;

        public ConnecterRepository(ILogger<ConnecterRepository> logger, IAuthorizationRepository IAmadeus_AuthorizationRepository, IConfiguration Configuration, IFileSaveService<FileSaveBaseEntity> FileSave, IErrorService<ErrorBaseEntity> ErrorLogCaller)
        {
            _IAmadeus_AuthorizationRepository = IAmadeus_AuthorizationRepository;
            _logger = logger;
            _Configuration = Configuration;
            _FileSave = FileSave;
            _ErrorLog = ErrorLogCaller;
        }



        public async Task<FlightOffersSearchRs> GetOffers(FlightOffersSearchRq FlightSearchRQObj,String TransactionId)
        {
           await _FileSave.Save_v1(FlightSearchRQObj, "AmadeusRQ", TransactionId);
            try
            {
                #region Calling to Supplier
                using (var client = new HttpClient())
                {
                    FlightOffersSearchRs FlightOffersSearchRSList = new FlightOffersSearchRs();
                    string CurrentEnvironment = _Configuration["AmadeusConfiguration:Environment"];
                    var Token = _IAmadeus_AuthorizationRepository.Authorization();
                    client.BaseAddress = new Uri(_Configuration["AmadeusConfiguration:" + CurrentEnvironment + ":BaseUri"] + "/v2/shopping/flight-offers");
                    string tokenfinal = "Bearer " + Token;
                    client.DefaultRequestHeaders.Add("Authorization", tokenfinal);
                    var Watch = System.Diagnostics.Stopwatch.StartNew();


                    var responseTask = client.PostAsJsonAsync(client.BaseAddress, FlightSearchRQObj).Result;

                    Watch.Stop();

                    var ElapsMilis = Watch.ElapsedMilliseconds;

                    _logger.LogInformation("Time took for Recievd result from Supplier : " + ElapsMilis / 1000);
                    
                    var Response = await responseTask.Content.ReadAsStringAsync();
 
       
                    if (responseTask.IsSuccessStatusCode)
                    {

                        await _FileSave.Save_v1(Response, "AmadeusRS", TransactionId);
                        FlightOffersSearchRSList = JsonConvert.DeserializeObject<FlightOffersSearchRs>(Response);
                        if (FlightOffersSearchRSList.data.Count == 0)
                        {
                            _logger.LogError(">>>>>>>>>>>> No results From supplier "+ TransactionId);

                            FlightOffersSearchRSList.ErrorList = new List<ErrorRs>();
                            FlightOffersSearchRSList.ErrorList.Add(await _ErrorLog.ErrorLog("204", "Result Not Found "));
                            return FlightOffersSearchRSList;

                        }
                        return FlightOffersSearchRSList;
                    }
                    else
                    {
                        await _FileSave.Save_v1(Response, "AmadeusRS",TransactionId);
                        Errors ErrorList = JsonConvert.DeserializeObject<Errors>(await responseTask.Content.ReadAsStringAsync());



                        foreach (var item in ErrorList.errors)
                        {
                            //_logger.LogError("FlightOffersSearch Error Code ="+  TransactionId + "_"+ item.code);
                           // _logger.LogError("FlightOffersSearch Error Status ="+  TransactionId + "_"+ item.status);
                            _logger.LogError("FlightOffersSearch Error Title =" + TransactionId+ "_"+ item.title);
                            _logger.LogError("FlightOffersSearch Error Detail =" + TransactionId+"_"+ item.detail);

                            FlightOffersSearchRSList.ErrorList = new List<ErrorRs>();
                            FlightOffersSearchRSList.ErrorList.Add(await _ErrorLog.ErrorLog(item.title,item.detail));
                        }
                       
                        return FlightOffersSearchRSList;
                    }
                }
                #endregion
            }
            catch (Exception Error)
            {
                _logger.LogInformation("Error while requesting Data from Amadeus");
                _logger.LogError("FlightOffersSearch Error", Error);
                return null;
            }
        }
    }

}
