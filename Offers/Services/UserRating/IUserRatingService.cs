using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Models.Common.Weightnings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.UserRating
{
    public interface IUserRatingService<T> where T : UserRatingBaseEntity
    {
        public Task<UserRatingRs> UserRating(Ratings Rating,string Context, String TransactionID);
    }
}
