using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Offers.Models.Common
{

    /*
     * This is the model which Flights API call requested 
     */
    public class FlightSearchQuery_v3
    {
        [DefaultValue("2021-08-01")]
        [Required]
        public String DepartureDate { get; set; }
        [DefaultValue("2021-08-08")]
        public string ArrivalDate { get; set; }
        [DefaultValue("MIA")]
        [Required]
        public String OriginLocation { get; set; }
        [DefaultValue("LHR")]
        [Required]
        public String DestinationLocation { get; set; }
        [DefaultValue("1")]
        [Required]
        public int PaxAdult { get; set; }
        [DefaultValue("3")]
        public int DateFlexibility { get; set; }
        [DefaultValue("BB")]
        [Required]
        public string Class { get; set; }
        public string Context { get; set; }
        public string TransactionId { get; set; }
        [DefaultValue("IB")]
        [Required]
        public String  Airline { get; set; }
    }
}
