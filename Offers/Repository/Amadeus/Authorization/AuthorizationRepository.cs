using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Offers.Models.Amadeus.Authorization;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Offers.Repository.Amadeus.Authorization
{
    /*
     * This class used for create authorize token this class will invoke Amadeus Auth call and return Auth token .
     */

    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly IConfiguration _Configuration;
        private readonly ILogger<AuthorizationRepository> _Logger;

        public AuthorizationRepository(IConfiguration Configuration, ILogger<AuthorizationRepository> Logger)
        {
            _Configuration = Configuration;
            _Logger = Logger;
        }


        public string Authorization()
        {
            //var CurrentEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string CurrentEnvironment = _Configuration["AmadeusConfiguration:Environment"];
            try
            {
                Dictionary<string, string> Params = new Dictionary<string, string>();
                Params.Add("client_id", _Configuration["AmadeusConfiguration:" + CurrentEnvironment + ":ClientId"]);
                Params.Add("client_secret", _Configuration["AmadeusConfiguration:" + CurrentEnvironment + ":ClientSecret"]);
                Params.Add("grant_type", _Configuration["AmadeusConfiguration:" + CurrentEnvironment + ":GrantType"]);

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_Configuration["AmadeusConfiguration:" + CurrentEnvironment + ":BaseUri"]);
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

                    var postTask = client.PostAsync("v1/security/oauth2/token", new FormUrlEncodedContent(Params));
                    postTask.Wait();

                    var result = postTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        AuthorizationRs AuthorizationRSModelObj = JsonConvert.DeserializeObject<AuthorizationRs>(result.Content.ReadAsStringAsync().Result);
                        return AuthorizationRSModelObj.Access_Token;
                    }

                }
            }
            catch (Exception Error)
            {
                _Logger.LogError("Autherization Error", Error);
                return null;
            }
            _Logger.LogError("return finally null block");
            return null;
        }
    }
}
