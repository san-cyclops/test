using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Offers.Models.Amadeus.Search;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Models.Common.Weightnings;
using Offers.Services.Cache;
using Offers.Services.Offers;
using Offers.Services.UserRating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.Controllers
{
    [Route("offers/v1/[controller]")]
    [ApiController]
    public class MockController : ControllerBase
    {
        #region Fields
        private readonly ILogger<MockController> _logger;
        private readonly IOffersService<OffersBaseEntity> _Amadeus_SearchRepositoryCaller;
        private readonly ICacheService<CacheBaseEntity> _CacheCaller;
        private readonly IUserRatingService<UserRatingBaseEntity> _IUserRatingCaller;
        public static Dictionary<string, string> Ratings = new Dictionary<string, string>();
        #endregion

        #region MockConstroctor
        public MockController(IUserRatingService<UserRatingBaseEntity> IUserRatingCaller, ILogger<MockController> logger, IOffersService<OffersBaseEntity> Amadeus_SearchRepositoryCaller, ICacheService<CacheBaseEntity> CacheCaller)
        {
            _logger = logger;
            _Amadeus_SearchRepositoryCaller = Amadeus_SearchRepositoryCaller;
            _CacheCaller = CacheCaller;
            _IUserRatingCaller = IUserRatingCaller;
        }
        #endregion

        #region SearchApiCall
       // [Authorize]
        [HttpGet]
        [Route("Search")]
        public async Task<List<FlightOffersSearchRs>> Search([FromQuery] FlightSearchQuery_v1 RequestQuery)
        {
            try
            {
                // _logger.LogDebug("response"+ response);
                return await _Amadeus_SearchRepositoryCaller.FlightOffersSearchock(RequestQuery);

            }
            catch (Exception)
            {
                /* Should have provide meaning full error that recvieved from supplier example : no results for that date
                 * Need create errorObj and added to the FlightOffersSearchRS
                 */
                return null;
            }
        }
        #endregion

        #region CacheApiCall
        [Authorize]
        [HttpGet]
        [Route("Cache/offer")]
        public async Task<FlightResponse_v1> Cache(string Key)
        {
            try
            {
                // _logger.LogDebug("response"+ response);
                return await _CacheCaller.GetOffer(Key);

            }
            catch (Exception)
            {
                /* Should have provide meaning full error that recvieved from supplier example : no results for that date
                 * Need create errorObj and added to the FlightOffersSearchRS
                 */
                return null;
            }
        }
        #endregion

        #region CSVOfferApiCall
        [Authorize]
        [HttpGet]
        [Produces("text/csv")]
        [Route("CSVOfferDownload")]
        public async Task<FileResult> OfferCSV([FromQuery] FlightSearchQuery_v1 RequestQuery)
        {
            try
            {
                Dictionary<string, string> List = new Dictionary<string, string>();
                var Csv = new StringBuilder();
                var Line = string.Format("{0},{1},{2},{3},{4},{5}", "Airline", "Equipment", "Version", "Cabin", "SeatWidth", "SeatPitch");
                Csv.AppendLine(Line);
                foreach (var item in await _Amadeus_SearchRepositoryCaller.FlightOffersSearch(RequestQuery))
                {
                    String KeyInbound = item.inbound.carrier.code + "/" + item.inbound.flight + "/" + "1" + "/" + item.inbound.@class;
                    String KeyOutbound = item.outbound.carrier.code + "/" + item.outbound.flight + "/" + "1" + "/" + item.outbound.@class;

                    Random Randomno = new Random();

                    if (!List.ContainsKey(KeyInbound))
                    {

                        var AirlineInbound = item.inbound.carrier.code;
                        var EquipmentInbound = item.inbound.flight;
                        var versionInbound = "1";
                        var CabinInbound = item.inbound.@class;
                        var SeatWidthInbound = Math.Round(Randomno.NextDouble() + Randomno.Next(17, 35));
                        var SeatPitchInbound = Math.Round(Randomno.NextDouble() + Randomno.Next(45, 75));

                        var newLineOne = string.Format("{0},{1},{2},{3},{4},{5}", AirlineInbound, EquipmentInbound, versionInbound, CabinInbound, SeatWidthInbound, SeatPitchInbound);
                        Csv.AppendLine(newLineOne);
                        List.Add(KeyInbound, "");
                    };

                    if (!List.ContainsKey(KeyOutbound))
                    {
                        var AirlineOutbound = item.outbound.carrier.code;
                        var EquipmentOutbound = item.outbound.flight;
                        var versionOutbound = "1";
                        var CabinOutbound = item.outbound.@class;
                        var SeatWidthOutbound = Math.Round(Randomno.NextDouble() + Randomno.Next(17, 45));
                        var SeatPitchOutbound = Math.Round(Randomno.NextDouble() + Randomno.Next(60, 85));

                        var newLineTwo = string.Format("{0},{1},{2},{3},{4},{5}", AirlineOutbound, EquipmentOutbound, versionOutbound, CabinOutbound, SeatWidthOutbound, SeatPitchOutbound);
                        Csv.AppendLine(newLineTwo);
                        List.Add(KeyOutbound, "");
                    }

                }

                string fileName = "SeatRatingRawData.csv";
                byte[] fileBytes = Encoding.ASCII.GetBytes(Csv.ToString());

                return File(fileBytes, "text/csv", fileName);

            }
            catch (Exception)
            {
                /* Should have provide meaning full error that recvieved from supplier example : no results for that date
                 * Need create errorObj and added to the FlightOffersSearchRS
                 */
                return null;
            }
        }
        #endregion

        #region OfferDataUpload
        [Authorize]
        [HttpPost]
        [Route("MockFileUpload")]
        public async Task<IActionResult> SeatRating(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            // full path to file in temp location
            var filePath = @"C:\FirstClassFlyer\SeatRatingRawData.csv";

            System.IO.File.Delete(filePath);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            return Ok(new { count = files.Count, size, filePath });
        }
        #endregion

        #region OfferDataRead
        [Authorize]
        [HttpGet]
        [Route("OfferDataRead")]
        public async Task<Dictionary<string, string>> SeatRatingData()
        {
            Dictionary<string, string>.KeyCollection AllKeys = Ratings.Keys;
            foreach (string Key in AllKeys)
            {
                Ratings.Remove(Key);
            }

            string[] lines = System.IO.File.ReadAllLines(@"C:\FirstClassFlyer\SeatRatingRawData.csv");
            int i = 0;
            foreach (string Line in lines)
            {

                if (Line != null)
                {
                    string[] KeyValues = Line.Split(',');
                    String Key = KeyValues[0] + "/" + KeyValues[1] + "/" + KeyValues[2] + "/" + KeyValues[3];
                    String value = KeyValues[4] + "/" + KeyValues[5];
                    if (!Ratings.ContainsKey(Key))
                    {
                        Ratings.Add(Key, value);
                    }
                    i++;
                }
            }
            return await Task.FromResult(Ratings);
        }
        #endregion

        #region Base64string
       // [Authorize]
        //[HttpGet]
        //[Route("ContextChecker")]
        //public async Task<Base64Weightings> ContextChecker(String Context)
        //{
        //    try
        //    {
        //        // _logger.LogDebug("response"+ response);
        //     //   return await _IUserRatingCaller.UserRating(Context);

        //    }
        //    catch (Exception)
        //    {
        //        /* Should have provide meaning full error that recvieved from supplier example : no results for that date
        //         * Need create errorObj and added to the FlightOffersSearchRS
        //         */
        //        return null;
        //    }
        //}
        #endregion

    }
}
