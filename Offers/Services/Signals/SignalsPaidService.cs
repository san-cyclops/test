using Microsoft.Extensions.Logging;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Models.Common.Weightnings;
using Offers.Services.Alliances;
using Offers.Services.FileSave;
using Offers.Services.UserRating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Offers.Services.Signals
{
    public class SignalsPaidService : ISignalsPaidService <SignalsBaseEntity>
    {

        #region Fields
        private IUserRatingService<UserRatingBaseEntity> _UserRating;
        private readonly ILogger<SignalsPaidService> _Logger;
        private readonly IFileSaveService<FileSaveBaseEntity> _IFileSave;
        private readonly IGetAlliances _IGetAlliances;
        #endregion

        #region Constructor 
        public SignalsPaidService(ILogger<SignalsPaidService> Logger, IFileSaveService<FileSaveBaseEntity> IFileSave,IUserRatingService<UserRatingBaseEntity> UserRating, IGetAlliances IGetAlliances)
        {
            _Logger = Logger;
            _IFileSave = IFileSave;
            _UserRating = UserRating;
            _IGetAlliances = IGetAlliances;
        }
        #endregion

        #region BaseCaller 
        public async Task<QuadrantsRS> SignalsPaidServiceBaseCaller(List<FlightResponse_v1> FlightResponse_v1, string Context, string TransactionId, string BaseOffer)
        {
            SignalsRs_v1 SignalsRs_v1Obj = new SignalsRs_v1();
            QuadrantsRS QuadrantsRSObj = new QuadrantsRS();
            List<SignalsBucketRs_v2> SignalsBucketRs_v2List = new List<SignalsBucketRs_v2>();
            SignalsBucketRs_v2List =  await SignalsPaidBucket(FlightResponse_v1,Context,TransactionId,BaseOffer);
            SignalsBucketRs_v2List  = await SignalsPaidBucketLogic(SignalsBucketRs_v2List,Context,TransactionId, BaseOffer);
            QuadrantsRSObj = await ResponseBuilder(SignalsBucketRs_v2List, TransactionId, BaseOffer);

            return await Task.FromResult(QuadrantsRSObj);
        }
        #endregion

        #region SignalBucket_v2
        public async Task<List<SignalsBucketRs_v2>> SignalsPaidBucket(List<FlightResponse_v1> FlightResponse_v1, string Context, string TransactionId, string BaseOffer)
        {
            List<SignalsBucketRs_v2> SignalBucketList = new List<SignalsBucketRs_v2>(); 
            foreach (var item in FlightResponse_v1)
            {
                try
                {
                    SignalsBucketRs_v2 SignalBucket = new SignalsBucketRs_v2();
                    SignalBucket.Id = item.id;
                    SignalBucket.OfferPrice = item.cost.amount;
                    SignalBucket.CabinClass = item.outbound.@class.Substring(0, 1) + item.inbound.@class.Substring(0,1);

                    //Outbound Details
                    SignalsBucketRs_Outbound OutboundObj = new SignalsBucketRs_Outbound();

                    OutboundObj.Cabin = item.outbound.@class;
                    OutboundObj.Date = item.outbound.departure.at;
                    OutboundObj.Destination = item.outbound.arrival.code;
                    OutboundObj.Origin = item.outbound.departure.code;
                    UserRatingRs UserRatingRsOutbondObj = new UserRatingRs();
                    UserRatingRsOutbondObj =  await _UserRating.UserRating(item.outbound.ratings, Context, TransactionId);
                    OutboundObj.UserRating = UserRatingRsOutbondObj.Value;
                    OutboundObj.OperatingAirline = item.outbound.operating.code;
                    SignalBucket.Outbound = OutboundObj;

                    //Outbound Details
                    SignalsBucketRs_Inbound InboundObj = new SignalsBucketRs_Inbound();

                    InboundObj.Cabin = item.inbound.@class;
                    InboundObj.Date = item.inbound.departure.at;
                    InboundObj.Destination = item.inbound.arrival.code;
                    InboundObj.Origin = item.inbound.departure.code;
                    UserRatingRs UserRatingRsInboundObj = new UserRatingRs();
                    UserRatingRsInboundObj = await _UserRating.UserRating(item.inbound.ratings, Context, TransactionId);
                    InboundObj.UserRating = UserRatingRsInboundObj.Value;
                    InboundObj.OperatingAirline = item.inbound.operating.code;
                    SignalBucket.Inbound = InboundObj;

                    SignalBucketList.Add(SignalBucket);
                }
                catch (Exception ErrorMessage)
                {
                    _Logger.LogWarning("Exception in SignalsPaidBucket");
                    _Logger.LogError(ErrorMessage.StackTrace);
                }
            }
              return await Task.FromResult(SignalBucketList);
        }
        #endregion

        #region SignalBucket_v2_Logic
        public async Task<List<SignalsBucketRs_v2>> SignalsPaidBucketLogic(List<SignalsBucketRs_v2> SignalsBucketRs_v2List, string Context, string TransactionId, string BaseLineFlight)
        {
            List<SignalsBucketRs_v2> NewSignalsBucketRs_v2List = new List<SignalsBucketRs_v2>();

            string[] DataList = BaseLineFlight.Split(",");
            double BaseFlightRating = Convert.ToDouble(DataList[4]);
            double BaseFlightPrice = Convert.ToDouble(DataList[5]);
            string BaseFlightDate = DataList[6];
            double BaseLineValueRatio = (BaseFlightRating / BaseFlightPrice) * 1000;

            foreach (var item in SignalsBucketRs_v2List)
            {
                item.Alliances = item.Outbound.OperatingAirline;
                item.Date = ((int)DateTime.Parse(item.Outbound.Date.Substring(0, 10)).Subtract(DateTime.Parse(BaseFlightDate)).TotalDays);
                item.UserRating =((item.Outbound.UserRating + item.Inbound.UserRating)/2);
                item.RatingDifference = item.UserRating- BaseFlightRating;
                item.PriceDifference = item.OfferPrice-BaseFlightPrice;
                item.ValueRatio= (item.UserRating/item.OfferPrice) * 1000;
                item.ValueDifference = item.ValueRatio - BaseLineValueRatio;

                _Logger.LogInformation("added To Paid Signals bucked  this Offer ID " + item.Id);
                NewSignalsBucketRs_v2List.Add(item);

                // item.PriceThreshold = 50.00;
                // item.IsCheaper = (item.PriceDifference <= item.PriceThreshold) ? true : false;
                // item.IsDearer = (item.PriceDifference > item.PriceThreshold) ? true : false;
                // item.IsSimilar = (item.IsCheaper ^ item.IsDearer);
                // item.IsUpgrade = item.RatingDifference > 0;
                // item.SavingsDowngrade = (item.PriceDifference > 0 ^ !(item.IsUpgrade));
                // item.BetterCheaper = (item.IsUpgrade ^ item.IsCheaper);
                // item.BetterSimilar = (item.IsSimilar ^ item.IsUpgrade);
                // item.Trash = (!item.IsUpgrade ^ item.IsDearer);
                // item.IsBetterValue = (item.ValueDifference > 0 ) ? true : false;
                // item.IsValidDestination = "Yes";
                // item.Include = (item.IsUpgrade && item.IsValidDestination=="Yes") ? true : false;
                // if (item.Include)
                //  {
                //  }

            }

            try
            {
                string[] Airlines = NewSignalsBucketRs_v2List.Select(p => p.Alliances).Distinct().ToArray();
                List<AirlineRS> AirlineRSList = await _IGetAlliances.GetAlliances(Airlines);

                foreach (SignalsBucketRs_v2 item in NewSignalsBucketRs_v2List)
                {
                    bool FindAlliances = false;
                    foreach (var AirlineList in AirlineRSList)
                    {
                        foreach (var Alliances in AirlineList.airlines)
                        {
                            if (item.Alliances == Alliances.iata)
                            {
                                FindAlliances = true;
                                item.Alliances = AirlineList.name;
                            }
                        }

                    }
                    if (!FindAlliances)
                    {
                        item.Alliances = "Non Alliance";
                    }

                }
            }
            catch (Exception ErrorMessage)
            {
               _Logger.LogInformation("Exception in PaidSignal Bucket Get Alliance");
               _Logger.LogError(ErrorMessage.StackTrace);
            }
            return await Task.FromResult(NewSignalsBucketRs_v2List);
        }
        #endregion

        #region SignalRS_v2
        public async Task <QuadrantsRS> ResponseBuilder(List<SignalsBucketRs_v2> SignalsBucketRsList, string TransactionId, string BaseLineFlight)
        {
            QuadrantsRS QuadrantsRS = new QuadrantsRS();
            string[] DataList = BaseLineFlight.Split(",");
            double BaseFlightRating = Convert.ToDouble(DataList[4]);
            double BaseFlightPrice = Convert.ToDouble(DataList[5]);
            string BaseFlightDate = DataList[6];
            double BaseLineValueRatio = (BaseFlightRating / BaseFlightPrice) * 1000;

            Dictionary<string, List<SignalsBucketRs_v2>> SignalFinalList = new Dictionary<string, List<SignalsBucketRs_v2>>();
            
            //ForUnitTesting
            // List<SignalsBucketRs_v2> OverviewBucket = SignalsBucketRsList;
            // SignalFinalList.Add("OverviewBucket", OverviewBucket);

            //savingsDowngrades: offer.price < baseline,price & offer.rating < baseline.rating
            List<SignalsBucketRs_v2> SavingsDowngradesBucket = SignalsBucketRsList.Where(data => data.OfferPrice < BaseFlightPrice && data.UserRating < BaseFlightRating).ToList();
            SignalFinalList.Add("SavingsDowngradesBucket", SavingsDowngradesBucket);

            //freeUpgrades: baseline.price * 1.05 >= offer.price >= baseline.price * 0.95 & offer.rating >= baseline.rating
            List<SignalsBucketRs_v2> FreeUpgradesBucket = SignalsBucketRsList.Where(data => (((BaseFlightPrice * 1.05 )>= data.OfferPrice) && (data.OfferPrice >= BaseFlightPrice*0.95 )&& (data.UserRating >= BaseFlightRating))).ToList();
            SignalFinalList.Add("FreeUpgradesBucket", FreeUpgradesBucket);

            //betterThanFree: offer.price < baseline.price * 0.95 & offer.rating < baseline.rating
            List<SignalsBucketRs_v2> BetterThanFree = SignalsBucketRsList.Where(data => data.OfferPrice < BaseFlightPrice * 0.95 && data.UserRating > BaseFlightRating).ToList();
            SignalFinalList.Add("BetterThanFree", BetterThanFree);

            //trash: offer.price > baseline,price & offer.rating < baseline.rating
            List<SignalsBucketRs_v2> TrashBucketBucket = SignalsBucketRsList.Where(data => data.OfferPrice > BaseFlightPrice && data.UserRating < BaseFlightRating).ToList();
            SignalFinalList.Add("TrashBucketBucket", TrashBucketBucket);

            //paidUpgrades: offer.price > baseline.price * 1.05 & offer.rating >= baseline.rating
            List<SignalsBucketRs_v2> PaidUpgradesBucket = SignalsBucketRsList.Where(data =>(data.OfferPrice >  (BaseFlightPrice * 1.05)) && data.UserRating >= BaseFlightRating).ToList();
           SignalFinalList.Add("PaidUpgradesBucket", PaidUpgradesBucket);


            foreach (KeyValuePair<string, List<SignalsBucketRs_v2>> Quadrants in SignalFinalList)
            {
                SignalsRs_v1 SignalsRs = new SignalsRs_v1();
                List<Destination> DestinationsList = new List<Destination>();
                List<Alliance> AlliancesList = new List<Alliance>();
                List<Airline> AirlinesList = new List<Airline>();
                List<DateFlexibility> DateFlexibilityList = new List<DateFlexibility>();
                MixedCabin MixedCabinObj = new MixedCabin();
                List<Cabin> cabinsList = new List<Cabin>();

                SignalsRs.BaselinePrice = BaseFlightPrice;
                SignalsRs.BaselineRating = BaseFlightRating;
                SignalsBucketRsList = Quadrants.Value;

                try
                {
                    var MixedCabinDeltaPriceMin = SignalsBucketRsList.Select(p => p.PriceDifference).Min();
                    var MixedCabinDeltaPriceMax = SignalsBucketRsList.Select(p => p.PriceDifference).Max();
                    var MixedCabinDeltaRatingMin = SignalsBucketRsList.Select(p => p.RatingDifference).Min();
                    var MixedCabinDeltaRatingMax = SignalsBucketRsList.Select(p => p.RatingDifference).Max();
                    var MixedCabinDeltaValueMin = SignalsBucketRsList.Select(p => p.ValueDifference).Min();
                    var MixedCabinDeltaValueMax = SignalsBucketRsList.Select(p => p.ValueDifference).Max();

                    MixedCabinObj.maxPriceDiff = MixedCabinDeltaPriceMax;
                    MixedCabinObj.minPriceDiff = MixedCabinDeltaPriceMin;
                    MixedCabinObj.minRatingDiff = MixedCabinDeltaRatingMin;
                    MixedCabinObj.maxRatingDiff = MixedCabinDeltaRatingMax;
                    MixedCabinObj.minValueDiff = MixedCabinDeltaValueMin;
                    MixedCabinObj.maxValueDiff = MixedCabinDeltaValueMax;
                    SignalsRs.mixedCabin = MixedCabinObj;
                }
                catch (Exception ErrorMessage)
                {
                    _Logger.LogInformation("Exception in MixedCabin " + Quadrants.Key);
                    _Logger.LogError(ErrorMessage.StackTrace);
                }


                try
                {
                    var Destination = SignalsBucketRsList.Select(data => new { data.Outbound.Destination }).Distinct().ToList();
                    foreach (var item in Destination)
                    {

                        var DeltaPriceMin = SignalsBucketRsList.Where(m => m.Outbound.Destination == item.Destination).Select(p => p.PriceDifference).Min();
                        var DeltaPriceMax = SignalsBucketRsList.Where(m => m.Outbound.Destination == item.Destination).Select(p => p.PriceDifference).Max();
                        var DeltaRatingMin = SignalsBucketRsList.Where(m => m.Outbound.Destination == item.Destination).Select(p => p.RatingDifference).Min();
                        var DeltaRatingMax = SignalsBucketRsList.Where(m => m.Outbound.Destination == item.Destination).Select(p => p.RatingDifference).Max();
                        var DeltaValueMin = SignalsBucketRsList.Where(m => m.Outbound.Destination == item.Destination).Select(p => p.ValueDifference).Min();
                        var DeltaValueMax = SignalsBucketRsList.Where(m => m.Outbound.Destination == item.Destination).Select(p => p.ValueDifference).Max();

                        Destination DestinationObj = new Destination();
                        DestinationObj.code = item.Destination;
                        DestinationObj.maxPriceDiff = DeltaPriceMax;
                        DestinationObj.minPriceDiff = DeltaPriceMin;
                        DestinationObj.minRatingDiff = DeltaRatingMin;
                        DestinationObj.maxRatingDiff = DeltaRatingMax;
                        DestinationObj.minValueDiff = DeltaValueMin;
                        DestinationObj.maxValueDiff = DeltaValueMax;

                        DestinationsList.Add(DestinationObj);

                    }
                }
                catch (Exception ErrorMessage)
                {
                    _Logger.LogInformation("Exception in Destination "+ Quadrants.Key);
                    _Logger.LogError(ErrorMessage.StackTrace);
                }


                try
                {
                    var Airlines = SignalsBucketRsList.Select(data => new { data.Outbound.OperatingAirline }).Distinct().ToList();
                    foreach (var item in Airlines)
                    {
                        var DeltaPriceMin = SignalsBucketRsList.Where(m => m.Outbound.OperatingAirline == item.OperatingAirline).Select(p => p.PriceDifference).Min();
                        var DeltaPriceMax = SignalsBucketRsList.Where(m => m.Outbound.OperatingAirline == item.OperatingAirline).Select(p => p.PriceDifference).Max();
                        var DeltaRatingMin = SignalsBucketRsList.Where(m => m.Outbound.OperatingAirline == item.OperatingAirline).Select(p => p.RatingDifference).Min();
                        var DeltaRatingMax = SignalsBucketRsList.Where(m => m.Outbound.OperatingAirline == item.OperatingAirline).Select(p => p.RatingDifference).Max();
                        var DeltaValueMin = SignalsBucketRsList.Where(m => m.Outbound.OperatingAirline == item.OperatingAirline).Select(p => p.ValueDifference).Min();
                        var DeltaValueMax = SignalsBucketRsList.Where(m => m.Outbound.OperatingAirline == item.OperatingAirline).Select(p => p.ValueDifference).Max();

                        Airline AirlineObj = new Airline();
                        AirlineObj.code = item.OperatingAirline;
                        AirlineObj.maxPriceDiff = DeltaPriceMax;
                        AirlineObj.minPriceDiff = DeltaPriceMin;
                        AirlineObj.minRatingDiff = DeltaRatingMin;
                        AirlineObj.maxRatingDiff = DeltaRatingMax;
                        AirlineObj.minValueDiff = DeltaValueMin;
                        AirlineObj.maxValueDiff = DeltaValueMax;

                        AirlinesList.Add(AirlineObj);

                    }
                }
                catch (Exception ErrorMessage)
                {
                    _Logger.LogInformation("Exception in Airlines " + Quadrants.Key);
                    _Logger.LogError(ErrorMessage.StackTrace);
                }

                try
                {
                    var Cabins = SignalsBucketRsList.Select(data => new { data.CabinClass }).Distinct().ToList();

                    foreach (var item in Cabins)
                    {
                        var DeltaPriceMin = SignalsBucketRsList.Where(m => m.CabinClass == item.CabinClass).Select(p => p.PriceDifference).Min();
                        var DeltaPriceMax = SignalsBucketRsList.Where(m => m.CabinClass == item.CabinClass).Select(p => p.PriceDifference).Max();
                        var DeltaRatingMin = SignalsBucketRsList.Where(m => m.CabinClass == item.CabinClass).Select(p => p.RatingDifference).Min();
                        var DeltaRatingMax = SignalsBucketRsList.Where(m => m.CabinClass == item.CabinClass).Select(p => p.RatingDifference).Max();
                        var DeltaValueMin = SignalsBucketRsList.Where(m => m.CabinClass == item.CabinClass).Select(p => p.ValueDifference).Min();
                        var DeltaValueMax = SignalsBucketRsList.Where(m => m.CabinClass == item.CabinClass).Select(p => p.ValueDifference).Max();

                        Cabin CabinObj = new Cabin();
                        CabinObj.code = item.CabinClass;
                        CabinObj.maxPriceDiff = DeltaPriceMax;
                        CabinObj.minPriceDiff = DeltaPriceMin;
                        CabinObj.minRatingDiff = DeltaRatingMin;
                        CabinObj.maxRatingDiff = DeltaRatingMax;
                        CabinObj.minValueDiff = DeltaValueMin;
                        CabinObj.maxValueDiff = DeltaValueMax;

                        cabinsList.Add(CabinObj);

                    }
                }
                catch (Exception ErrorMessage)
                {
                    _Logger.LogInformation("Exception in Cabins " + Quadrants.Key);
                    _Logger.LogError(ErrorMessage.StackTrace);
                }

                try
                {
                    var DateFlexibility = SignalsBucketRsList.Select(data => new { data.Date }).Distinct().ToList();

                    foreach (var item in DateFlexibility)
                    {
                        var DeltaPriceMin = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.PriceDifference).Min();
                        var DeltaPriceMax = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.PriceDifference).Max();
                        var DeltaRatingMin = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.RatingDifference).Min();
                        var DeltaRatingMax = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.RatingDifference).Max();
                        var DeltaValueMin = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.ValueDifference).Min();
                        var DeltaValueMax = SignalsBucketRsList.Where(m => m.Date == item.Date).Select(p => p.ValueDifference).Max();

                        DateFlexibility DateObj = new DateFlexibility();
                        DateObj.code = item.Date;
                        DateObj.maxPriceDiff = DeltaPriceMax;
                        DateObj.minPriceDiff = DeltaPriceMin;
                        DateObj.minRatingDiff = DeltaRatingMin;
                        DateObj.maxRatingDiff = DeltaRatingMax;
                        DateObj.minValueDiff = DeltaValueMin;
                        DateObj.maxValueDiff = DeltaValueMax;

                        DateFlexibilityList.Add(DateObj);

                    }
                }
                catch (Exception ErrorMessage)
                {
                    _Logger.LogInformation("Exception in DateFlexibility " + Quadrants.Key);
                    _Logger.LogError(ErrorMessage.StackTrace);
                }

                try
                {
                    var Alliances = SignalsBucketRsList.Select(data => new { data.Alliances }).Distinct().ToList();
                    foreach (var item in Alliances)
                    {
                        var DeltaPriceMin = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.PriceDifference).Min();
                        var DeltaPriceMax = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.PriceDifference).Max();
                        var DeltaRatingMin = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.RatingDifference).Min();
                        var DeltaRatingMax = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.RatingDifference).Max();
                        var DeltaValueMin = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.ValueDifference).Min();
                        var DeltaValueMax = SignalsBucketRsList.Where(m => m.Alliances == item.Alliances).Select(p => p.ValueDifference).Max();

                        Alliance AllianceObj = new Alliance();
                        AllianceObj.code = item.Alliances;
                        AllianceObj.maxPriceDiff = DeltaPriceMax;
                        AllianceObj.minPriceDiff = DeltaPriceMin;
                        AllianceObj.minRatingDiff = DeltaRatingMin;
                        AllianceObj.maxRatingDiff = DeltaRatingMax;
                        AllianceObj.minValueDiff = DeltaValueMin;
                        AllianceObj.maxValueDiff = DeltaValueMax;

                        AlliancesList.Add(AllianceObj);

                    }
                }
                catch (Exception ErrorMessage)
                {
                    _Logger.LogInformation("Exception in Alliances " + Quadrants.Key);
                    _Logger.LogError(ErrorMessage.StackTrace);
                }


                SignalsRs.offerIds = SignalsBucketRsList.Select(O => O.Id).ToArray();
                SignalsRs.destinations = DestinationsList;
                SignalsRs.airlines = AirlinesList;
                SignalsRs.alliances = AlliancesList;
                SignalsRs.cabins = cabinsList;
                DateFlexibilityList = DateFlexibilityList.Where(p => p.code >= -3 && p.code <= 3).ToList();
                SignalsRs.dateFlexibility = DateFlexibilityList.OrderBy(d => d.code).ToList();

                #region AddingtoQuadrantsSignalRS
                switch (Quadrants.Key)
                {
                    case "SavingsDowngradesBucket":
                        QuadrantsRS.savingsdowngrades = SignalsRs;
                        break;
                    case "FreeUpgradesBucket":
                        QuadrantsRS.freeupgrades = SignalsRs;
                        break;
                    case "BetterThanFree":
                        QuadrantsRS.betterthanfree = SignalsRs;
                        break;
                    case "TrashBucketBucket":
                        QuadrantsRS.trash = SignalsRs;
                        break;
                    case "PaidUpgradesBucket":
                        QuadrantsRS.paidupgrades = SignalsRs;
                        break;
                        
                }
                #endregion
            }
            return await Task.FromResult(QuadrantsRS);
        }
        #endregion


    }
}
