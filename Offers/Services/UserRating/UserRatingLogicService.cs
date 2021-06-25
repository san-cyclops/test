using Microsoft.Extensions.Logging;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Models.Common.Weightnings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.UserRating
{
    public class UserRatingLogicService : IUserRatingLogicService <UserRatingBaseEntity>
    {
        private ILogger<UserRatingLogicService> _logger;

        public UserRatingLogicService(ILogger<UserRatingLogicService> Logger)
        {
            _logger = Logger;
        }
        public Task<double> UserRatingCalculation(Ratings Ratings, Base64Weightings Base64Weightings)
        {
            try
            {
                double UserRating = 0.0;
                double TotalWeightnings = (Base64Weightings.weightings.outbound.seatWidth + Base64Weightings.weightings.outbound.seatPitch + Base64Weightings.weightings.outbound.seatPrivacy + Base64Weightings.weightings.outbound.seatRecline);

                if (Ratings.seatPitch > 0)
                {
                    UserRating = (
                                     (Base64Weightings.weightings.outbound.seatWidth * Ratings.seatWidth) +
                                     (Base64Weightings.weightings.outbound.seatPitch * Ratings.seatPitch) +
                                     (Base64Weightings.weightings.outbound.seatPrivacy * Ratings.seatPrivacy) +
                                     (Base64Weightings.weightings.outbound.seatRecline * Ratings.seatRecline)) / TotalWeightnings;


                }
                else
                {
                    UserRating = (
                                    (Base64Weightings.weightings.outbound.seatWidth * Ratings.seatWidth) +
                                    (Base64Weightings.weightings.outbound.seatPitch * Ratings.bedLenth) +
                                    (Base64Weightings.weightings.outbound.seatPrivacy * Ratings.seatPrivacy) +
                                    (Base64Weightings.weightings.outbound.seatRecline * Ratings.seatRecline)) / TotalWeightnings;
                }
                   
                 return Task.FromResult(Math.Round(UserRating,1));
                
            }
            catch (Exception ErrorMessage)
            {
                _logger.LogError("Exeception in UserRatingCalulation ");
                _logger.LogError(ErrorMessage.StackTrace);
                return null;
            }
        }
    }
}
