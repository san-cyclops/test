using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Offers.Models;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Offers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Flights
{
    public class FlightsService : IFlightsService<OffersBaseEntity>
    {
        #region Fields
        private readonly ICacheService<CacheBaseEntity> _CacheCaller;
        private readonly IFileSaveService<FileSaveBaseEntity> _FileSaveCaller;
        private readonly IOffersService<OffersBaseEntity> _OffersCaller;
        private readonly IErrorService<ErrorBaseEntity> _ErrorLog;
        public ILogger<FlightsService> _Logger { get; }
        #endregion

        #region Costructor
        public FlightsService(IOffersService<OffersBaseEntity> OffersCaller, IFileSaveService<FileSaveBaseEntity> FileSaveCaller, ILogger<FlightsService> Logger, ICacheService<CacheBaseEntity> CacheCaller, IErrorService<ErrorBaseEntity> ErrorLogCaller)
        {
            _OffersCaller = OffersCaller;
            _Logger = Logger;
            _CacheCaller = CacheCaller;
            _FileSaveCaller = FileSaveCaller;
            _ErrorLog = ErrorLogCaller;
        }
        #endregion

        #region SearchBaseCaller


        public async Task<List<FlightResponse_v3>> FlightsSearchBaseCaller([FromQuery] FlightSearchQuery_v3 RequestQuery)
        {
            #region Calling to Supplier
            try
            {
                await _FileSaveCaller.Save_v1(RequestQuery, "Fights-RQ", RequestQuery.TransactionId);

                List<FlightResponse_v1> FlightResponse_v1List = new List<FlightResponse_v1>();
                List<FlightResponse_v3> FlightResponse_v3List = new List<FlightResponse_v3>();
                List<FlightResponse_v3> FlightResponse_v3ListRange = new List<FlightResponse_v3>();

                Random RandomNumber = new Random();

                RequestQuery.TransactionId = RequestQuery.TransactionId + "_" + RandomNumber.Next(1000, 1500).ToString();


                if (RequestQuery.ArrivalDate == "" || RequestQuery.ArrivalDate == null)
                {
                    DateTime DepartureDateObj = Convert.ToDateTime(RequestQuery.DepartureDate.Trim());
                    DateTime ArrivalDateObj = DepartureDateObj.AddDays(7);
                    RequestQuery.ArrivalDate = ArrivalDateObj.ToString("yyyy-MM-dd");
                }

                String BaseKey = RequestQuery.DepartureDate + "@@" + RequestQuery.ArrivalDate + "@@" + RequestQuery.OriginLocation + "@@" + RequestQuery.DestinationLocation + "@@" + RequestQuery.Class + "@@" + RequestQuery.Airline;

                try
                {
                    FlightResponse_v3List = await _CacheCaller.GetFlights(BaseKey);
                    await _FileSaveCaller.Save_v1(BaseKey, "CacheFlights-RQ", RequestQuery.Class + "_" + RequestQuery.TransactionId);
                    await _FileSaveCaller.Save_v1(FlightResponse_v3List, "CacheFlights-RS", RequestQuery.Class + "_" + RequestQuery.TransactionId);
                    FlightResponse_v3ListRange.AddRange(FlightResponse_v3List);

                }
                catch
                {
                    FlightSearchQuery_v1 FlightSearchQuery_v1Obj = await FlightSearchRQBuilder(RequestQuery);
                    FlightResponse_v1List = await _OffersCaller.FlightOffersSearch(FlightSearchQuery_v1Obj);
                    FlightResponse_v3List = await FlightsResponseBuilderV1(FlightResponse_v1List, RequestQuery);
                    await _FileSaveCaller.Save_v1(FlightResponse_v3List, "Flights-RS", RequestQuery.TransactionId);

                    if (FlightResponse_v3List.Count == 0)
                    {
                        FlightResponse_v3 FlightResponse_v3Obj = new FlightResponse_v3();
                        FlightResponse_v3Obj.ErrorList = new List<ErrorRs>();
                        FlightResponse_v3Obj.ErrorList.Add(await _ErrorLog.ErrorLog("No Data found", "No Data found for chosen selection criteria"));
                        FlightResponse_v3ListRange.Add(FlightResponse_v3Obj);
                        return await Task.FromResult(FlightResponse_v3ListRange);
                    }
                    try
                    {
                        await _CacheCaller.PostFlights(FlightResponse_v3List, BaseKey);
                        await _FileSaveCaller.Save_v1(BaseKey, "Cache-Flights-RQ", RequestQuery.Class + "_" + RequestQuery.TransactionId);
                        await _FileSaveCaller.Save_v1(FlightResponse_v3List, "Cache-Flights-RS", RequestQuery.Class + "_" + RequestQuery.TransactionId);
                        return await Task.FromResult(FlightResponse_v3List);
                    }
                    catch (Exception Error)
                    {
                        await _FileSaveCaller.Save_v1(Error, "Error-Flights", "" + "_" + RequestQuery.TransactionId);
                        _Logger.LogInformation("FlightsSearch Error because of exception at caching FlightsService");
                        _Logger.LogError("FlightsSearch Error because of exception at caching", Error.StackTrace);
                        return null;
                    }
                }
                return FlightResponse_v3ListRange;
            }
            #endregion
            catch (Exception Error)
            {
                await _FileSaveCaller.Save_v1(Error, "Error-Flights", "" + "_" + RequestQuery.TransactionId);
                _Logger.LogInformation("FlightsSearch Error because of exception at FlightsService");
                _Logger.LogError("FlightsSearch Error", Error.StackTrace);
                return null;
            }

        }
        #endregion

        #region RequestBuilder
        /*
       * Create Request according to Amadeus Format after we got FlightSearchQuery_v3 From Front-end
       * 
       */
        public async Task<FlightSearchQuery_v1> FlightSearchRQBuilder([FromQuery] FlightSearchQuery_v3 RequestQuery)
        {
            try
            {
                FlightSearchQuery_v1 FlightSearchQuery_v1 = new FlightSearchQuery_v1();
                FlightSearchQuery_v1.ArrivalDate = RequestQuery.ArrivalDate;
                FlightSearchQuery_v1.DepartureDate = RequestQuery.DepartureDate;
                FlightSearchQuery_v1.DepatureLocation = RequestQuery.OriginLocation;
                FlightSearchQuery_v1.ArrivalLocation = RequestQuery.DestinationLocation;
                FlightSearchQuery_v1.Cabins = RequestQuery.Class;
                FlightSearchQuery_v1.PaxAdult = RequestQuery.PaxAdult;
                FlightSearchQuery_v1.DateFlexibility = 3;
                return await Task.FromResult(FlightSearchQuery_v1);
            }
            catch (Exception Error)
            {
                _Logger.LogError("FlightSearchRQBuilder  in Flight Service Error", Error);
                return null;
            }


        }
        #endregion

        #region ResponseBuilder
        public async Task<List<FlightResponse_v3>> FlightsResponseBuilderV1(List<FlightResponse_v1> FlightResponse_v1List, FlightSearchQuery_v3 SearchQuery_v3)
        {
            try
            {
                List<FlightResponse_v1> FlightResponse_v1FilterList = FlightResponse_v1List.Where(data => data.outbound.carrier.code == SearchQuery_v3.Airline && data.inbound.carrier.code == SearchQuery_v3.Airline && data.outbound.departure.at.Substring(0, 10) == SearchQuery_v3.DepartureDate && data.inbound.arrival.at.Substring(0, 10) == SearchQuery_v3.ArrivalDate && data.outbound.@class == CabinFinder(SearchQuery_v3.Class.Substring(1, 1)) && data.inbound.@class == CabinFinder(SearchQuery_v3.Class.Substring(1, 1)) && data.outbound.@class == CabinFinder(SearchQuery_v3.Class.Substring(1, 1))).ToList();
               
                List<FlightResponse_v3> FlightResponse_v3ReturnList = new List<FlightResponse_v3>();
                
                foreach (FlightResponse_v1 FlightResItem in FlightResponse_v1FilterList)
                {
                    List<FlightResponse_v3> FlightResponseList = new List<FlightResponse_v3>();

                    FlightResponse_v3 FlightResponse_V3InboundObj = new FlightResponse_v3();
                    #region Inbound FlightRes V3
                    FlightResponse_V3InboundObj = new FlightResponse_v3();
                    FlightResponse_V3InboundObj.id = FlightResItem.id;
                    FlightResponse_V3InboundObj.duration = FlightResItem.inbound.duration;
                    Departure_v3 Departure_v3Obj = new Departure_v3();
                    Departure_v3Obj.code = FlightResItem.inbound.departure.code;
                    Departure_v3Obj.name = FlightResItem.inbound.departure.name;
                    Departure_v3Obj.city = "";
                    Departure_v3Obj.country = "";
                    Departure_v3Obj.at = FlightResItem.inbound.departure.at;
                    FlightResponse_V3InboundObj.departure = Departure_v3Obj;

                    Arrival_v3 Arrival_v3Obj = new Arrival_v3();
                    Arrival_v3Obj.code = FlightResItem.inbound.arrival.code;
                    Arrival_v3Obj.name = FlightResItem.inbound.arrival.name;
                    Arrival_v3Obj.city = "";
                    Arrival_v3Obj.country = "";
                    Arrival_v3Obj.at = FlightResItem.inbound.arrival.at;
                    FlightResponse_V3InboundObj.arrival = Arrival_v3Obj;

                    Carrier_v3 Carrier_v3Obj = new Carrier_v3();
                    Carrier_v3Obj.code = FlightResItem.inbound.carrier.code;
                    Carrier_v3Obj.name = FlightResItem.inbound.carrier.name;
                    FlightResponse_V3InboundObj.carrier = Carrier_v3Obj;

                    Operating_v3 Operating_v3Obj = new Operating_v3();
                    Operating_v3Obj.code = FlightResItem.inbound.operating.code;
                    Operating_v3Obj.name = FlightResItem.inbound.operating.name;
                    FlightResponse_V3InboundObj.operating = Operating_v3Obj;

                    Aircraft_v3 Aircraft_v3Obj = new Aircraft_v3();
                    Aircraft_v3Obj.code = FlightResItem.inbound.aircraft.code;
                    Aircraft_v3Obj.name = FlightResItem.inbound.aircraft.name;
                    FlightResponse_V3InboundObj.aircraft = Aircraft_v3Obj;


                    FlightResponse_V3InboundObj.numberOfStops = 0;
                    FlightResponse_V3InboundObj.number = FlightResItem.inbound.flight;
                    FlightResponse_V3InboundObj.cabinClass = FlightResItem.inbound.@class;
                    FlightResponse_V3InboundObj.cost = FlightResItem.cost.amount;
                    #endregion

                    #region Outbound FlightRes V3
                    FlightResponse_v3 FlightResponse_V3OutboundObj = new FlightResponse_v3();
                    FlightResponse_V3OutboundObj.id = FlightResItem.id;
                    FlightResponse_V3OutboundObj.duration = FlightResItem.outbound.duration;
                    Departure_v3Obj = new Departure_v3();
                    Departure_v3Obj.code = FlightResItem.outbound.departure.code;
                    Departure_v3Obj.name = FlightResItem.outbound.departure.name;
                    Departure_v3Obj.city = "";
                    Departure_v3Obj.country = "";
                    Departure_v3Obj.at = FlightResItem.outbound.departure.at;
                    FlightResponse_V3OutboundObj.departure = Departure_v3Obj;

                    Arrival_v3Obj = new Arrival_v3();
                    Arrival_v3Obj.code = FlightResItem.outbound.arrival.code;
                    Arrival_v3Obj.name = FlightResItem.outbound.arrival.name;
                    Arrival_v3Obj.city = "";
                    Arrival_v3Obj.country = "";
                    Arrival_v3Obj.at = FlightResItem.outbound.arrival.at;
                    FlightResponse_V3OutboundObj.arrival = Arrival_v3Obj;

                    Carrier_v3Obj = new Carrier_v3();
                    Carrier_v3Obj.code = FlightResItem.outbound.carrier.code;
                    Carrier_v3Obj.name = FlightResItem.outbound.carrier.name;
                    FlightResponse_V3OutboundObj.carrier = Carrier_v3Obj;

                    Operating_v3Obj = new Operating_v3();
                    Operating_v3Obj.code = FlightResItem.outbound.operating.code;
                    Operating_v3Obj.name = FlightResItem.outbound.operating.name;
                    FlightResponse_V3OutboundObj.operating = Operating_v3Obj;

                    Aircraft_v3Obj = new Aircraft_v3();
                    Aircraft_v3Obj.code = FlightResItem.outbound.aircraft.code;
                    Aircraft_v3Obj.name = FlightResItem.outbound.aircraft.name;
                    FlightResponse_V3OutboundObj.aircraft = Aircraft_v3Obj;


                    FlightResponse_V3OutboundObj.numberOfStops = 0;
                    FlightResponse_V3OutboundObj.number = FlightResItem.outbound.flight;
                    FlightResponse_V3OutboundObj.cabinClass = FlightResItem.outbound.@class;
                    FlightResponse_V3OutboundObj.cost = FlightResItem.cost.amount;
                    #endregion

                    FlightResponseList.Add(FlightResponse_V3OutboundObj);
                    FlightResponseList.Add(FlightResponse_V3InboundObj);
                    FlightResponse_v3ReturnList.AddRange(FlightResponseList);
                }
                return await Task.FromResult(FlightResponse_v3ReturnList);
            }
            catch (Exception Error)
            {
                _Logger.LogError("FlightResponseBuilder Error", Error);
                return null;
            }
        }
        #endregion

        #region CabinClassMapperForSeatData
        /*
       * For replace cabin class which is FCF Format to apply ratings
       * 
       */
        public string CabinFinder(String Cabin)
        {
            switch (Cabin.ToUpper())
            {
                case "B":
                    Cabin = "BUSINESS";
                    break;
                case "F":
                    Cabin = "FIRST";
                    break;
                case "P":
                    Cabin = "PREMIUM_ECONOMY";
                    break;
                case "E":
                    Cabin = "ECONOMY";
                    break;
            }
            return Cabin;
        }
        #endregion
    }
}
