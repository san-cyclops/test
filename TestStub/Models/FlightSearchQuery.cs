using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TestStub
{
    public class FlightSearchQuery
    {

        [DefaultValue("2021-04-01")]
        [Required]
        public String DepartureDate { get; set; }
        [DefaultValue("2021-04-08")]
        [Required]
        public string ArrivalDate { get; set; }
        [DefaultValue("MIA")]
        [Required]
        public String DepatureLocation { get; set; }
        [DefaultValue("LHR")]
        [Required]
        public String ArrivalLocation { get; set; }
        [DefaultValue("true")]
        [Required]
        public bool MixedCabin { get; set; }
        [DefaultValue("1")]
        [Required]
        public int PaxAdult { get; set; }
        [DefaultValue("1")]
        public int DateFlexibility { get; set; }
        [DefaultValue("BUS")]
        [Required]
        public string[] MixedCabinFlexibility { get; set; }

        public int PaxChild { get; set; }

    }
}
