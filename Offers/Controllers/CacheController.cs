using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Offers.Models.BaseEntities;
using Offers.Services.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Controllers
{
    [Route("offers/v1/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private ICacheService<CacheBaseEntity> _CacheCaller;

        public CacheController(ICacheService<CacheBaseEntity> CacheCaller)
        {
            _CacheCaller = CacheCaller;
        }

        #region DeleteCache
        [HttpGet]
        [Route("DeleteCache")]
        public async Task<bool> DeleteCache(String key)
        {
            try
            {
                // _logger.LogDebug("response"+ response);
                return await Task.FromResult(await _CacheCaller.ClearCache(key));

            }
            catch (Exception)
            {
                /* Should have provide meaning full error that recvieved from supplier example : no results for that date
                 * Need create errorObj and added to the FlightOffersSearchRS
                 */
                return false;
            }
        }
        #endregion

        #region DeleteCache
        [HttpGet]
        [Route("FlushCache")]
        public async Task<bool> FlushCache()
        {
            try
            {
                // _logger.LogDebug("response"+ response);
                return await Task.FromResult(await _CacheCaller.FlushCache());

            }
            catch (Exception ErrorMessage)
            {
                Console.WriteLine(ErrorMessage.StackTrace);
                return false;
            }
        }
        #endregion

    }
}
