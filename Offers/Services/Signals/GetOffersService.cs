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
    public class GetOffersService : IGetOffersService<SignalsBaseEntity>
    {
        #region SignalsServiceFields
        private readonly IOffersService<OffersBaseEntity> _OffersServiceCaller;
        private readonly ILogger<SignalsService> _Logger;
        #endregion

        #region SignalBucketConstructor
        public GetOffersService(IOffersService<OffersBaseEntity> OffersServiceCaller,ILogger<SignalsService> Logger, IFileSaveService<FileSaveBaseEntity> IFileSave)
        {
            _OffersServiceCaller = OffersServiceCaller;
            _Logger = Logger;
        }
        #endregion

        #region GetSearch
        public async Task<List<FlightResponse_v1>> GetOffers(FlightSearchQuery_v2 Query)
        {
            try
            {
                List<FlightResponse_v1> FlightResponse_v1List = new List<FlightResponse_v1>();
                FlightSearchQuery_v1 FlightSearchQuery = new FlightSearchQuery_v1();
                FlightSearchQuery.TransactionId = Query.TransactionId;
                FlightSearchQuery.ArrivalDate = Query.ArrivalDate;
                FlightSearchQuery.ArrivalLocation = Query.ArrivalLocation;
                FlightSearchQuery.Cabins = Query.Cabins;
                FlightSearchQuery.DateFlexibility = Query.DateFlexibility;
                FlightSearchQuery.DepartureDate = Query.DepartureDate;
                FlightSearchQuery.DepatureLocation = Query.DepatureLocation;
                FlightSearchQuery.PaxAdult = Query.PaxAdult;

                FlightResponse_v1List = await _OffersServiceCaller.FlightOffersSearch(FlightSearchQuery);
                return await Task.FromResult(FlightResponse_v1List);

            }
            catch (Exception ErrorMessage)
            {
                _Logger.LogInformation(ErrorMessage.StackTrace);
                _Logger.LogError(ErrorMessage.StackTrace);
                return null;
            }
        }

        #endregion
    }
}
