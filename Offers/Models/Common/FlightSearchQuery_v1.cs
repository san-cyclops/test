using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Offers.Models.Common
{

    /*
     * This is the model which Offers API call requested 
     */
    public class FlightSearchQuery_v1
    {
        [DefaultValue("2021-08-01")]
        [Required]
        public String DepartureDate { get; set; }
        [DefaultValue("2021-08-08")]
        [Required]
        public string ArrivalDate { get; set; }
        [DefaultValue("MIA")]
        [Required]
        public String DepatureLocation { get; set; }
        [DefaultValue("LHR")]
        [Required]
        public String ArrivalLocation { get; set; }
        [DefaultValue("1")]
        [Required]
        public int PaxAdult { get; set; }
        [DefaultValue("0")]
        public int DateFlexibility { get; set; }
        [DefaultValue("BB,PP,EE,FF")]
        [Required]
        public string Cabins { get; set; }
        public int PaxChild { get; set; }
        public string Context { get; set; }
        public string TransactionId { get; set; }
    }
}
