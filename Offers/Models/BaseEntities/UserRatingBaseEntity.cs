using Offers.Models.Common;
using Offers.Models.Common.Weightnings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Models.BaseEntities
{
    public class UserRatingBaseEntity 
    {
        public double UserRating { get; set; }
        public Base64Weightings UserWeightning { get; set; }
    }
}
