using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Offers.Models;
using Offers.Models.Amadeus.Search;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Repository.Connecter;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Offer;
using Offers.Services.Offers;
using Offers.Services.Seat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class OfferTest
    {
        public static List<FlightResponse_v2> Offerresult { get; set; }

        #region GetResponse
        public static async Task<List<FlightResponse_v2>> GetOfferResults()
        {
            var mockconfig = new Mock<IConfiguration>();
            IConfiguration MockConfig = mockconfig.Object;

            var mock = new Mock<ILogger<OfferService>>();
            ILogger<OfferService> MockLogger = mock.Object;

            var mockc = new Mock<IConnecterRepository<ConnecterBaseEntity>>();
            IConnecterRepository<ConnecterBaseEntity> MockConnector = mockc.Object;

            var mockcache = new Mock<ICacheService<CacheBaseEntity>>();
            ICacheService<CacheBaseEntity> MockCache = mockcache.Object;

            var mockst = new Mock<ISeatService<SeatBaseEntity>>();
            ISeatService<SeatBaseEntity> MockSeat = mockst.Object;

            var mocks = new Mock<IFileSaveService<FileSaveBaseEntity>>();
            IFileSaveService<FileSaveBaseEntity> MockFileSave = mocks.Object;

            var errors = new Mock<IErrorService<ErrorBaseEntity>>();
            IErrorService<ErrorBaseEntity> ErrorLog = errors.Object;

            var controller = new OfferService(MockSeat,MockLogger, MockCache,MockFileSave,ErrorLog);

            FlightSearchQuery_v1 flightSearchQuery_V1 = new FlightSearchQuery_v1();
            flightSearchQuery_V1.DepartureDate = "2021-04-23";
            flightSearchQuery_V1.ArrivalDate = "2021-04-24";
            flightSearchQuery_V1.DepatureLocation = "MIA";
            flightSearchQuery_V1.ArrivalLocation = "LHR";
            flightSearchQuery_V1.PaxAdult = 1;
            flightSearchQuery_V1.DateFlexibility = 0;
            flightSearchQuery_V1.Cabins = "BP";

            List<FlightResponse_v1> FlightResponse_v1List = new List<FlightResponse_v1>();
            FlightResponse_v1List = JsonConvert.DeserializeObject<List<FlightResponse_v1>>(File.ReadAllText(@"D:\OZEN\offer\New folder\_Cache-Offer-RS_2021_05_23T14_53_32.json"));
             
           
            Offerresult = await controller.RatingCalculation(FlightResponse_v1List, null, null);
            return Offerresult;
        }
        #endregion



        //OutBound

        #region OutBound_Duration_Validate_Values
        [Fact]
        public async void OutBound_Duration_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundDuration = "PT8H35M";
            var item = Offerresult.Select(p => p.outbound.duration).FirstOrDefault();
            Assert.Equal(OutBoundDuration.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_NumberOfStops_Validate_Values
        [Fact]
        public async void OutBound_NumberOfStops_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundNumberOfStops = "0";
            var item = Offerresult.Select(p => p.outbound.numberOfStops).FirstOrDefault();
            Assert.Equal(OutBoundNumberOfStops.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_flight_Validate_Values
        [Fact]
        public async void OutBound_Flight_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundFlight = "5506";
            var item = Offerresult.Select(p => p.outbound.number).FirstOrDefault();
            Assert.Equal(OutBoundFlight.ToString(), item.ToString());
        }
        #endregion


        #region OutBound_Departure_Code_Validate_Values
        [Fact]
        public async void OutBound_Departure_Code_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundDepartureCode = "MIA";
            var item = Offerresult.Select(p => p.outbound.departure.code).FirstOrDefault();
            Assert.Equal(OutBoundDepartureCode.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Departure_At_Validate_Values
        [Fact]
        public async void OutBound_Departure_At_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundDepartureAt = "2021-08-01T17:05:00";
            var item = Offerresult.Select(p => p.outbound.departure.at).FirstOrDefault();
            Assert.Equal(OutBoundDepartureAt.ToString(), item.ToString());
        }
        #endregion


        #region OutBound_Arrival_Code_Validate_Values
        [Fact]
        public async void OutBound_Arrival_Code_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundArrivalCode = "LHR";
            var item = Offerresult.Select(p => p.outbound.arrival.code).FirstOrDefault();
            Assert.Equal(OutBoundArrivalCode.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Arrival_At_Validate_Values
        [Fact]
        public async void OutBound_Arrival_At_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundArrivalAt = "2021-08-02T06:40:00";
            var item = Offerresult.Select(p => p.outbound.arrival.at).FirstOrDefault();
            Assert.Equal(OutBoundArrivalAt.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Carrier_Code_Validate_Values
        [Fact]
        public async void OutBound_Carrier_Code_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundCarrierCode = "AY";
            var item = Offerresult.Select(p => p.outbound.carrier.code).FirstOrDefault();
            Assert.Equal(OutBoundCarrierCode.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Carrier_Name_Validate_Values
        [Fact]
        public async void OutBound_Carrier_Name_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundCarrierName = "FINNAIR";
            var item = Offerresult.Select(p => p.outbound.carrier.name).FirstOrDefault();
            Assert.Equal(OutBoundCarrierName.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Operating_Code_Validate_Values
        [Fact]
        public async void OutBound_Operating_Code_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundOperatingCode = "BA";
            var item = Offerresult.Select(p => p.outbound.operating.code).FirstOrDefault();
            Assert.Equal(OutBoundOperatingCode.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Operating_Name_Validate_Values
        [Fact]
        public async void OutBound_Operating_Name_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundOperatingName = "BRITISH AIRWAYS";
            var item = Offerresult.Select(p => p.outbound.operating.name).FirstOrDefault();
            Assert.Equal(OutBoundOperatingName.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_Code_Validate_Values
        [Fact]
        public async void OutBound_Aircraft_Code_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundAircraftCode = "777";
            var item = Offerresult.Select(p => p.outbound.aircraft.code).FirstOrDefault();
            Assert.Equal(OutBoundAircraftCode.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_Cabin_Random_Validate_Values
        [Fact]
        public async void OutBound_Aircraft_Cabin_Random_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundAircraftCabinRandom = "false";
            var item = Offerresult.Select(p => p.outbound.aircraft.cabin.random).FirstOrDefault();
            Assert.Equal(OutBoundAircraftCabinRandom.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_Cabin_CabinClass_Validate_Values
        [Fact]
        public async void OutBound_Aircraft_Cabin_CabinClass_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundAircraftCabinCabinClass = "FIRST";
            var item = Offerresult.Select(p => p.outbound.aircraft.cabin.cabinClass).FirstOrDefault();
            Assert.Equal(OutBoundAircraftCabinCabinClass.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_Cabin_Amenities1_Name_Validate_Values
        [Fact]
        public async void OutBound_Aircraft_Cabin_Amenities1_Name_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundAircraftCabinAmenities1Name = "Seat Width";
            var item = Offerresult.Select(p => p.outbound.aircraft.cabin.amenities[0].name).First();
            Assert.Equal(OutBoundAircraftCabinAmenities1Name.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_Cabin_Amenities1_Unit_Validate_Values
        [Fact]
        public async void OutBound_Aircraft_Cabin_Amenities1_Unit_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundAircraftCabinAmenities1Unit = "inches";
            var item = Offerresult.Select(p => p.outbound.aircraft.cabin.amenities[0].unit).First();
            Assert.Equal(OutBoundAircraftCabinAmenities1Unit.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_Cabin_Amenities1_Value_Validate_Values
        [Fact]
        public async void OutBound_Aircraft_Cabin_Amenities1_Value_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundAircraftCabinAmenities1Value = "21";
            var item = Offerresult.Select(p => p.outbound.aircraft.cabin.amenities[0].value).First();
            Assert.Equal(OutBoundAircraftCabinAmenities1Value.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_Cabin_Amenities1_Rating_Validate_Values
        [Fact]
        public async void OutBound_Aircraft_Cabin_Amenities1_Rating_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundAircraftCabinAmenities1Rating = "4";
            var item = Offerresult.Select(p => p.outbound.aircraft.cabin.amenities[0].rating).First();
            Assert.Equal(OutBoundAircraftCabinAmenities1Rating.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_Cabin_Amenities1_Key_Validate_Values
        [Fact]
        public async void OutBound_Aircraft_Cabin_Amenities1_Key_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundAircraftCabinAmenities1Key = "seatWidth";
            var item = Offerresult.Select(p => p.outbound.aircraft.cabin.amenities[0].key).First();
            Assert.Equal(OutBoundAircraftCabinAmenities1Key.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_Cabin_Rating_Validate_Values
        [Fact]
        public async void OutBound_Aircraft_Cabin_Rating_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundAircraftCabinCabinRating = "4.5";
            var item = Offerresult.Select(p => p.outbound.aircraft.cabin.rating).FirstOrDefault();
            Assert.Equal(OutBoundAircraftCabinCabinRating.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_Cabin_Video_Validate_Values
        [Fact]
        public async void OutBound_Aircraft_Cabin_Video_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundAircraftCabinCabinVideo = "null";
            var item = Offerresult.Select(p => p.outbound.aircraft.cabin.video).FirstOrDefault();
            Assert.Equal(OutBoundAircraftCabinCabinVideo.ToString(), item.ToString());
        }
        #endregion


        //InBound

        #region InBound_Duration_Validate_Values
        [Fact]
        public async void InBound_Duration_Validate_Values()
        {
            await GetOfferResults();
            String InBoundDuration = "PT9H20M";
            var item = Offerresult.Select(p => p.inbound.duration).FirstOrDefault();
            Assert.Equal(InBoundDuration.ToString(), item.ToString());
        }
        #endregion

        #region InBound_NumberOfStops_Validate_Values
        [Fact]
        public async void InBound_NumberOfStops_Validate_Values()
        {
            await GetOfferResults();
            String InBoundNumberOfStops = "0";
            var item = Offerresult.Select(p => p.inbound.numberOfStops).FirstOrDefault();
            Assert.Equal(InBoundNumberOfStops.ToString(), item.ToString());
        }
        #endregion

        #region InBound_flight_Validate_Values
        [Fact]
        public async void InBound_Flight_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundFlight = "5509";
            var item = Offerresult.Select(p => p.inbound.number).FirstOrDefault();
            Assert.Equal(OutBoundFlight.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Departure_Code_Validate_Values
        [Fact]
        public async void InBound_Departure_Code_Validate_Values()
        {
            await GetOfferResults();
            String InBoundDepartureCode = "LHR";
            var item = Offerresult.Select(p => p.inbound.departure.code).FirstOrDefault();
            Assert.Equal(InBoundDepartureCode.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Departure_At_Validate_Values
        [Fact]
        public async void InBound_Departure_At_Validate_Values()
        {
            await GetOfferResults();
            String InBoundDepartureAt = "2021-08-08T14:00:00";
            var item = Offerresult.Select(p => p.inbound.departure.at).FirstOrDefault();
            Assert.Equal(InBoundDepartureAt.ToString(), item.ToString());
        }
        #endregion


        #region InBound_Arrival_Code_Validate_Values
        [Fact]
        public async void InBound_Arrival_Code_Validate_Values()
        {
            await GetOfferResults();
            String InBoundArrivalCode = "MIA";
            var item = Offerresult.Select(p => p.inbound.arrival.code).FirstOrDefault();
            Assert.Equal(InBoundArrivalCode.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Arrival_At_Validate_Values
        [Fact]
        public async void InBound_Arrival_At_Validate_Values()
        {
            await GetOfferResults();
            String InBoundArrivalAt = "2021-08-08T18:20:00";
            var item = Offerresult.Select(p => p.inbound.arrival.at).FirstOrDefault();
            Assert.Equal(InBoundArrivalAt.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Carrier_Code_Validate_Values
        [Fact]
        public async void InBound_Carrier_Code_Validate_Values()
        {
            await GetOfferResults();
            String InBoundCarrierCode = "AY";
            var item = Offerresult.Select(p => p.inbound.carrier.code).FirstOrDefault();
            Assert.Equal(InBoundCarrierCode.ToString(), item.ToString());
        }
        #endregion

        #region IInBound_Carrier_Name_Validate_Values
        [Fact]
        public async void InBound_Carrier_Name_Validate_Values()
        {
            await GetOfferResults();
            String InBoundCarrierName = "FINNAIR";
            var item = Offerresult.Select(p => p.inbound.carrier.name).FirstOrDefault();
            Assert.Equal(InBoundCarrierName.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Operating_Code_Validate_Values
        [Fact]
        public async void InBound_Operating_Code_Validate_Values()
        {
            await GetOfferResults();
            String InBoundOperatingCode = "BA";
            var item = Offerresult.Select(p => p.inbound.operating.code).FirstOrDefault();
            Assert.Equal(InBoundOperatingCode.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Operating_Name_Validate_Values
        [Fact]
        public async void InBound_Operating_Name_Validate_Values()
        {
            await GetOfferResults();
            String InBoundOperatingName = "BRITISH AIRWAYS";
            var item = Offerresult.Select(p => p.inbound.operating.name).FirstOrDefault();
            Assert.Equal(InBoundOperatingName.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Operating_Code_Validate_Values
        [Fact]
        public async void InBound_Aircraft_Code_Validate_Values()
        {
            await GetOfferResults();
            String InBoundAircraftCode = "388";
            var item = Offerresult.Select(p => p.inbound.aircraft.code).FirstOrDefault();
            Assert.Equal(InBoundAircraftCode.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_Cabin_Random_Validate_Values
        [Fact]
        public async void InBound_Aircraft_Cabin_Random_Validate_Values()
        {
            await GetOfferResults();
            String InBoundAircraftCabinRandom = "false";
            var item = Offerresult.Select(p => p.inbound.aircraft.cabin.random).FirstOrDefault();
            Assert.Equal(InBoundAircraftCabinRandom.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Aircraft_Cabin_CabinClass_Validate_Values
        [Fact]
        public async void InBound_Aircraft_Cabin_CabinClass_Validate_Values()
        {
            await GetOfferResults();
            String InBoundAircraftCabinCabinClass = "FIRST";
            var item = Offerresult.Select(p => p.inbound.aircraft.cabin.cabinClass).FirstOrDefault();
            Assert.Equal(InBoundAircraftCabinCabinClass.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Aircraft_Cabin_Amenities1_Name_Validate_Values
        [Fact]
        public async void InBound_Aircraft_Cabin_Amenities1_Name_Validate_Values()
        {
            await GetOfferResults();
            String InBoundAircraftCabinAmenities1Name = "Seat Width";
            var item = Offerresult.Select(p => p.inbound.aircraft.cabin.amenities[0].name).First();
            Assert.Equal(InBoundAircraftCabinAmenities1Name.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Aircraft_Cabin_Amenities1_Unit_Validate_Values
        [Fact]
        public async void InBound_Aircraft_Cabin_Amenities1_Unit_Validate_Values()
        {
            await GetOfferResults();
            String InBoundAircraftCabinAmenities1Unit = "inches";
            var item = Offerresult.Select(p => p.inbound.aircraft.cabin.amenities[0].unit).First();
            Assert.Equal(InBoundAircraftCabinAmenities1Unit.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Aircraft_Cabin_Amenities1_Value_Validate_Values
        [Fact]
        public async void InBound_Aircraft_Cabin_Amenities1_Value_Validate_Values()
        {
            await GetOfferResults();
            String InBoundAircraftCabinAmenities1Value = "22.5";
            var item = Offerresult.Select(p => p.inbound.aircraft.cabin.amenities[0].value).First();
            Assert.Equal(InBoundAircraftCabinAmenities1Value.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Aircraft_Cabin_Amenities1_Rating_Validate_Values
        [Fact]
        public async void InBound_Aircraft_Cabin_Amenities1_Rating_Validate_Values()
        {
            await GetOfferResults();
            String InBoundAircraftCabinAmenities1Rating = "4.25";
            var item = Offerresult.Select(p => p.inbound.aircraft.cabin.amenities[0].rating).First();
            Assert.Equal(InBoundAircraftCabinAmenities1Rating.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Aircraft_Cabin_Amenities1_Key_Validate_Values
        [Fact]
        public async void InBound_Aircraft_Cabin_Amenities1_Key_Validate_Values()
        {
            await GetOfferResults();
            String InBoundAircraftCabinAmenities1Key = "seatWidth";
            var item = Offerresult.Select(p => p.inbound.aircraft.cabin.amenities[0].key).First();
            Assert.Equal(InBoundAircraftCabinAmenities1Key.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Aircraft_Cabin_Rating_Validate_Values
        [Fact]
        public async void InBound_Aircraft_Cabin_Rating_Validate_Values()
        {
            await GetOfferResults();
            String OutBoundAircraftCabinCabinRating = "4.625";
            var item = Offerresult.Select(p => p.inbound.aircraft.cabin.rating).FirstOrDefault();
            Assert.Equal(OutBoundAircraftCabinCabinRating.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Aircraft_Cabin_Video_Validate_Values
        [Fact]
        public async void InBound_Aircraft_Cabin_Video_Validate_Values()
        {
            await GetOfferResults();
            String InBoundAircraftCabinCabinVideo = "'https://www.youtube.com/watch?v=g5fSLrVzpxg&feature=emb_rel_pause'";
            var item = Offerresult.Select(p => p.inbound.aircraft.cabin.video).FirstOrDefault();
            Assert.Equal(InBoundAircraftCabinCabinVideo.ToString(), item.ToString());
        }
        #endregion

        #region Cost__Validate_Values
        [Fact]
        public async void Cost__Validate_Values()
        {
            await GetOfferResults();
            String Cost = "20026.25";
            var item = Offerresult.Select(p => p.cost).FirstOrDefault();
            Assert.Equal(Cost.ToString(), item.ToString());
        }
        #endregion



        /*#region Response_Not_Null
        [Fact]
        public void Outbound()
        {
            Assert.NotNull("");

            //outbound
            
            //departure
            Assert.Equal("code", "");
            Assert.Equal("at", "");

            //arrival
            Assert.Equal("code", "");
            Assert.Equal("at", "");

            //carrier
            Assert.Equal("code", "");
            Assert.Equal("name", "");

            //operating
            Assert.Equal("code", "");
            Assert.Equal("name", "");

            //aircraft
            Assert.Equal("code", "");
            Assert.Equal("name", "");

            //cabin
            Assert.Equal("random", "");
            Assert.Equal("seatWidth", "");
            Assert.Equal("seatRecline", "");
            Assert.Equal("seatPitch", "");
            Assert.Equal("videoType", "");
            Assert.Equal("laptopPower", "");
            Assert.Equal("wiFi", "");
            Assert.Equal("seatType", "");
            Assert.Equal("seatPrivacy", "");
            Assert.Equal("bedLenth", "");

            //Number
            Assert.Equal("code", "");

            //Number
            Assert.Equal("rating", "");

        }
        #endregion*/

    }
}
