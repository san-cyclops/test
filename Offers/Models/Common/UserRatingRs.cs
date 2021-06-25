using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Models.Common
{
    public class UserRatingRs
    {
        public Double Value { get; set; }
        public List<ErrorRs> ErrorList { get; set; }
    }
}
