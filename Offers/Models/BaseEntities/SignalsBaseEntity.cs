using Offers.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Models.BaseEntities
{
    public class SignalsBaseEntity
    {
        public String BaselineOffer { get; set; }
        public SignalsRs_v1 SignalsRs { get; set; }
        public List<FlightResponse_v1> FlightResponse_v1List { get; set; }
    }
}
