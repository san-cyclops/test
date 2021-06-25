using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Models.Common
{
    public class QuadrantsRS
    {
        public SignalsRs_v1 overview { get; set; }
        public SignalsRs_v1 betterthanfree { get; set; }
        public SignalsRs_v1 freeupgrades { get; set; }
        public SignalsRs_v1 paidupgrades { get; set; }
        public SignalsRs_v1 savingsdowngrades { get; set; }
        public SignalsRs_v1 trash { get; set; }
        public List<ErrorRs> ErrorList { get; set; }
    }

    public class BaseOfferRs
    {
        public string BaseOffer { get; set; }
        public string Origin { get; set; }
        public List<ErrorRs> ErrorList { get; set; }
    }
}
