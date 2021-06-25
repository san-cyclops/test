using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TestStub.Models;

namespace TestStub.Repository
{
    public class TestAuthRepository : ITestAuthRepository<IAuthBaseEntity>
    {
        private readonly ILogger _logger;

        public TestAuthRepository(ILogger<TestAuthRepository> Logger)
        {
            _logger = Logger;
        }
        public async Task<AuthResponse> AuthCallerAsync(AuthRequest RequestObj)
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
                    return JsonConvert.DeserializeObject<AuthResponse>(await responseTask.Content.ReadAsStringAsync());
                }
                _logger.LogInformation(" Error Return Null");
                return null;
            }
            #endregion
        }
    }
}
