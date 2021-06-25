using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Models.Common.Weightnings;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Offers.Services.UserRating
{
    public class UserRatingService : IUserRatingService<UserRatingBaseEntity>
    {
        #region UserRatingServiceFields
        private readonly ICacheService<CacheBaseEntity> _CacheCaller;
        private readonly ILogger<UserRatingService> _Logger;
        private readonly IFileSaveService<FileSaveBaseEntity> _IFileSave;
        private readonly IUserRatingLogicService<UserRatingBaseEntity> _UserRatingCaller;
        private readonly IErrorService<ErrorBaseEntity> _ErrorLog;
        #endregion

        #region UserRatingServiceConstuctor
        public UserRatingService(ICacheService<CacheBaseEntity> CacheCaller, ILogger<UserRatingService> Logger, IFileSaveService<FileSaveBaseEntity> IFileSave,IUserRatingLogicService<UserRatingBaseEntity> UserRatingCaller, IErrorService<ErrorBaseEntity> ErrorLogCaller)
        {
            _CacheCaller = CacheCaller;
            _Logger = Logger;
            _IFileSave = IFileSave;
            _UserRatingCaller = UserRatingCaller;
            _ErrorLog = ErrorLogCaller;
        }
        #endregion

        #region UserRatingServiceBasecall
        public async Task<UserRatingRs> UserRating(Ratings Rating, string Context,String TransactionID)
        {
            try
            {
                Base64Weightings Base64WeightingsObj = new Base64Weightings();
                Base64WeightingsObj = await UserRatingSignalsAsync(Context, TransactionID);
                double UserRatingValue= await _UserRatingCaller.UserRatingCalculation(Rating, Base64WeightingsObj);
                UserRatingRs UserRatingRsObj = new UserRatingRs();
                UserRatingRsObj.Value = UserRatingValue;
                return await Task.FromResult(UserRatingRsObj);
            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation("Error in Context = "+Context);
                _Logger.LogError(ErrorMessage.StackTrace);
                
                UserRatingRs UserRatingRsObj = new UserRatingRs();
                UserRatingRsObj.ErrorList = new List<ErrorRs>();
                UserRatingRsObj.ErrorList.Add(await _ErrorLog.ErrorLog("206", "UserRating Error" + ErrorMessage.Message)); 
                return await Task.FromResult(UserRatingRsObj);
            }
          
        }
        #endregion

        public async Task<Base64Weightings> UserRatingSignalsAsync(String Context, String TransactionID)
        {
            await _IFileSave.Save_v1(Context,"Userweightning-RQ",TransactionID);
            try
            {
                Base64Weightings Weightings = await ContextDecoder(Context, TransactionID);
                await _IFileSave.Save_v1(Weightings, "Userweightning-RS", TransactionID);
                return Weightings;
            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation("Error caught in UserRatingSignalsAsync Context Decoding");
                _Logger.LogError(ErrorMessage.StackTrace);

                Base64Weightings Weightings = new Base64Weightings();
                Weightings.ErrorList = new List<ErrorRs>();
                Weightings.ErrorList.Add(await _ErrorLog.ErrorLog("206", "Error caught in UserRatingSignalsAsync Context Decoding"));
                return Weightings;
            }
        }

        public async Task<Base64Weightings> ContextDecoder(String Context, String TransactionID)
        {
            try
            {
                await _IFileSave.Save_v1(Context, "ContextRQ", TransactionID);

                Base64Weightings WeightingsReturnObj = new Base64Weightings();
                byte[] Data = Convert.FromBase64String(Context);
                string DecodedString = Encoding.UTF8.GetString(Data);
                WeightingsReturnObj = JsonConvert.DeserializeObject<Base64Weightings>(DecodedString);


                await _IFileSave.Save_v1(WeightingsReturnObj, "ContextRS", TransactionID);

                return await Task.FromResult(WeightingsReturnObj);
            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation("Error caught in Context Decoding ");
                _Logger.LogError(ErrorMessage.StackTrace);

                Base64Weightings Weightings = new Base64Weightings();
                Weightings.ErrorList = new List<ErrorRs>();
                Weightings.ErrorList.Add(await _ErrorLog.ErrorLog("206", "Error caught in Context Decoding"));

                return Weightings;
            }
        }

      
    }
}
