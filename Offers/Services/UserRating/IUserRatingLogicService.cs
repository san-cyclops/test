using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Models.Common.Weightnings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.UserRating
{
    public interface IUserRatingLogicService <T> where T : UserRatingBaseEntity
    {
        Task<double> UserRatingCalculation(Ratings Ratings, Base64Weightings Base64Weightings);


    }
}
