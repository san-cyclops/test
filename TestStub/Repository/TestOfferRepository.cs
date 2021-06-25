using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TestStub.Models;

namespace TestStub.Repository
{
    public class TestOfferRepository : ITestOfferRepository<IOffersBaseEntity>
    {
        private ITestAuthRepository<IAuthBaseEntity> _ITestAuthRepositoryCaller;

        public TestOfferRepository(ITestAuthRepository<IAuthBaseEntity> ITestAuthRepositoryCaller)
        {
            _ITestAuthRepositoryCaller = ITestAuthRepositoryCaller;
        }

        #region` RequestBuilder
        public async Task<List<OfferCompareResponse>> OfferCompareCaller()
        {
            List<OfferCompareResponse> OfferCompareResponseList = new List<OfferCompareResponse>();
            try
            {
                foreach (var item in await S3TestSecenarioCaller())
                {

                    //CsV File values initialize

                    string[] KeyValues = item.Value.Split('/');

                    FlightSearchQuery RequestQuery = new FlightSearchQuery();
                    RequestQuery.DepartureDate = KeyValues[0];
                    RequestQuery.ArrivalDate = KeyValues[1];
                    RequestQuery.DepatureLocation = KeyValues[2];
                    RequestQuery.ArrivalLocation = KeyValues[3];
                    RequestQuery.MixedCabin = Convert.ToBoolean(KeyValues[4]);
                    RequestQuery.PaxAdult = Convert.ToInt32(KeyValues[5]);
                    RequestQuery.DateFlexibility = Convert.ToInt32(KeyValues[6]);
                    RequestQuery.MixedCabinFlexibility = KeyValues[7].Split(',');


                    Dictionary<int, List<FlightResponse_v1>> FlightResponse_v1Dictionary = new Dictionary<int, List<FlightResponse_v1>>();
                    FlightResponse_v1Dictionary.Add(1, await OffersAPICaller(RequestQuery));
                    FlightResponse_v1Dictionary.Add(2, await S3FakeGDSReader(item.Key));
                    OfferCompareResponseList = await RespeonseBuilder(FlightResponse_v1Dictionary);
                }

            }
            catch (Exception)
            {

                throw;
            }
            return await Task.FromResult(OfferCompareResponseList);
        }
        #endregion

        #region` OffersAPICaller
        public async Task<List<FlightResponse_v1>> OffersAPICaller([FromQuery] FlightSearchQuery RequestQuery)
        {
            try
            {
                AuthRequest AuthRequest = new AuthRequest();
                // set auth values 
                AuthRequest.client_id = "lcjcPju7JT5uJWkoD2rDJtoYbK0tEBeG";
                AuthRequest.client_secret = "O59TIb82ypGt3oiGXkx1Xm5uK3IjZ8dCMDuX4IcqvlLQ9hrKrZP_X9xFsn8jXoaA";
                AuthRequest.audience = "https://upgradeengine.com";
                AuthRequest.grant_type = "client_credentials";
                AuthResponse AuthresponseObj = await _ITestAuthRepositoryCaller.AuthCallerAsync(AuthRequest);

                //
                String Request = "DepartureDate=" + RequestQuery.DepartureDate + "&ArrivalDate=" + RequestQuery.ArrivalDate + "&DepatureLocation=" + RequestQuery.DepatureLocation + "&ArrivalLocation=" + RequestQuery.ArrivalLocation + "&MixedCabin=" + RequestQuery.MixedCabin + "&PaxAdult=" + RequestQuery.PaxAdult + "&DateFlexibility=" + RequestQuery.DateFlexibility + "&MixedCabinFlexibility=" + RequestQuery.MixedCabinFlexibility[0];
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://upggradeengine-env.eba-utpezc53.ap-southeast-1.elasticbeanstalk.com/offers/Offers?");
                    string tokenfinal = "Bearer " + AuthresponseObj.access_token;
                    client.DefaultRequestHeaders.Add("accept", "text/plain");
                    client.DefaultRequestHeaders.Add("Authorization", tokenfinal);
                    var responseTask = client.GetAsync("https://upggradeengine-env.eba-utpezc53.ap-southeast-1.elasticbeanstalk.com/offers/Offers?"+ Request);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        Console.WriteLine(result.Content.ReadAsStringAsync().Result);
                        return await Task.FromResult(JsonConvert.DeserializeObject<List<FlightResponse_v1>>(result.Content.ReadAsStringAsync().Result));
                    }
                }
            }
            catch (Exception ErrorMessage)
            {
                Console.WriteLine(ErrorMessage);
                return null;
            }
            return null;
        }
        #endregion

        #region` S3TestSecenarioCaller
        public async Task<Dictionary<string, string>> S3TestSecenarioCaller()
        {
            try
            {
                //1/lon/lon/200.
                Dictionary<string, string> TestSamples = new Dictionary<string, string>();
                string[] lines = System.IO.File.ReadAllLines(@"C:\FirstClassFlyer\Offer\OfferRawData.csv");
                int i = 0;
                foreach (string Line in lines)
                {

                    if (Line != null)
                    {
                        string[] KeyValues = Line.Split(',');
                        string Key = KeyValues[0];
                        String value = KeyValues[1] + "/" + KeyValues[2] + "/" + KeyValues[3] + "/" + KeyValues[4] + "/" + KeyValues[5] + "/" + KeyValues[6] + "/" + KeyValues[7] + "/" + KeyValues[8];
                        if (!TestSamples.ContainsKey(Key))
                        {
                            TestSamples.Add(Key, value);
                        }
                        i++;
                    }
                }

                return await Task.FromResult(TestSamples);
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region` S3FakeGDSReader
        public async Task<List<FlightResponse_v1>> S3FakeGDSReader(String RequestId)
        {
            List<FlightResponse_v1> FlightResponse_v1Obj = new List<FlightResponse_v1>();
            try
            {
                string jsonFilePath = @"C:\FirstClassFlyer\Offer\" + RequestId + ".json";

                string json = File.ReadAllText(jsonFilePath);
                FlightResponse_v1Obj = JsonConvert.DeserializeObject<List<FlightResponse_v1>>(json);
            }
            catch
            {
                throw;
            }
            return await Task.FromResult(FlightResponse_v1Obj);
        }
        #endregion

        #region` RespeonseBuilder
        public async Task<List<OfferCompareResponse>> RespeonseBuilder(Dictionary<int, List<FlightResponse_v1>> FlightResponse_v1Dictionary)
        {
            List<OfferCompareResponse> OfferCompareResponseSender = new List<OfferCompareResponse>();
            List<FlightResponse_v1> FlightResponse_v1ObjOne = FlightResponse_v1Dictionary[1];
            List<FlightResponse_v1> FlightResponse_v1ObjTwo = FlightResponse_v1Dictionary[2];

            try
            {


                for (int i = 0; i < FlightResponse_v1ObjOne.Count; i++)
                {
                    var itemOne = FlightResponse_v1ObjOne[i];
                    var itemTwo = FlightResponse_v1ObjTwo[i];

                    OfferCompareResponse OfferCompareResponseList = new OfferCompareResponse();
                    List<OfferCompare> OfferCompareList = new List<OfferCompare>();

                    OfferCompare OfferCompareResponseId = new OfferCompare();
                    OfferCompareResponseId.FieldName = "Id";
                    OfferCompareResponseId.ActualValue = itemOne.id.ToString();
                    OfferCompareResponseId.ExpectedValue = itemTwo.id.ToString();
                    OfferCompareResponseId.Result = (OfferCompareResponseId.ActualValue == OfferCompareResponseId.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseId);

                    OfferCompare OfferCompareResponseArrival = new OfferCompare();
                    OfferCompareResponseArrival.FieldName = "Arrival";
                    OfferCompareResponseArrival.ActualValue = itemOne.inbound.arrival.code.ToString();
                    OfferCompareResponseArrival.ExpectedValue = itemTwo.inbound.arrival.code.ToString();
                    OfferCompareResponseArrival.Result = (OfferCompareResponseArrival.ActualValue == OfferCompareResponseArrival.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseArrival);

                    OfferCompare OfferCompareResponseArrivaldate = new OfferCompare();
                    OfferCompareResponseArrivaldate.FieldName = "Arrivaldate";
                    OfferCompareResponseArrivaldate.ActualValue = itemOne.inbound.arrival.at.ToString();
                    OfferCompareResponseArrivaldate.ExpectedValue = itemTwo.inbound.arrival.at.ToString();
                    OfferCompareResponseArrivaldate.Result = (OfferCompareResponseArrivaldate.ActualValue == OfferCompareResponseArrivaldate.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseArrivaldate);

                    OfferCompare OfferCompareResponseCarriercode = new OfferCompare();
                    OfferCompareResponseCarriercode.FieldName = "Carriercode";
                    OfferCompareResponseCarriercode.ActualValue = itemOne.inbound.carrier.code.ToString();
                    OfferCompareResponseCarriercode.ExpectedValue = itemTwo.inbound.carrier.code.ToString();
                    OfferCompareResponseCarriercode.Result = (OfferCompareResponseCarriercode.ActualValue == OfferCompareResponseCarriercode.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseCarriercode);

                    OfferCompare OfferCompareResponseCarriercodename = new OfferCompare();
                    OfferCompareResponseCarriercodename.FieldName = "Carriercodename";
                    OfferCompareResponseCarriercodename.ActualValue = itemOne.inbound.carrier.name.ToString();
                    OfferCompareResponseCarriercodename.ExpectedValue = itemTwo.inbound.carrier.name.ToString();
                    OfferCompareResponseCarriercodename.Result = (OfferCompareResponseCarriercodename.ActualValue == OfferCompareResponseCarriercodename.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseCarriercodename);

                    OfferCompare OfferCompareResponseDeparture = new OfferCompare();
                    OfferCompareResponseDeparture.FieldName = "Departure";
                    OfferCompareResponseDeparture.ActualValue = itemOne.inbound.departure.code.ToString();
                    OfferCompareResponseDeparture.ExpectedValue = itemTwo.inbound.departure.code.ToString();
                    OfferCompareResponseDeparture.Result = (OfferCompareResponseDeparture.ActualValue == OfferCompareResponseDeparture.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseDeparture);

                    OfferCompare OfferCompareResponseDeparturedate = new OfferCompare();
                    OfferCompareResponseDeparturedate.FieldName = "Departuredate";
                    OfferCompareResponseDeparturedate.ActualValue = itemOne.inbound.departure.at.ToString();
                    OfferCompareResponseDeparturedate.ExpectedValue = itemTwo.inbound.departure.at.ToString();
                    OfferCompareResponseDeparturedate.Result = (OfferCompareResponseDeparturedate.ActualValue == OfferCompareResponseDeparturedate.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseDeparturedate);

                    OfferCompare OfferCompareResponseDuration = new OfferCompare();
                    OfferCompareResponseDuration.FieldName = "Duration";
                    OfferCompareResponseDuration.ActualValue = itemOne.inbound.duration.ToString();
                    OfferCompareResponseDuration.ExpectedValue = itemTwo.inbound.duration.ToString();
                    OfferCompareResponseDuration.Result = (OfferCompareResponseDuration.ActualValue == OfferCompareResponseDuration.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseDuration);

                    OfferCompare OfferCompareResponseAircraftcode = new OfferCompare();
                    OfferCompareResponseAircraftcode.FieldName = "Aircraftcode";
                    OfferCompareResponseAircraftcode.ActualValue = itemOne.inbound.aircraft.code.ToString();
                    OfferCompareResponseAircraftcode.ExpectedValue = itemTwo.inbound.aircraft.code.ToString();
                    OfferCompareResponseAircraftcode.Result = (OfferCompareResponseAircraftcode.ActualValue == OfferCompareResponseAircraftcode.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseAircraftcode);

                    OfferCompare OfferCompareResponseAircraftname = new OfferCompare();
                    OfferCompareResponseAircraftname.FieldName = "Aircraftname";
                    OfferCompareResponseAircraftname.ActualValue = itemOne.inbound.aircraft.name.ToString();
                    OfferCompareResponseAircraftname.ExpectedValue = itemTwo.inbound.aircraft.name.ToString();
                    OfferCompareResponseAircraftname.Result = (OfferCompareResponseAircraftname.ActualValue == OfferCompareResponseAircraftname.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseAircraftname);

                    OfferCompare OfferCompareResponseFlight = new OfferCompare();
                    OfferCompareResponseFlight.FieldName = "Flight";
                    OfferCompareResponseFlight.ActualValue = itemOne.inbound.flight.ToString();
                    OfferCompareResponseFlight.ExpectedValue = itemTwo.inbound.flight.ToString();
                    OfferCompareResponseFlight.Result = (OfferCompareResponseFlight.ActualValue == OfferCompareResponseFlight.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseFlight);

                    OfferCompare OfferCompareResponseOperatingcode = new OfferCompare();
                    OfferCompareResponseOperatingcode.FieldName = "Operatingcode";
                    OfferCompareResponseOperatingcode.ActualValue = itemOne.inbound.operating.code.ToString();
                    OfferCompareResponseOperatingcode.ExpectedValue = itemTwo.inbound.operating.code.ToString();
                    OfferCompareResponseOperatingcode.Result = (OfferCompareResponseOperatingcode.ActualValue == OfferCompareResponseOperatingcode.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOperatingcode);

                    OfferCompare OfferCompareResponseOperatingcodename = new OfferCompare();
                    OfferCompareResponseOperatingcodename.FieldName = "Operatingcodename";
                    OfferCompareResponseOperatingcodename.ActualValue = itemOne.inbound.operating.name.ToString();
                    OfferCompareResponseOperatingcodename.ExpectedValue = itemTwo.inbound.operating.name.ToString();
                    OfferCompareResponseOperatingcodename.Result = (OfferCompareResponseOperatingcodename.ActualValue == OfferCompareResponseOperatingcodename.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOperatingcodename);

                    OfferCompare OfferCompareResponseClass = new OfferCompare();
                    OfferCompareResponseClass.FieldName = "Class";
                    OfferCompareResponseClass.ActualValue = itemOne.inbound.@class.ToString();
                    OfferCompareResponseClass.ExpectedValue = itemTwo.inbound.@class.ToString();
                    OfferCompareResponseClass.Result = (OfferCompareResponseClass.ActualValue == OfferCompareResponseClass.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseClass);

                    OfferCompare OfferCompareResponseRatings = new OfferCompare();
                    OfferCompareResponseRatings.FieldName = "Ratings";
                    OfferCompareResponseRatings.ActualValue = itemOne.inbound.ratings.ToString();
                    OfferCompareResponseRatings.ExpectedValue = itemTwo.inbound.ratings.ToString();
                    OfferCompareResponseRatings.Result = (OfferCompareResponseRatings.ActualValue == OfferCompareResponseRatings.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseRatings);

                    OfferCompare OfferCompareResponseOutArrival = new OfferCompare();
                    OfferCompareResponseOutArrival.FieldName = "OutArrival";
                    OfferCompareResponseOutArrival.ActualValue = itemOne.outbound.arrival.code.ToString();
                    OfferCompareResponseOutArrival.ExpectedValue = itemTwo.outbound.arrival.code.ToString();
                    OfferCompareResponseOutArrival.Result = (OfferCompareResponseOutArrival.ActualValue == OfferCompareResponseOutArrival.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutArrival);

                    OfferCompare OfferCompareResponseOutArrivaldate = new OfferCompare();
                    OfferCompareResponseOutArrivaldate.FieldName = "OutArrivaldate";
                    OfferCompareResponseOutArrivaldate.ActualValue = itemOne.outbound.arrival.at.ToString();
                    OfferCompareResponseOutArrivaldate.ExpectedValue = itemTwo.outbound.arrival.at.ToString();
                    OfferCompareResponseOutArrivaldate.Result = (OfferCompareResponseOutArrivaldate.ActualValue == OfferCompareResponseOutArrivaldate.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutArrivaldate);

                    OfferCompare OfferCompareResponseOutCarriercode = new OfferCompare();
                    OfferCompareResponseOutCarriercode.FieldName = "OutCarriercode";
                    OfferCompareResponseOutCarriercode.ActualValue = itemOne.outbound.carrier.name.ToString();
                    OfferCompareResponseOutCarriercode.ExpectedValue = itemTwo.outbound.carrier.name.ToString();
                    OfferCompareResponseOutCarriercode.Result = (OfferCompareResponseOutCarriercode.ActualValue == OfferCompareResponseOutCarriercode.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutCarriercode);

                    OfferCompare OfferCompareResponseOutCarriercodename = new OfferCompare();
                    OfferCompareResponseOutCarriercodename.FieldName = "OutCarriercodename";
                    OfferCompareResponseOutCarriercodename.ActualValue = itemOne.outbound.carrier.name.ToString();
                    OfferCompareResponseOutCarriercodename.ExpectedValue = itemTwo.outbound.carrier.name.ToString();
                    OfferCompareResponseOutCarriercodename.Result = (OfferCompareResponseOutCarriercodename.ActualValue == OfferCompareResponseOutCarriercodename.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutCarriercodename);

                    OfferCompare OfferCompareResponseOutDeparture = new OfferCompare();
                    OfferCompareResponseOutDeparture.FieldName = "Outbound-Departure";
                    OfferCompareResponseOutDeparture.ActualValue = itemOne.outbound.departure.code.ToString();
                    OfferCompareResponseOutDeparture.ExpectedValue = itemTwo.outbound.departure.code.ToString();
                    OfferCompareResponseOutDeparture.Result = (OfferCompareResponseOutDeparture.ActualValue == OfferCompareResponseOutDeparture.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutDeparture);

                    OfferCompare OfferCompareResponseOutDeparturedate = new OfferCompare();
                    OfferCompareResponseOutDeparturedate.FieldName = "OutDeparturedate";
                    OfferCompareResponseOutDeparturedate.ActualValue = itemOne.outbound.departure.at.ToString();
                    OfferCompareResponseOutDeparturedate.ExpectedValue = itemTwo.outbound.departure.at.ToString();
                    OfferCompareResponseOutDeparturedate.Result = (OfferCompareResponseOutDeparturedate.ActualValue == OfferCompareResponseOutDeparturedate.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutDeparturedate);

                    OfferCompare OfferCompareResponseOutDuration = new OfferCompare();
                    OfferCompareResponseOutDuration.FieldName = "OutDuration";
                    OfferCompareResponseOutDuration.ActualValue = itemOne.outbound.duration.ToString();
                    OfferCompareResponseOutDuration.ExpectedValue = itemTwo.outbound.duration.ToString();
                    OfferCompareResponseOutDuration.Result = (OfferCompareResponseOutDuration.ActualValue == OfferCompareResponseOutDuration.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutDuration);

                    OfferCompare OfferCompareResponseOutAircraftcode = new OfferCompare();
                    OfferCompareResponseOutAircraftcode.FieldName = "OutAircraftcode";
                    OfferCompareResponseOutAircraftcode.ActualValue = itemOne.outbound.aircraft.code.ToString();
                    OfferCompareResponseOutAircraftcode.ExpectedValue = itemTwo.outbound.aircraft.code.ToString();
                    OfferCompareResponseOutAircraftcode.Result = (OfferCompareResponseOutAircraftcode.ActualValue == OfferCompareResponseOutAircraftcode.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutAircraftcode);

                    OfferCompare OfferCompareResponseOutAircraftname = new OfferCompare();
                    OfferCompareResponseOutAircraftname.FieldName = "OutAircraftname";
                    OfferCompareResponseOutAircraftname.ActualValue = itemOne.outbound.aircraft.name.ToString();
                    OfferCompareResponseOutAircraftname.ExpectedValue = itemTwo.outbound.aircraft.name.ToString();
                    OfferCompareResponseOutAircraftname.Result = (OfferCompareResponseOutAircraftname.ActualValue == OfferCompareResponseOutAircraftname.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutAircraftname);

                    OfferCompare OfferCompareResponseOutFlight = new OfferCompare();
                    OfferCompareResponseOutFlight.FieldName = "OutFlight";
                    OfferCompareResponseOutFlight.ActualValue = itemOne.outbound.flight.ToString();
                    OfferCompareResponseOutFlight.ExpectedValue = itemTwo.outbound.flight.ToString();
                    OfferCompareResponseOutFlight.Result = (OfferCompareResponseOutFlight.ActualValue == OfferCompareResponseOutFlight.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutFlight);

                    OfferCompare OfferCompareResponseOutOperatingcode = new OfferCompare();
                    OfferCompareResponseOutOperatingcode.FieldName = "OutOperatingcode";
                    OfferCompareResponseOutOperatingcode.ActualValue = itemOne.outbound.operating.code.ToString();
                    OfferCompareResponseOutOperatingcode.ExpectedValue = itemTwo.outbound.operating.code.ToString();
                    OfferCompareResponseOutOperatingcode.Result = (OfferCompareResponseOutOperatingcode.ActualValue == OfferCompareResponseOutOperatingcode.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutOperatingcode);

                    OfferCompare OfferCompareResponseOutOperatingcodename = new OfferCompare();
                    OfferCompareResponseOutOperatingcodename.FieldName = "OutOperatingcodename";
                    OfferCompareResponseOutOperatingcodename.ActualValue = itemOne.outbound.operating.name.ToString();
                    OfferCompareResponseOutOperatingcodename.ExpectedValue = itemTwo.outbound.operating.name.ToString();
                    OfferCompareResponseOutOperatingcodename.Result = (OfferCompareResponseOutOperatingcodename.ActualValue == OfferCompareResponseOutOperatingcodename.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutOperatingcodename);

                    OfferCompare OfferCompareResponseOutClass = new OfferCompare();
                    OfferCompareResponseOutClass.FieldName = "OutClass";
                    OfferCompareResponseOutClass.ActualValue = itemOne.outbound.@class.ToString();
                    OfferCompareResponseOutClass.ExpectedValue = itemTwo.outbound.@class.ToString();
                    OfferCompareResponseOutClass.Result = (OfferCompareResponseOutClass.ActualValue == OfferCompareResponseOutClass.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutClass);

                    OfferCompare OfferCompareResponseOutRatings = new OfferCompare();
                    OfferCompareResponseOutRatings.FieldName = "OutRatings";
                    OfferCompareResponseOutRatings.ActualValue = itemOne.outbound.ratings.ToString();
                    OfferCompareResponseOutRatings.ExpectedValue = itemTwo.outbound.ratings.ToString();
                    OfferCompareResponseOutRatings.Result = (OfferCompareResponseOutRatings.ActualValue == OfferCompareResponseOutRatings.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseOutRatings);

                    OfferCompare OfferCompareResponseAmount = new OfferCompare();
                    OfferCompareResponseAmount.FieldName = "Amount";
                    OfferCompareResponseAmount.ActualValue = itemOne.cost.amount.ToString();
                    OfferCompareResponseAmount.ExpectedValue = itemTwo.cost.amount.ToString();
                    OfferCompareResponseAmount.Result = (OfferCompareResponseAmount.ActualValue == OfferCompareResponseAmount.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseAmount);

                    OfferCompare OfferCompareResponseUnit = new OfferCompare();
                    OfferCompareResponseUnit.FieldName = "Unit";
                    OfferCompareResponseUnit.ActualValue = itemOne.cost.unit.ToString();
                    OfferCompareResponseUnit.ExpectedValue = itemTwo.cost.unit.ToString();
                    OfferCompareResponseUnit.Result = (OfferCompareResponseUnit.ActualValue == OfferCompareResponseUnit.ExpectedValue) ? true : false;
                    OfferCompareList.Add(OfferCompareResponseUnit);

                    OfferCompareResponseList.OfferCompareList = OfferCompareList;
                    OfferCompareResponseSender.Add(OfferCompareResponseList);
                }

                //foreach (var itemOne in FlightResponse_v1ObjOne)
                //{
                //    foreach (var itemTwo in FlightResponse_v1ObjTwo)
                //    {
                //        OfferCompareResponse OfferCompareResponseList = new OfferCompareResponse();
                //        List<OfferCompare> OfferCompareList = new List<OfferCompare>();

                //        OfferCompare OfferCompareResponseId = new OfferCompare();
                //        OfferCompareResponseId.FieldName = "Id";
                //        OfferCompareResponseId.ActualValue = itemOne.id.ToString();
                //        OfferCompareResponseId.ExpectedValue = itemTwo.id.ToString();
                //        OfferCompareResponseId.Result = (OfferCompareResponseId.ActualValue == OfferCompareResponseId.ExpectedValue) ? true : false;
                //        //OfferCompareResponseObj.Add(OfferCompareResponseId);
                //        //OfferCompareResponseList.OfferCompareList.Add(OfferCompareResponseId);
                //        OfferCompareList.Add(OfferCompareResponseId);

                //        OfferCompare OfferCompareResponseArrival = new OfferCompare();
                //        OfferCompareResponseArrival.FieldName = "Arrival";
                //        OfferCompareResponseArrival.ActualValue = itemOne.inbound.arrival.code.ToString();
                //        OfferCompareResponseArrival.ExpectedValue = itemTwo.inbound.arrival.code.ToString();
                //        OfferCompareResponseArrival.Result = (OfferCompareResponseArrival.ActualValue == OfferCompareResponseArrival.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseArrival);

                //        OfferCompare OfferCompareResponseArrivaldate = new OfferCompare();
                //        OfferCompareResponseArrivaldate.FieldName = "Arrivaldate";
                //        OfferCompareResponseArrivaldate.ActualValue = itemOne.inbound.arrival.at.ToString();
                //        OfferCompareResponseArrivaldate.ExpectedValue = itemTwo.inbound.arrival.at.ToString();
                //        OfferCompareResponseArrivaldate.Result = (OfferCompareResponseArrivaldate.ActualValue == OfferCompareResponseArrivaldate.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseArrivaldate);

                //        OfferCompare OfferCompareResponseCarriercode = new OfferCompare();
                //        OfferCompareResponseCarriercode.FieldName = "Carriercode";
                //        OfferCompareResponseCarriercode.ActualValue = itemOne.inbound.carrier.code.ToString();
                //        OfferCompareResponseCarriercode.ExpectedValue = itemTwo.inbound.carrier.code.ToString();
                //        OfferCompareResponseCarriercode.Result = (OfferCompareResponseCarriercode.ActualValue == OfferCompareResponseCarriercode.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseCarriercode);

                //        OfferCompare OfferCompareResponseCarriercodename = new OfferCompare();
                //        OfferCompareResponseCarriercodename.FieldName = "Carriercodename";
                //        OfferCompareResponseCarriercodename.ActualValue = itemOne.inbound.carrier.name.ToString();
                //        OfferCompareResponseCarriercodename.ExpectedValue = itemTwo.inbound.carrier.name.ToString();
                //        OfferCompareResponseCarriercodename.Result = (OfferCompareResponseCarriercodename.ActualValue == OfferCompareResponseCarriercodename.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseCarriercodename);

                //        OfferCompare OfferCompareResponseDeparture = new OfferCompare();
                //        OfferCompareResponseDeparture.FieldName = "Departure";
                //        OfferCompareResponseDeparture.ActualValue = itemOne.inbound.departure.code.ToString();
                //        OfferCompareResponseDeparture.ExpectedValue = itemTwo.inbound.departure.code.ToString();
                //        OfferCompareResponseDeparture.Result = (OfferCompareResponseDeparture.ActualValue == OfferCompareResponseDeparture.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseDeparture);

                //        OfferCompare OfferCompareResponseDeparturedate = new OfferCompare();
                //        OfferCompareResponseDeparturedate.FieldName = "Departuredate";
                //        OfferCompareResponseDeparturedate.ActualValue = itemOne.inbound.departure.at.ToString();
                //        OfferCompareResponseDeparturedate.ExpectedValue = itemTwo.inbound.departure.at.ToString();
                //        OfferCompareResponseDeparturedate.Result = (OfferCompareResponseDeparturedate.ActualValue == OfferCompareResponseDeparturedate.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseDeparturedate);

                //        OfferCompare OfferCompareResponseDuration = new OfferCompare();
                //        OfferCompareResponseDuration.FieldName = "Duration";
                //        OfferCompareResponseDuration.ActualValue = itemOne.inbound.duration.ToString();
                //        OfferCompareResponseDuration.ExpectedValue = itemTwo.inbound.duration.ToString();
                //        OfferCompareResponseDuration.Result = (OfferCompareResponseDuration.ActualValue == OfferCompareResponseDuration.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseDuration);

                //        OfferCompare OfferCompareResponseAircraftcode = new OfferCompare();
                //        OfferCompareResponseAircraftcode.FieldName = "Aircraftcode";
                //        OfferCompareResponseAircraftcode.ActualValue = itemOne.inbound.aircraft.code.ToString();
                //        OfferCompareResponseAircraftcode.ExpectedValue = itemTwo.inbound.aircraft.code.ToString();
                //        OfferCompareResponseAircraftcode.Result = (OfferCompareResponseAircraftcode.ActualValue == OfferCompareResponseAircraftcode.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseAircraftcode);

                //        OfferCompare OfferCompareResponseAircraftname = new OfferCompare();
                //        OfferCompareResponseAircraftname.FieldName = "Aircraftname";
                //        OfferCompareResponseAircraftname.ActualValue = itemOne.inbound.aircraft.name.ToString();
                //        OfferCompareResponseAircraftname.ExpectedValue = itemTwo.inbound.aircraft.name.ToString();
                //        OfferCompareResponseAircraftname.Result = (OfferCompareResponseAircraftname.ActualValue == OfferCompareResponseAircraftname.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseAircraftname);

                //        OfferCompare OfferCompareResponseFlight = new OfferCompare();
                //        OfferCompareResponseFlight.FieldName = "Flight";
                //        OfferCompareResponseFlight.ActualValue = itemOne.inbound.flight.ToString();
                //        OfferCompareResponseFlight.ExpectedValue = itemTwo.inbound.flight.ToString();
                //        OfferCompareResponseFlight.Result = (OfferCompareResponseFlight.ActualValue == OfferCompareResponseFlight.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseFlight);

                //        OfferCompare OfferCompareResponseOperatingcode = new OfferCompare();
                //        OfferCompareResponseOperatingcode.FieldName = "Operatingcode";
                //        OfferCompareResponseOperatingcode.ActualValue = itemOne.inbound.operating.code.ToString();
                //        OfferCompareResponseOperatingcode.ExpectedValue = itemTwo.inbound.operating.code.ToString();
                //        OfferCompareResponseOperatingcode.Result = (OfferCompareResponseOperatingcode.ActualValue == OfferCompareResponseOperatingcode.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOperatingcode);

                //        OfferCompare OfferCompareResponseOperatingcodename = new OfferCompare();
                //        OfferCompareResponseOperatingcodename.FieldName = "Operatingcodename";
                //        OfferCompareResponseOperatingcodename.ActualValue = itemOne.inbound.operating.name.ToString();
                //        OfferCompareResponseOperatingcodename.ExpectedValue = itemTwo.inbound.operating.name.ToString();
                //        OfferCompareResponseOperatingcodename.Result = (OfferCompareResponseOperatingcodename.ActualValue == OfferCompareResponseOperatingcodename.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOperatingcodename);

                //        OfferCompare OfferCompareResponseClass = new OfferCompare();
                //        OfferCompareResponseClass.FieldName = "Class";
                //        OfferCompareResponseClass.ActualValue = itemOne.inbound.@class.ToString();
                //        OfferCompareResponseClass.ExpectedValue = itemTwo.inbound.@class.ToString();
                //        OfferCompareResponseClass.Result = (OfferCompareResponseClass.ActualValue == OfferCompareResponseClass.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseClass);

                //        OfferCompare OfferCompareResponseRatings = new OfferCompare();
                //        OfferCompareResponseRatings.FieldName = "Ratings";
                //        OfferCompareResponseRatings.ActualValue = itemOne.inbound.ratings.ToString();
                //        OfferCompareResponseRatings.ExpectedValue = itemTwo.inbound.ratings.ToString();
                //        OfferCompareResponseRatings.Result = (OfferCompareResponseRatings.ActualValue == OfferCompareResponseRatings.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseRatings);

                //        OfferCompare OfferCompareResponseOutArrival = new OfferCompare();
                //        OfferCompareResponseOutArrival.FieldName = "OutArrival";
                //        OfferCompareResponseOutArrival.ActualValue = itemOne.outbound.arrival.code.ToString();
                //        OfferCompareResponseOutArrival.ExpectedValue = itemTwo.outbound.arrival.code.ToString();
                //        OfferCompareResponseOutArrival.Result = (OfferCompareResponseOutArrival.ActualValue == OfferCompareResponseOutArrival.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutArrival);

                //        OfferCompare OfferCompareResponseOutArrivaldate = new OfferCompare();
                //        OfferCompareResponseOutArrivaldate.FieldName = "OutArrivaldate";
                //        OfferCompareResponseOutArrivaldate.ActualValue = itemOne.outbound.arrival.at.ToString();
                //        OfferCompareResponseOutArrivaldate.ExpectedValue = itemTwo.outbound.arrival.at.ToString();
                //        OfferCompareResponseOutArrivaldate.Result = (OfferCompareResponseOutArrivaldate.ActualValue == OfferCompareResponseOutArrivaldate.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutArrivaldate);

                //        OfferCompare OfferCompareResponseOutCarriercode = new OfferCompare();
                //        OfferCompareResponseOutCarriercode.FieldName = "OutCarriercode";
                //        OfferCompareResponseOutCarriercode.ActualValue = itemOne.outbound.carrier.name.ToString();
                //        OfferCompareResponseOutCarriercode.ExpectedValue = itemTwo.outbound.carrier.name.ToString();
                //        OfferCompareResponseOutCarriercode.Result = (OfferCompareResponseOutCarriercode.ActualValue == OfferCompareResponseOutCarriercode.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutCarriercode);

                //        OfferCompare OfferCompareResponseOutCarriercodename = new OfferCompare();
                //        OfferCompareResponseOutCarriercodename.FieldName = "OutCarriercodename";
                //        OfferCompareResponseOutCarriercodename.ActualValue = itemOne.outbound.carrier.name.ToString();
                //        OfferCompareResponseOutCarriercodename.ExpectedValue = itemTwo.outbound.carrier.name.ToString();
                //        OfferCompareResponseOutCarriercodename.Result = (OfferCompareResponseOutCarriercodename.ActualValue == OfferCompareResponseOutCarriercodename.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutCarriercodename);

                //        OfferCompare OfferCompareResponseOutDeparture = new OfferCompare();
                //        OfferCompareResponseOutDeparture.FieldName = "OutDeparture";
                //        OfferCompareResponseOutDeparture.ActualValue = itemOne.outbound.departure.code.ToString();
                //        OfferCompareResponseOutDeparture.ExpectedValue = itemTwo.outbound.departure.code.ToString();
                //        OfferCompareResponseOutDeparture.Result = (OfferCompareResponseOutDeparture.ActualValue == OfferCompareResponseOutDeparture.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutDeparture);

                //        OfferCompare OfferCompareResponseOutDeparturedate = new OfferCompare();
                //        OfferCompareResponseOutDeparturedate.FieldName = "OutDeparturedate";
                //        OfferCompareResponseOutDeparturedate.ActualValue = itemOne.outbound.departure.at.ToString();
                //        OfferCompareResponseOutDeparturedate.ExpectedValue = itemTwo.outbound.departure.at.ToString();
                //        OfferCompareResponseOutDeparturedate.Result = (OfferCompareResponseOutDeparturedate.ActualValue == OfferCompareResponseOutDeparturedate.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutDeparturedate);

                //        OfferCompare OfferCompareResponseOutDuration = new OfferCompare();
                //        OfferCompareResponseOutDuration.FieldName = "OutDuration";
                //        OfferCompareResponseOutDuration.ActualValue = itemOne.outbound.duration.ToString();
                //        OfferCompareResponseOutDuration.ExpectedValue = itemTwo.outbound.duration.ToString();
                //        OfferCompareResponseOutDuration.Result = (OfferCompareResponseOutDuration.ActualValue == OfferCompareResponseOutDuration.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutDuration);

                //        OfferCompare OfferCompareResponseOutAircraftcode = new OfferCompare();
                //        OfferCompareResponseOutAircraftcode.FieldName = "OutAircraftcode";
                //        OfferCompareResponseOutAircraftcode.ActualValue = itemOne.outbound.aircraft.code.ToString();
                //        OfferCompareResponseOutAircraftcode.ExpectedValue = itemTwo.outbound.aircraft.code.ToString();
                //        OfferCompareResponseOutAircraftcode.Result = (OfferCompareResponseOutAircraftcode.ActualValue == OfferCompareResponseOutAircraftcode.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutAircraftcode);

                //        OfferCompare OfferCompareResponseOutAircraftname = new OfferCompare();
                //        OfferCompareResponseOutAircraftname.FieldName = "OutAircraftname";
                //        OfferCompareResponseOutAircraftname.ActualValue = itemOne.outbound.aircraft.name.ToString();
                //        OfferCompareResponseOutAircraftname.ExpectedValue = itemTwo.outbound.aircraft.name.ToString();
                //        OfferCompareResponseOutAircraftname.Result = (OfferCompareResponseOutAircraftname.ActualValue == OfferCompareResponseOutAircraftname.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutAircraftname);

                //        OfferCompare OfferCompareResponseOutFlight = new OfferCompare();
                //        OfferCompareResponseOutFlight.FieldName = "OutFlight";
                //        OfferCompareResponseOutFlight.ActualValue = itemOne.outbound.flight.ToString();
                //        OfferCompareResponseOutFlight.ExpectedValue = itemTwo.outbound.flight.ToString();
                //        OfferCompareResponseOutFlight.Result = (OfferCompareResponseOutFlight.ActualValue == OfferCompareResponseOutFlight.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutFlight);

                //        OfferCompare OfferCompareResponseOutOperatingcode = new OfferCompare();
                //        OfferCompareResponseOutOperatingcode.FieldName = "OutOperatingcode";
                //        OfferCompareResponseOutOperatingcode.ActualValue = itemOne.outbound.operating.code.ToString();
                //        OfferCompareResponseOutOperatingcode.ExpectedValue = itemTwo.outbound.operating.code.ToString();
                //        OfferCompareResponseOutOperatingcode.Result = (OfferCompareResponseOutOperatingcode.ActualValue == OfferCompareResponseOutOperatingcode.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutOperatingcode);

                //        OfferCompare OfferCompareResponseOutOperatingcodename = new OfferCompare();
                //        OfferCompareResponseOutOperatingcodename.FieldName = "OutOperatingcodename";
                //        OfferCompareResponseOutOperatingcodename.ActualValue = itemOne.outbound.operating.name.ToString();
                //        OfferCompareResponseOutOperatingcodename.ExpectedValue = itemTwo.outbound.operating.name.ToString();
                //        OfferCompareResponseOutOperatingcodename.Result = (OfferCompareResponseOutOperatingcodename.ActualValue == OfferCompareResponseOutOperatingcodename.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutOperatingcodename);

                //        OfferCompare OfferCompareResponseOutClass = new OfferCompare();
                //        OfferCompareResponseOutClass.FieldName = "OutClass";
                //        OfferCompareResponseOutClass.ActualValue = itemOne.outbound.@class.ToString();
                //        OfferCompareResponseOutClass.ExpectedValue = itemTwo.outbound.@class.ToString();
                //        OfferCompareResponseOutClass.Result = (OfferCompareResponseOutClass.ActualValue == OfferCompareResponseOutClass.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutClass);

                //        OfferCompare OfferCompareResponseOutRatings = new OfferCompare();
                //        OfferCompareResponseOutRatings.FieldName = "OutRatings";
                //        OfferCompareResponseOutRatings.ActualValue = itemOne.outbound.ratings.ToString();
                //        OfferCompareResponseOutRatings.ExpectedValue = itemTwo.outbound.ratings.ToString();
                //        OfferCompareResponseOutRatings.Result = (OfferCompareResponseOutRatings.ActualValue == OfferCompareResponseOutRatings.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseOutRatings);

                //        OfferCompare OfferCompareResponseAmount = new OfferCompare();
                //        OfferCompareResponseAmount.FieldName = "Amount";
                //        OfferCompareResponseAmount.ActualValue = itemOne.cost.amount.ToString();
                //        OfferCompareResponseAmount.ExpectedValue = itemTwo.cost.amount.ToString();
                //        OfferCompareResponseAmount.Result = (OfferCompareResponseAmount.ActualValue == OfferCompareResponseAmount.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseAmount);

                //        OfferCompare OfferCompareResponseUnit = new OfferCompare();
                //        OfferCompareResponseUnit.FieldName = "Unit";
                //        OfferCompareResponseUnit.ActualValue = itemOne.cost.unit.ToString();
                //        OfferCompareResponseUnit.ExpectedValue = itemTwo.cost.unit.ToString();
                //        OfferCompareResponseUnit.Result = (OfferCompareResponseUnit.ActualValue == OfferCompareResponseUnit.ExpectedValue) ? true : false;
                //        OfferCompareList.Add(OfferCompareResponseUnit);

                //        OfferCompareResponseList.OfferCompareList = OfferCompareList;
                //        OfferCompareResponseSender.Add(OfferCompareResponseList);

                //    }



                //}





                // OfferCompareResponseList.

            }
            catch (Exception)
            {

                throw;
            }

            return await Task.FromResult(OfferCompareResponseSender);
        }



        #endregion
    }
}
