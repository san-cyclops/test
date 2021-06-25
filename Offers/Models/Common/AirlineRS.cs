using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Models.Common
{
    public class Airlines
    {
        public string iata { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
    }

    public class AirlineRS
    {
        public string id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public List<Airlines> airlines { get; set; }
    }

}
