using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Offers.Models;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Repository.Connecter;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Flight;
using Offers.Services.Flights;
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
    public class FlighTest
    {

        public static FlightResponse_v4 Flightresult { get; set; }

        #region GetResponse
        public static async Task<FlightResponse_v4> GetFlightData()
        {
            var mock = new Mock<ILogger<OfferService>>();
            ILogger<OfferService> MockLogger = mock.Object;

            var mockcache = new Mock<ICacheService<CacheBaseEntity>>();
            ICacheService<CacheBaseEntity> MockCache = mockcache.Object;

            var mocks = new Mock<IFileSaveService<FileSaveBaseEntity>>();
            IFileSaveService<FileSaveBaseEntity> MockFileSave = mocks.Object;

            var errors = new Mock<IErrorService<ErrorBaseEntity>>();
            IErrorService<ErrorBaseEntity> ErrorLog = errors.Object;

            var offers = new Mock<IOfferService<OfferBaseEntity>>();
            IOfferService<OfferBaseEntity> OffersCaller = offers.Object;
            

            var seatData = new Mock<ISeatService<SeatBaseEntity>>();
            ISeatService<SeatBaseEntity> SeatData = seatData.Object;

            var controller = new FlightService(OffersCaller, MockLogger,MockFileSave,ErrorLog, MockCache, SeatData);


            FlightResponse_v3 FlightResponse_v3Obj = new FlightResponse_v3();

           // FlightResp6onse_v3Obj = JsonConvert.DeserializeObject<FlightResponse_v3>(File.ReadAllText(@"C:\SystemLogs\Test\_Cache-Flight-RS_2021_05_24T04_23_16.json"));

            Flightresult = await controller.ResponseRatingCalculation(FlightResponse_v3Obj, "","");

            return Flightresult;
        }
        #endregion


        #region OutBound_Duration
        [Fact]
        public async void OutBound_Duration()
        {
            await GetFlightData();
            String OutBoundDuration = "PT9H35M";
            var item = Flightresult.duration;
            Assert.Equal(OutBoundDuration.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Departure_Code
        [Fact]
        public async void OutBound_Departure_Code()
        {
            await GetFlightData();
            String OutBoundDeparture_Code = "LHR";
            var item = Flightresult.departure.code;
            Assert.Equal(OutBoundDeparture_Code.ToString(),item.ToString());
        }
        #endregion
        #region OutBound_Departure_Name
        [Fact]
        public async void OutBound_Departure_Name()
        {
            await GetFlightData();
            String OutBoundDeparture_Name = "";
            var item = Flightresult.departure.name;
            Assert.Equal(OutBoundDeparture_Name.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Departure_City
        [Fact]
        public async void OutBound_Departure_City()
        {
            await GetFlightData();
            String OutBoundDeparture_City = "";
            var item = Flightresult.departure.city;
            Assert.Equal(OutBoundDeparture_City.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Departure_City
        [Fact]
        public async void OutBound_Departure_Country()
        {
            await GetFlightData();
            String OutBoundDeparture_Country = "";
            var item = Flightresult.departure.country;
            Assert.Equal(OutBoundDeparture_Country.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Departure_At
        [Fact]
        public async void OutBound_Departure_At()
        {
            await GetFlightData();
            String OutBoundDeparture_At = "2021-08-08T12:50:00";
            var item = Flightresult.departure.at;
            Assert.Equal(OutBoundDeparture_At.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Arrival_Code
        [Fact]
        public async void OutBound_Arrival_Code()
        {
            await GetFlightData();
            String OutBoundArrival_Code = "MIA";
            var item = Flightresult.arrival.code;
            Assert.Equal(OutBoundArrival_Code.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Arrival_Name
        [Fact]
        public async void OutBound_Arrival_Name()
        {
            await GetFlightData();
            String OutBoundArrival_Name = "";
            var item = Flightresult.arrival.name;
            Assert.Equal(OutBoundArrival_Name.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Arrival_City
        [Fact]
        public async void OutBound_Arrival_City()
        {
            await GetFlightData();
            String OutBoundArrival_City = "";
            var item = Flightresult.arrival.city;
            Assert.Equal(OutBoundArrival_City.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Arrival_City
        [Fact]
        public async void OutBound_Arrival_Country()
        {
            await GetFlightData();
            String OutBoundArrival_Country = "";
            var item = Flightresult.arrival.country;
            Assert.Equal(OutBoundArrival_Country.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Arrival_At
        [Fact]
        public async void OutBound_Arrival_At()
        {
            await GetFlightData();
            String OutBoundArrival_At = "2021-08-08T17:25:00";
            var item = Flightresult.arrival.at;
            Assert.Equal(OutBoundArrival_At.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Carrier_Code
        [Fact]
        public async void OutBound_Carrier_Code()
        {
            await GetFlightData();
            String OutBoundcarrier_Code = "IB";
            var item = Flightresult.carrier.code;
            Assert.Equal(OutBoundcarrier_Code.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Carrier_Name
        [Fact]
        public async void OutBound_Carrier_Name()
        {
            await GetFlightData();
            String OutBoundCarrier_Name = "IBERIA";
            var item = Flightresult.carrier.name;
            Assert.Equal(OutBoundCarrier_Name.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Operating_Code
        [Fact]
        public async void OutBound_Operating_Code()
        {
            await GetFlightData();
            String OutBoundOperating_Code = "AA";
            var item = Flightresult.operating.code;
            Assert.Equal(OutBoundOperating_Code.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Operating_Name
        [Fact]
        public async void OutBound_Operating_Name()
        {
            await GetFlightData();
            String OutBoundOperating_Name = "AMERICAN AIRLINES";
            var item = Flightresult.operating.name;
            Assert.Equal(OutBoundOperating_Name.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Operating_Code
        [Fact]
        public async void OutBound_Aircraft_Code()
        {
            await GetFlightData();
            String OutBoundAircraft_Code = "77W";
            var item = Flightresult.aircraft.code;
            Assert.Equal(OutBoundAircraft_Code.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Aircraft_Name
        [Fact]
        public async void OutBound_Aircraft_Name()
        {
            await GetFlightData();
            String OutBoundAircraft_Name = "BOEING 777-300ER";
            var item = Flightresult.aircraft.name;
            Assert.Equal(OutBoundAircraft_Name.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_cabin_cabinClass
        [Fact]
        public async void OutBound_Aircraft_cabin_cabinClass()
        {
            await GetFlightData();
            String OutBoundAircraft_cabin_cabinClass = "BUSINESS";
            var item = Flightresult.aircraft.cabin.cabinClass;
            Assert.Equal(OutBoundAircraft_cabin_cabinClass.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Aircraft_cabin_cabinClass
        [Fact]
        public async void OutBound_Aircraft_cabin_pictures()
        {
            await GetFlightData();
            String OutBoundAircraft_cabin_pictures_0 = "https://dl.airtable.com/.attachments/200f31dc09b5c024de1042a6d9c88ba7/038a973a/Aircraft-Interiors-AA777-Business-Class-Seat-Reclined.jpg";
            var item = Flightresult.aircraft.cabin.pictures[0].ToString();
            Assert.Equal(OutBoundAircraft_cabin_pictures_0.ToString(), item.ToString());
        }
        #endregion


        #region OutBound_numberOfStops
        [Fact]
        public async void OutBound_numberOfStops()
        {
            await GetFlightData();
            String OutBoundnumberOfStops = "0";
            var item = Flightresult.numberOfStops;
            Assert.Equal(OutBoundnumberOfStops.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_number
        [Fact]
        public async void OutBound_number()
        {
            await GetFlightData();
            String OutBoundnumber = "4228";
            var item = Flightresult.number;
            Assert.Equal(OutBoundnumber.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_number
        [Fact]
        public async void OutBound_cost()
        {
            await GetFlightData();
            String OutBoundcost = "2719.25";
            var item = Flightresult.cost;
            Assert.Equal(OutBoundcost.ToString(), item.ToString());
        }
        #endregion
       
    }
}
