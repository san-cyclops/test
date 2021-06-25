using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Models.Common
{
    public class LocationsRs
    {
        public string id { get; set; }
        public string iata { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public List<Alternate> alternate { get; set; }
        public Country country { get; set; }
    }
    public class Alternate
    {
        public string iata { get; set; }
        public string name { get; set; }
    }

    public class Country
    {
        public string iata { get; set; }
        public string name { get; set; }
    }
}
