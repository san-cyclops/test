using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Offers.Models;
using Offers.Models.Amadeus.Search;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Repository.Amadeus.Authorization;
using Offers.Repository.Connecter;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Flights;
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
    public class FlighsTest
    {

        public static List<FlightResponse_v3> Flightresult { get; set; }

        #region GetResponse
        public static async Task<List<FlightResponse_v3>> GetFlightAsync()
        {

            var offers = new Mock<IOffersService<OffersBaseEntity>>();
            IOffersService<OffersBaseEntity> OffersCaller = offers.Object;

            var mock = new Mock<ILogger<FlightsService>>();
            ILogger<FlightsService> MockLogger = mock.Object;

            var mockOfferLog = new Mock<ILogger<OffersService>>();
            ILogger<OffersService> MockOfferLog = mockOfferLog.Object;

            var mockcache = new Mock<ICacheService<CacheBaseEntity>>();
            ICacheService<CacheBaseEntity> MockCache = mockcache.Object;

            var mocks = new Mock<IFileSaveService<FileSaveBaseEntity>>();
            IFileSaveService<FileSaveBaseEntity> MockFileSave = mocks.Object;

            var errors = new Mock<IErrorService<ErrorBaseEntity>>();
            IErrorService<ErrorBaseEntity> ErrorLog = errors.Object;

            var mockc = new Mock<IConnecterRepository<ConnecterBaseEntity>>();
            IConnecterRepository<ConnecterBaseEntity> MockConnector = mockc.Object;

            var mockst = new Mock<ISeatService<SeatBaseEntity>>();
            ISeatService<SeatBaseEntity> MockSeat = mockst.Object;

            var mockconfig = new Mock<IConfiguration>();
           


            //IConfiguration MockConfig = mockconfig.Object;

            //var inMemorySettings = new Dictionary<string, string> {
            //    {"AmadeusConfiguration:Environment", "Test"},
            //    {"AmadeusConfiguration:Prod:BaseUri", "https://api.amadeus.com/v2/shopping/flight-offers"},
            //    {"AmadeusConfiguration:Prod:ClientId", "A1oh7UlaWAh0tE6g0j8IJH4G8yVuP4zF"},
            //    {"AmadeusConfiguration:Prod:ClientSecret", "MCQKDttOwHH0Kdgv"},
            //    {"AmadeusConfiguration:Prod:GrantType", "client_credentials"},
            //    //...populate as needed for the test
            //};

            //IConfiguration configuration = new ConfigurationBuilder()
            //    .AddInMemoryCollection(inMemorySettings)
            //    .Build();



            var FlightsServiceControler = new FlightsService(OffersCaller, MockFileSave, MockLogger, MockCache, ErrorLog); 

            //var OffersServiceControler = new OffersService(MockFileSave, MockConnector, MockConfig, MockOfferLog, MockCache, MockSeat, ErrorLog);
               

            FlightSearchQuery_v3 flightSearchQuery_V3Obj = new FlightSearchQuery_v3();
            flightSearchQuery_V3Obj.DepartureDate = "2021-08-01";
            flightSearchQuery_V3Obj.ArrivalDate = "2021-08-08";
            flightSearchQuery_V3Obj.OriginLocation = "MIA";
            flightSearchQuery_V3Obj.DestinationLocation = "LHR";
            flightSearchQuery_V3Obj.Class = "BB,EE,FF,PP";
            flightSearchQuery_V3Obj.PaxAdult = 1;
            flightSearchQuery_V3Obj.DateFlexibility = 3;
            flightSearchQuery_V3Obj.Airline = "AA";
            flightSearchQuery_V3Obj.TransactionId = "100000111111111";
             
            List<FlightResponse_v1> FlightResponse_v1List = new List<FlightResponse_v1>();
           
            FlightResponse_v1List = JsonConvert.DeserializeObject<List<FlightResponse_v1>>(File.ReadAllText(@"C:\SystemLogs\Test\_BB_MIA_LHR_Offfers-RS_2021_05_22T18_12_20.json"));

            Flightresult = await FlightsServiceControler.FlightsResponseBuilderV1(FlightResponse_v1List, flightSearchQuery_V3Obj);

           

       

           

            return Flightresult;



            //var mockLoggerF = new Mock<ILogger<ConnecterRepository>>();
            //ILogger<ConnecterRepository> MockLoggerFS = mockLoggerF.Object;

            //var mockAuths = new Mock<IAuthorizationRepository>();
            //IAuthorizationRepository mockAuth = mockAuths.Object;

            //var connectoramedus  =new  ConnecterRepository(MockLoggerFS, mockAuth, configuration, MockFileSave, ErrorLog);

            //FlightOffersSearchRs FlightOffersSearchRsObj = await connectoramedus.GetOffers(FlightSearchRQObj,"UnitTest");

            //List<FlightResponse_v1> FlightResponse_v1Obj = await  OffersServiceControler.FlightResponseBuilderV1(FlightOffersSearchRsObj);


            //Flightresult = await FlightsServiceControler.FlightsResponseBuilderV1(FlightResponse_v1Obj, flightSearchQuery_V3Obj);




            //Mock<IOffersService<OffersBaseEntity>> mockFileIOAsync = new Mock<IOffersService<OffersBaseEntity>>();

            //mockFileIOAsync.Setup(t => t.FlightOffersSearch(FlightSearchQuery_v1)).ReturnsAsync(FlightResponse_v1List);

            //FlightsService fileWordCounter = new FlightsService(mockFileIOAsync.Object, MockFileSave, MockLogger, MockCache, ErrorLog);

            //var actualCount = await fileWordCounter.FlightsSearchBaseCaller(flightSearchQuery_V3Obj);

             
        }
        #endregion


        #region OutBound_Duration
        [Fact]
        public async void OutBound_Duration()
        {
            await GetFlightAsync();
            String OutBoundDuration = "PT8H50M";
            var item = Flightresult.Select(p => p.duration).FirstOrDefault();
            Assert.Equal(OutBoundDuration.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Departure_Code
        [Fact]
        public async void OutBound_Departure_Code()
        {
            await GetFlightAsync();
            String OutBoundDeparture_Code = "MIA";
            var item = Flightresult.Select(p => p.departure.code).FirstOrDefault();
            Assert.Equal(OutBoundDeparture_Code.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Departure_Name
        [Fact]
        public async void OutBound_Departure_Name()
        {
            await GetFlightAsync();
            String OutBoundDeparture_Name = "";
            var item = Flightresult.Select(p => p.departure.name).FirstOrDefault();
            Assert.Equal(OutBoundDeparture_Name.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Departure_City
        [Fact]
        public async void OutBound_Departure_City()
        {
            await GetFlightAsync();
            String OutBoundDeparture_City = "";
            var item = Flightresult.Select(p => p.departure.city).FirstOrDefault();
            Assert.Equal(OutBoundDeparture_City.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Departure_City
        [Fact]
        public async void OutBound_Departure_Country()
        {
            await GetFlightAsync();
            String OutBoundDeparture_Country = "";
            var item = Flightresult.Select(p => p.departure.country).FirstOrDefault();
            Assert.Equal(OutBoundDeparture_Country.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Departure_At
        [Fact]
        public async void OutBound_Departure_At()
        {
            await GetFlightAsync();
            String OutBoundDeparture_At = "2021-08-01T19:35:00";
            var item = Flightresult.Select(p => p.departure.at).FirstOrDefault();
            Assert.Equal(OutBoundDeparture_At.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Arrival_Code
        [Fact]
        public async void OutBound_Arrival_Code()
        {
            await GetFlightAsync();
            String OutBoundArrival_Code = "LHR";
            var item = Flightresult.Select(p => p.arrival.code).FirstOrDefault();
            Assert.Equal(OutBoundArrival_Code.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Arrival_Name
        [Fact]
        public async void OutBound_Arrival_Name()
        {
            await GetFlightAsync();
            String OutBoundArrival_Name = "";
            var item = Flightresult.Select(p => p.arrival.name).FirstOrDefault();
            Assert.Equal(OutBoundArrival_Name.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Arrival_City
        [Fact]
        public async void OutBound_Arrival_City()
        {
            await GetFlightAsync();
            String OutBoundArrival_City = "";
            var item = Flightresult.Select(p => p.arrival.city).FirstOrDefault();
            Assert.Equal(OutBoundArrival_City.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Arrival_City
        [Fact]
        public async void OutBound_Arrival_Country()
        {
            await GetFlightAsync();
            String OutBoundArrival_Country = "";
            var item = Flightresult.Select(p => p.arrival.country).FirstOrDefault();
            Assert.Equal(OutBoundArrival_Country.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Arrival_At
        [Fact]
        public async void OutBound_Arrival_At()
        {
            await GetFlightAsync();
            String OutBoundArrival_At = "2021-08-02T09:25:00";
            var item = Flightresult.Select(p => p.arrival.at).FirstOrDefault();
            Assert.Equal(OutBoundArrival_At.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Carrier_Code
        [Fact]
        public async void OutBound_Carrier_Code()
        {
            await GetFlightAsync();
            String OutBoundcarrier_Code = "AY";
            var item = Flightresult.Select(p => p.carrier.code).FirstOrDefault();
            Assert.Equal(OutBoundcarrier_Code.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Carrier_Name
        [Fact]
        public async void OutBound_Carrier_Name()
        {
            await GetFlightAsync();
            String OutBoundCarrier_Name = "FINNAIR";
            var item = Flightresult.Select(p => p.carrier.name).FirstOrDefault();
            Assert.Equal(OutBoundCarrier_Name.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Operating_Code
        [Fact]
        public async void OutBound_Operating_Code()
        {
            await GetFlightAsync();
            String OutBoundOperating_Code = "AA";
            var item = Flightresult.Select(p => p.operating.code).FirstOrDefault();
            Assert.Equal(OutBoundOperating_Code.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Operating_Name
        [Fact]
        public async void OutBound_Operating_Name()
        {
            await GetFlightAsync();
            String OutBoundOperating_Name = "AMERICAN AIRLINES";
            var item = Flightresult.Select(p => p.operating.name).FirstOrDefault();
            Assert.Equal(OutBoundOperating_Name.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_Operating_Code
        [Fact]
        public async void OutBound_Aircraft_Code()
        {
            await GetFlightAsync();
            String OutBoundAircraft_Code = "77W";
            var item = Flightresult.Select(p => p.aircraft.code).FirstOrDefault();
            Assert.Equal(OutBoundAircraft_Code.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_Aircraft_Name
        [Fact]
        public async void OutBound_Aircraft_Name()
        {
            await GetFlightAsync();
            String OutBoundAircraft_Name = "BOEING 777-300ER";
            var item = Flightresult.Select(p => p.aircraft.name).FirstOrDefault();
            Assert.Equal(OutBoundAircraft_Name.ToString(), item.ToString());
        }
        #endregion

        #region OutBound_numberOfStops
        [Fact]
        public async void OutBound_numberOfStops()
        {
            await GetFlightAsync();
            String OutBoundnumberOfStops = "0";
            var item = Flightresult.Select(p => p.numberOfStops).FirstOrDefault();
            Assert.Equal(OutBoundnumberOfStops.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_number
        [Fact]
        public async void OutBound_number()
        {
            await GetFlightAsync();
            String OutBoundnumber = "4028";
            var item = Flightresult.Select(p => p.number).FirstOrDefault();
            Assert.Equal(OutBoundnumber.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_number
        [Fact]
        public async void OutBound_cabinClass()
        {
            await GetFlightAsync();
            String OutBoundcabinClass = "BUSINESS";
            var item = Flightresult.Select(p => p.cabinClass).FirstOrDefault();
            Assert.Equal(OutBoundcabinClass.ToString(), item.ToString());
        }
        #endregion
        #region OutBound_number
        [Fact]
        public async void OutBound_cost()
        {
            await GetFlightAsync();
            String OutBoundcost = "2689.25";
            var item = Flightresult.Select(p => p.cost).FirstOrDefault();
            Assert.Equal(OutBoundcost.ToString(), item.ToString());
        }
        #endregion


        // InBound ---------------------------------------------------------------------------------


        #region InBound_Duration
        [Fact]
        public async void InBound_Duration()
        {
            await GetFlightAsync();
            String InBoundDuration = "PT9H35M";
            var item = Flightresult.Select(p => p.duration).Skip(1).First();
            Assert.Equal(InBoundDuration.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Departure_Code
        [Fact]
        public async void InBound_Departure_Code()
        {
            await GetFlightAsync();
            String InBoundDeparture_Code = "LHR";
            var item = Flightresult.Select(p => p.departure.code).Skip(1).First();
            Assert.Equal(InBoundDeparture_Code.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Departure_Name
        [Fact]
        public async void InBound_Departure_Name()
        {
            await GetFlightAsync();
            String InBoundDeparture_Name = "";
            var item = Flightresult.Select(p => p.departure.name).Skip(1).First();
            Assert.Equal(InBoundDeparture_Name.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Departure_City
        [Fact]
        public async void InBound_Departure_City()
        {
            await GetFlightAsync();
            String InBoundDeparture_City = "";
            var item = Flightresult.Select(p => p.departure.city).Skip(1).First();
            Assert.Equal(InBoundDeparture_City.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Departure_City
        [Fact]
        public async void InBound_Departure_Country()
        {
            await GetFlightAsync();
            String InBoundDeparture_Country = "";
            var item = Flightresult.Select(p => p.departure.country).Skip(1).First();
            Assert.Equal(InBoundDeparture_Country.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Departure_At
        [Fact]
        public async void InBound_Departure_At()
        {
            await GetFlightAsync();
            String InBoundDeparture_At = "2021-08-08T12:50:00";
            var item = Flightresult.Select(p => p.departure.at).Skip(1).First();
            Assert.Equal(InBoundDeparture_At.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Arrival_Code
        [Fact]
        public async void InBound_Arrival_Code()
        {
            await GetFlightAsync();
            String InBoundArrival_Code = "MIA";
            var item = Flightresult.Select(p => p.arrival.code).Skip(1).First();
            Assert.Equal(InBoundArrival_Code.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Arrival_Name
        [Fact]
        public async void InBound_Arrival_Name()
        {
            await GetFlightAsync();
            String InBoundArrival_Name = "";
            var item = Flightresult.Select(p => p.arrival.name).Skip(1).First();
            Assert.Equal(InBoundArrival_Name.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Arrival_City
        [Fact]
        public async void InBound_Arrival_City()
        {
            await GetFlightAsync();
            String InBoundArrival_City = "";
            var item = Flightresult.Select(p => p.arrival.city).Skip(1).First();
            Assert.Equal(InBoundArrival_City.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Arrival_City
        [Fact]
        public async void InBound_Arrival_Country()
        {
            await GetFlightAsync();
            String InBoundArrival_Country = "";
            var item = Flightresult.Select(p => p.arrival.country).Skip(1).First();
            Assert.Equal(InBoundArrival_Country.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Arrival_At
        [Fact]
        public async void InBound_Arrival_At()
        {
            await GetFlightAsync();
            String InBoundArrival_At = "2021-08-08T17:25:00";
            var item = Flightresult.Select(p => p.arrival.at).Skip(1).First();
            Assert.Equal(InBoundArrival_At.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Carrier_Code
        [Fact]
        public async void InBound_Carrier_Code()
        {
            await GetFlightAsync();
            String InBoundcarrier_Code = "AY";
            var item = Flightresult.Select(p => p.carrier.code).Skip(1).First();
            Assert.Equal(InBoundcarrier_Code.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Carrier_Name
        [Fact]
        public async void InBound_Carrier_Name()
        {
            await GetFlightAsync();
            String InBoundCarrier_Name = "FINNAIR";
            var item = Flightresult.Select(p => p.carrier.name).Skip(1).First();
            Assert.Equal(InBoundCarrier_Name.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Operating_Code
        [Fact]
        public async void InBound_Operating_Code()
        {
            await GetFlightAsync();
            String InBoundOperating_Code = "AA";
            var item = Flightresult.Select(p => p.operating.code).Skip(1).First();
            Assert.Equal(InBoundOperating_Code.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Operating_Name
        [Fact]
        public async void InBound_Operating_Name()
        {
            await GetFlightAsync();
            String InBoundOperating_Name = "AMERICAN AIRLINES";
            var item = Flightresult.Select(p => p.operating.name).Skip(1).First();
            Assert.Equal(InBoundOperating_Name.ToString(), item.ToString());
        }
        #endregion

        #region InBound_Operating_Code
        [Fact]
        public async void InBound_Aircraft_Code()
        {
            await GetFlightAsync();
            String InBoundAircraft_Code = "77W";
            var item = Flightresult.Select(p => p.aircraft.code).Skip(1).First();
            Assert.Equal(InBoundAircraft_Code.ToString(), item.ToString());
        }
        #endregion
        #region InBound_Aircraft_Name
        [Fact]
        public async void InBound_Aircraft_Name()
        {
            await GetFlightAsync();
            String InBoundAircraft_Name = "BOEING 777-300ER";
            var item = Flightresult.Select(p => p.aircraft.name).Skip(1).First();
            Assert.Equal(InBoundAircraft_Name.ToString(), item.ToString());
        }
        #endregion

        #region InBound_numberOfStops
        [Fact]
        public async void InBound_numberOfStops()
        {
            await GetFlightAsync();
            String InBoundnumberOfStops = "0";
            var item = Flightresult.Select(p => p.numberOfStops).Skip(1).First();
            Assert.Equal(InBoundnumberOfStops.ToString(), item.ToString());
        }
        #endregion
        #region InBound_number
        [Fact]
        public async void InBound_number()
        {
            await GetFlightAsync();
            String InBoundnumber = "4029";
            var item = Flightresult.Select(p => p.number).Skip(1).First();
            Assert.Equal(InBoundnumber.ToString(), item.ToString());
        }
        #endregion
        #region InBound_number
        [Fact]
        public async void InBound_cabinClass()
        {
            await GetFlightAsync();
            String InBoundcabinClass = "BUSINESS";
            var item = Flightresult.Select(p => p.cabinClass).Skip(1).First();
            Assert.Equal(InBoundcabinClass.ToString(), item.ToString());
        }
        #endregion
        #region InBound_number
        [Fact]
        public async void InBound_cost()
        {
            await GetFlightAsync();
            String InBoundcost = "2689.25";
            var item = Flightresult.Select(p => p.cost).Skip(1).First();
            Assert.Equal(InBoundcost.ToString(), item.ToString());
        }
        #endregion
    }
}
