using Microsoft.Extensions.Logging;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Cache;
using Offers.Services.FileSave;
using Offers.Services.Locations;
using Offers.Services.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Signals
{
    public class GetAlternattiveOffers : IGetAlternattiveOffers<SignalsBaseEntity>
    {
        #region SignalsServiceFields
        private readonly IOffersService<OffersBaseEntity> _OffersServiceCaller;
        private readonly ILocationsService<LocationsBaseEntity> _LocationsServiceCaller;
        private readonly ILogger<SignalsService> _Logger;
        private readonly IFileSaveService<FileSaveBaseEntity> _IFileSave;
        #endregion

        #region SignalBucketConstructor
        public GetAlternattiveOffers(IOffersService<OffersBaseEntity> OffersServiceCaller, ILocationsService<LocationsBaseEntity> LocationsServiceCaller, ILogger<SignalsService> Logger, IFileSaveService<FileSaveBaseEntity> IFileSave)
        {
            _OffersServiceCaller = OffersServiceCaller;
            _LocationsServiceCaller = LocationsServiceCaller;
            _Logger = Logger;
            _IFileSave = IFileSave;
        }
        #endregion

        /*
         * This service will create related query for the alternative offers and invoke those using /offer endpoint
         */

        #region GetCities
        public async Task<List<FlightResponse_v1>> GetOffers(string Iata, FlightSearchQuery_v2 Query)
        {
            try
            {
                LocationsRs LocationsRsList = new LocationsRs();
                try
                {
                    LocationsRsList = await _LocationsServiceCaller.GetLocation(Iata, Query.TransactionId);
                }
                catch
                {
                    return null;
                }
                List<FlightResponse_v1> FlightResponse_v1List = new List<FlightResponse_v1>();
                if (LocationsRsList != null || LocationsRsList.alternate.Count > 0)
                {
                    foreach (var item in LocationsRsList.alternate)
                    {
                        foreach (var items in Query.Cabins.Split(','))
                        {

                            FlightSearchQuery_v1 FlightSearchQuery = new FlightSearchQuery_v1();
                            FlightSearchQuery.TransactionId = Query.TransactionId;
                            FlightSearchQuery.ArrivalDate = Query.ArrivalDate;
                            FlightSearchQuery.ArrivalLocation = String.Concat(item.iata.Where(c => !Char.IsWhiteSpace(c)));
                            FlightSearchQuery.Cabins = items;
                            FlightSearchQuery.DateFlexibility = Query.DateFlexibility;
                            FlightSearchQuery.DepartureDate = Query.DepartureDate;
                            FlightSearchQuery.DepatureLocation = Query.DepatureLocation;
                            FlightSearchQuery.PaxAdult = Query.PaxAdult;

                            List<FlightResponse_v1> FlightResponse_v1 = new List<FlightResponse_v1>();
                            FlightResponse_v1 = await _OffersServiceCaller.FlightOffersSearch(FlightSearchQuery);
                            await _IFileSave.Save_v1(FlightSearchQuery, "AlternativeOffersRQ_"+item, Query.TransactionId);
                            await _IFileSave.Save_v1(FlightResponse_v1, "AlternativeOffersRS_"+item, Query.TransactionId);
                            if (FlightResponse_v1 != null)
                            {
                                FlightResponse_v1List.AddRange(FlightResponse_v1);
                            }
                        }
                    }
                }
                return await Task.FromResult(FlightResponse_v1List);
            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation("City data null for this transaction Id " + Query.TransactionId);
                _Logger.LogError(ErrorMessage.StackTrace);
                return null;
            }

        }
        #endregion
    }
}
