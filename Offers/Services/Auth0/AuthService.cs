using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Offers.Models.Auth0;
using Offers.Models.BaseEntities;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace Offers.Services.Auth0
{
    public class AuthService : IAuthService<AuthBaseEntity>
    {
        private readonly ILogger _logger;

        public AuthService(ILogger<AuthService> Logger)
        {
            _logger = Logger;
        }
        public async Task<AuthRs> AuthCallerAsync(AuthRq RequestObj)
        {
            #region Calling to Supplier
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("https://upgradeengine.us.auth0.com/oauth/token");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var Watch = System.Diagnostics.Stopwatch.StartNew();


                var responseTask = client.PostAsJsonAsync(client.BaseAddress, RequestObj).Result;

                Watch.Stop();

                var ElapsMilis = Watch.ElapsedMilliseconds;

                _logger.LogInformation("Time took for Recievd result from Supplier : " + ElapsMilis / 1000);

                if (responseTask.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<AuthRs>(await responseTask.Content.ReadAsStringAsync());
                }
                _logger.LogError("eturn Null Authentication keys from OAuth");
                return null;
            }
            #endregion
        }
    }
}
