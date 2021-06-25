using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Offers.Models.Common
{

    /*
     * This is the model which Offers API call requested 
     */
    public class FlightSearchQuery_v2
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
        public string TransactionId { get; set; }
        [DefaultValue("0")]
        [Required]
        public string BaselineOffer { get; set; }
        public string  Quadrant { get; set; }

        [DefaultValue("ewogICJ3ZWlnaHRpbmdzIjogewogICAgIm91dGJvdW5kIjogewogICAgICAic2VhdFdpZHRoIjogNSwKICAgICAgInNlYXRQaXRjaCI6IDUsCiAgICAgICJzZWF0VHlwZSI6IDUsCiAgICAgICJzZWF0UHJpdmFjeSI6IDUKICAgIH0sCiAgICAiaW5ib3VuZCI6IHsKICAgICAgInNlYXRXaWR0aCI6IDUsCiAgICAgICJzZWF0UGl0Y2giOiA1LAogICAgICAic2VhdFR5cGUiOiA1LAogICAgICAic2VhdFByaXZhY3kiOiA1CiAgICB9CiAgfSwKICAiYWlybGluZVBsYW5zIjogWyJyZWN4M3g2T0tBbzQwNk56byIsICJyZWNBRnJwY3VLbVBtMnh6SSJdCn0=")]
        [Required]
        public string Context { get; set; }
    }
}
