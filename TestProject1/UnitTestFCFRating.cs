using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Offers;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Alliances;
using Offers.Services.FileSave;
using Offers.Services.Signals;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class UnitTestFCFRating
    {
        public static SignalsRs_v1 result { get; set; }

        #region GetResponse
        public static async Task<SignalsRs_v1> GetResultAsync()
        {
            var mock = new Mock<ILogger<SignalsService>>();
            ILogger<SignalsService> MockSignal = mock.Object;

            var mockf = new Mock<IGetAlliances>();
            IGetAlliances MockALLiances = mockf.Object;

            var mocks = new Mock<IFileSaveService<FileSaveBaseEntity>>();
            IFileSaveService<FileSaveBaseEntity> MockFileSave = mocks.Object;

          //  var controller = new SignalBucketService(MockSignal, MockFileSave,MockALLiances);
            List<FlightResponse_v1> FlightResponse_v1List = JsonConvert.DeserializeObject<List<FlightResponse_v1>>(File.ReadAllText(@"D:\Ozen Labs\FCF\SIGNALS_qa\SPECIFICATIONS.json"));
         //   result = await controller.SignalBucketBaseCaller(FlightResponse_v1List,"AY,351,1,BB,3.5,2100,2021-04-04","UnitTest");
            return result;
        }
        #endregion

        #region Response_Not_Null
        [Fact]
        public async void ResponseNotNull()
        {
            await GetResultAsync();
            Assert.NotNull(result);
        }
        #endregion

        #region DestinationMinDiff
        [Fact]
        public async void Destination_Price_MinDiff()
        {
            await GetResultAsync();
            double DestinationMinDiff = -1100.00;
            var item = result.destinations.Find(p => p.code == "DEST").minPriceDiff;
            Assert.Equal(DestinationMinDiff.ToString(), item.ToString());
        }
        #endregion

        #region DestinationMaxDiff
        [Fact]
        public async void Destination_Price_MaxDiff()
        {
            await GetResultAsync();
            double DestinationMaxDiff = 400;
            var items = result.destinations.Find(p => p.code == "DEST").maxPriceDiff;
            Assert.Equal(DestinationMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DestinationRatingMaxDiff
        [Fact]
        public async void Destination_Rating_MaxDiff()
        {
            await GetResultAsync();
            double TestDestinationRatingMaxDiff = 0.70;
            var items = result.destinations.Find(p => p.code == "DEST").maxRatingDiff;
            Assert.Equal(TestDestinationRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DestinationRatingMinDiff
        [Fact]
        public async Task Destination_Rating_MinDiff()
        {
            await GetResultAsync();
            double TestDestinationRatingMinDiff = -0.50;
            var items = result.destinations.Find(p => p.code == "DEST").minRatingDiff;
            Assert.Equal(TestDestinationRatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DestinationValueMaxDiff
        [Fact]
        public async Task Destination_Value_MaxDiff()
        {
            await GetResultAsync();
            double TestDestinationValueMaxDiff = 1.33;
            var items = result.destinations.Find(p => p.code == "DEST").maxValueDiff;
            Assert.Equal(TestDestinationValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DestinationValueMinDiff
        [Fact]
        public async Task Destination_Value_MinDiff()
        {
            await GetResultAsync();
            double DestinationValueMinDiff = 0.01;
            var items = result.destinations.Find(p => p.code == "DEST").minValueDiff;
            Assert.Equal(DestinationValueMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region AlternativeDestinationMinDiff
        [Fact]
        public async void Alternative_Destination_Price_MinDiff()
        {
            await GetResultAsync();
            double ALTDestinationMinDiff = -700.00;
            var item = result.destinations.Find(p => p.code == "ALT").minPriceDiff;
            Assert.Equal(ALTDestinationMinDiff.ToString(), item.ToString());
        }
        #endregion

        #region AlternativeTestDestinationMinDiff
        [Fact]
         public async void Alternative_Destination_Price_MaxDiff()
        {
            await GetResultAsync();
            double ALDestinationMinDiff = -700.00;
            var item = result.destinations.Find(p => p.code == "ALT").maxPriceDiff;
            Assert.Equal(ALDestinationMinDiff.ToString(), item.ToString());
        }
        #endregion

        #region AlternativeDestinationRatingMaxDiff
        [Fact]
        public async void Alternative_Destination_Rating_MaxDiff()
        {
            await GetResultAsync();
            double ALTDestinationRatingMaxDiff = -0.40;
            var items = result.destinations.Find(p => p.code == "ALT").maxRatingDiff;
            Assert.Equal(ALTDestinationRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region AlternativeDestinationRatingMinDiff
        [Fact]
        public async void Alternative_Destination_Rating_MinDiff()
        {
            await GetResultAsync();
            double ALTDestinationRatingMaxDiff = -0.40;
            var items = result.destinations.Find(p => p.code == "ALT").minRatingDiff;
            Assert.Equal(ALTDestinationRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region AlternativeDestinationValueMaxDiff
        [Fact]
        public async Task Alternative_Destination_Value_MaxDiff()
        {
            await GetResultAsync();
            double AlternativeDestinationValueMaxDiff = 0.54;
            var items = result.destinations.Find(p => p.code == "ALT").maxValueDiff;
            Assert.Equal(AlternativeDestinationValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region AlternativeDestinationValueMinDiff
        [Fact]
        public async Task Alternative_Destination_Value_MinDiff()
        {
            await GetResultAsync();
            double AlternativeDestinationValueMaxDiff = 0.54;
            var items = result.destinations.Find(p => p.code == "ALT").minValueDiff;
            Assert.Equal(AlternativeDestinationValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region OtherAlternativeDestinationMinPriceDiff
        [Fact]
        public async void OtherAlternative_Destination_PriceMin()
        {
            await GetResultAsync();
            double OtherAlternativeDestinationMin = -600;
            var item = result.destinations.Find(p => p.code == "OTHERALT").minPriceDiff;
            Assert.Equal(OtherAlternativeDestinationMin.ToString(), item.ToString());
        }
        #endregion

        #region OtherAlternativeDestinationMaxPrice
        [Fact]
        public async void OtherAlternative_Destination_Price_Max()
        {
            await GetResultAsync();
            double OtherAlternativeTestDestinationMin = -600.00;
            var item = result.destinations.Find(p => p.code == "OTHERALT").maxPriceDiff;
            Assert.Equal(OtherAlternativeTestDestinationMin.ToString(), item.ToString());
        }
        #endregion

        #region OtherAlternativeDestinationRatingMaxDiff
        [Fact]
        public async void OtherAlternative_Destination_Rating_MaxDiff()
        {
            await GetResultAsync();
            double OtherDestinationRatingMaxDiff = 0.70;
            var items = result.destinations.Find(p => p.code == "OTHERALT").maxRatingDiff;
            Assert.Equal(OtherDestinationRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region OtherAlternativeDestinationRatingMinDiff
        [Fact]
        public async void OtherAlternative_Destination_Rating_MinDiff()
        {
            await GetResultAsync();
            double OtherDestinationRatingMaxDiff = 0.70;
            var items = result.destinations.Find(p => p.code == "OTHERALT").minRatingDiff;
            Assert.Equal(OtherDestinationRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region OtherAlternativeDestinationValueMaxDiff
        [Fact]
        public async Task OtherAlternative_Destination_Value_MaxDiff()
        {
            await GetResultAsync();
            double OtherAlternativeDestinationValueMaxDiff = 1.13;
            var items = result.destinations.Find(p => p.code == "OTHERALT").maxValueDiff;
            Assert.Equal(OtherAlternativeDestinationValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region OtherAlternativeDestinationValueMinDiff
        [Fact]
        public async Task OtherAlternative_Destination_Value_MinDiff()
        {
            await GetResultAsync();
            double OtherAlternativeDestinationValueMaxDiff = 1.13;
            var items = result.destinations.Find(p => p.code == "OTHERALT").minValueDiff;
            Assert.Equal(OtherAlternativeDestinationValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DateMinusTwoValueMinDiff
        [Fact]
        public async Task Date_MinusTwo_Value_MinDiff()
        {
            await GetResultAsync();
            double DateMinusTwoValueMinDiff = 0.01;
            var items = result.dateFlexibility.Find(p => p.code == -2).minValueDiff;
            Assert.Equal(DateMinusTwoValueMinDiff.ToString(), items.ToString());
        }
        #endregion

        // 
        #region DateMinusTwoValueMaxDiff
        [Fact]
        public async Task Date_MinusTwo_Value_MaxDiff()
        {
            await GetResultAsync();
            double DateMaxTwoValueMaxDiff = 0.54;
            var items = result.dateFlexibility.Find(p => p.code == -2).maxValueDiff;
            Assert.Equal(DateMaxTwoValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DateMinusTwoRatingMaxDiff
        [Fact]
        public async Task Date_MinusTwo_Rating_MaxDiff()
        {
            await GetResultAsync();
            double DateMinusTwoRatingMaxDiff = 0.70;
            var items = result.dateFlexibility.Find(p => p.code == -2).maxRatingDiff;
            Assert.Equal(DateMinusTwoRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DateMinusTwoRatingMinDiff
        [Fact]
        public async Task Date_MinusTwo_Rating_MinDiff()
        {
            await GetResultAsync();
            double DateMinusTwoRatingMinDiff = -0.40;
            var items = result.dateFlexibility.Find(p => p.code == -2).minRatingDiff;
            Assert.Equal(DateMinusTwoRatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        //note excell wrong

        #region DateMinusTwoPriceMinDiff
        [Fact]
        public async Task Date_MinusTwo_Price_MinDiff()
        {
            await GetResultAsync();
            double DateMinusTwoRatingMaxDiff = -700;
            var items = result.dateFlexibility.Find(p => p.code == -2).minPriceDiff;
            Assert.Equal(DateMinusTwoRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DateMinusTwoPriceMaxDiff
        [Fact]
        public async Task Date_MinusTwo_Price_MaxDiff()
        {
            await GetResultAsync();
            double DateMinusTwoPriceMaxDiff = 400;
            var items = result.dateFlexibility.Find(p => p.code == -2).maxPriceDiff;
            Assert.Equal(DateMinusTwoPriceMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DateMinusOnePriceMinDiff
        [Fact]
        public async Task Date_MinusOne_Price_MinDiff()
        {
            await GetResultAsync();
            double DateMinusOnePriceMinDiff = -300;
            var items = result.dateFlexibility.Find(p => p.code == -1).minPriceDiff;
            Assert.Equal(DateMinusOnePriceMinDiff.ToString(), items.ToString());
        }
        #endregion

        // Note 
        #region DateMinusOnePriceMaxDiff
        [Fact]
        public async Task Date_MinusOne_Price_MaxDiff()
        {
            await GetResultAsync();
            double DateMinusOnePriceMaxDiff = 400;
            var items = result.dateFlexibility.Find(p => p.code == -1).maxPriceDiff;
            Assert.Equal(DateMinusOnePriceMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DateMinusOneRatingMaxDiff
        [Fact]
        public async Task Date_MinusOne_Rating_MaxDiff()
        {
            await GetResultAsync();
            double DateMinusOneRatingMaxDiff = 0.60;
            var items = result.dateFlexibility.Find(p => p.code == -1).minRatingDiff;
            Assert.Equal(DateMinusOneRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        // 
        #region DateMinusOneRatingMinDiff
        [Fact]
        public async Task Date_MinusOne_Rating_MinDiff()
        {
            await GetResultAsync();
            double DateMinusOneRatingMinDiff = 0.60;
            var items = result.dateFlexibility.Find(p => p.code == -1).minRatingDiff;
            Assert.Equal(DateMinusOneRatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DateMinusOneValueMinDiff
        [Fact]
        public async Task Date_MinusOne_Value_MinDiff()
        {
            await GetResultAsync();
            double DateMinusOneValueMinDiff = 0.01;
            var items = result.dateFlexibility.Find(p => p.code == -1).minValueDiff;
            Assert.Equal(DateMinusOneValueMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DateMinusOneValueMaxDiff
        [Fact]
        public async Task Date_MinusOne_Value_MaxDiff()
        {
            await GetResultAsync();
            double DateMinusOneValueMaxDiff = 0.61;
            var items = result.dateFlexibility.Find(p => p.code == -1).maxValueDiff;
            Assert.Equal(DateMinusOneValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePriceMinDiff
        [Fact]
        public async Task Date_Price_MinDiff()
        {
            await GetResultAsync();
            double DatePriceMinDiff = 200;
            var items = result.dateFlexibility.Find(p => p.code == 0).minPriceDiff;
            Assert.Equal(DatePriceMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePriceMaxDiff
        [Fact]
        public async Task Date_Price_MaxDiff()
        {
            await GetResultAsync();
            double DatePriceMaxDiff = 400;
            var items = result.dateFlexibility.Find(p => p.code == 0).maxPriceDiff;
            Assert.Equal(DatePriceMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DateRatingMinDiff
        [Fact]
        public async Task Date_Rating_MinDiff()
        {
            await GetResultAsync();
            double DateRatingMinDiff = 0.60;
            var items = result.dateFlexibility.Find(p => p.code == 0).minRatingDiff;
            Assert.Equal(DateRatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DateRatingMaxDiff
        [Fact]
        public async Task Date_Rating_MaxDiff()
        {
            await GetResultAsync();
            double DateRatingMaxDiff = 0.70;
            var items = result.dateFlexibility.Find(p => p.code == 0).maxRatingDiff;
            Assert.Equal(DateRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        // excell wrong
        #region DateValueMaxDiff
        [Fact]
        public async Task Date_Value_MaxDiff()
        {
            await GetResultAsync();
            double DateValueMaxDiff = 0.11;
            var items = result.dateFlexibility.Find(p => p.code == 0).maxValueDiff;
            Assert.Equal(DateValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatevalueMinDiff
        [Fact]
        public async Task Date_value_MinDiff()
        {
            await GetResultAsync();
            double DatePriceMinDiff = 0.01;
            var items = result.dateFlexibility.Find(p => p.code == 0).minValueDiff;
            Assert.Equal(DatePriceMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneMinPrice
        [Fact]
        public async Task Date_PlusOne_MinPrice()
        {
            await GetResultAsync();
            double DatePlusvalueMinPrice = -1100;
            var items = result.dateFlexibility.Find(p => p.code == 1).minPriceDiff;
            Assert.Equal(DatePlusvalueMinPrice.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneMaxPrice
        [Fact]
        public async Task Date_PlusOne_MaxPrice()
        {
            await GetResultAsync();
            double DatePlusvalueMaxPrice = 400;
            var items = result.dateFlexibility.Find(p => p.code == 1).maxPriceDiff;
            Assert.Equal(DatePlusvalueMaxPrice.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneRatingMinDiff
        [Fact]
        public async Task Date_PlusOne_Rating_MinDiff()
        {
            await GetResultAsync();
            double DatePlusRatingMinDiff = -0.50;
            var items = result.dateFlexibility.Find(p => p.code == 1).minRatingDiff;
            Assert.Equal(DatePlusRatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneRatingMaxDiff
        [Fact]
        public async Task Date_PlusOne_Rating_MaxDiff()
        {
            await GetResultAsync();
            double DatePlusRatingMaxDiff = 0.70;
            var items = result.dateFlexibility.Find(p => p.code == 1).maxRatingDiff;
            Assert.Equal(DatePlusRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneValueMaxDiff
        [Fact]
        public async Task Date_PlusOne_Value_MaxDiff()
        {
            await GetResultAsync();
            double DatePlusValueMaxDiff = 1.33;
            var items = result.dateFlexibility.Find(p => p.code == 1).maxValueDiff;
            Assert.Equal(DatePlusValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneValueMinDiff
        [Fact]
        public async Task Date_PlusOne_Value_MinDiff()
        {
            await GetResultAsync();
            double DatePlusValueMinDiff = 0.01;
            var items = result.dateFlexibility.Find(p => p.code == 1).minValueDiff;
            Assert.Equal(DatePlusValueMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusTwoPriceMinDiff
        [Fact]
        public async Task Date_PlusTwo_Price_MinDiff()
        {
            await GetResultAsync();
            double DatePlusTwoPriceMinDiff = -200;
            var items = result.dateFlexibility.Find(p => p.code == 2).minPriceDiff;
            Assert.Equal(DatePlusTwoPriceMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusTwoPriceMaxDiff
        [Fact]
        public async Task Date_PlusTwo_Price_MaxDiff()
        {
            await GetResultAsync();
            double DatePlusTwoPriceMaxDiff = 200;
            var items = result.dateFlexibility.Find(p => p.code == 2).maxPriceDiff;
            Assert.Equal(DatePlusTwoPriceMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusTwoRatingMaxDiff
        [Fact]
        public async Task Date_PlusTwo_Rating_MaxDiff()
        {
            await GetResultAsync();
            double DatePlusTwoRatingMaxDiff = 0.70;
            var items = result.dateFlexibility.Find(p => p.code == 2).maxRatingDiff;
            Assert.Equal(DatePlusTwoRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusTwoRatingMaxDiff
        [Fact]
        public async Task Date_PlusTwo_Rating_MinDiff()
        {
            await GetResultAsync();
            double DatePlusTwoRatingMinDiff = 0.60;
            var items = result.dateFlexibility.Find(p => p.code == 2).minRatingDiff;
            Assert.Equal(DatePlusTwoRatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusTwoValueMaxDiff
        [Fact]
        public async Task Date_PlusTwo_Value_MaxDiff()
        {
            await GetResultAsync();
            double DatePlusTwoValueMaxDiff = 0.54;
            var items = result.dateFlexibility.Find(p => p.code == 2).maxValueDiff;
            Assert.Equal(DatePlusTwoValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        // 
        #region DatePlusTwoValueMaxDiff
        [Fact]
        public async Task Date_PlusTwo_Value_MinDiff()
        {
            await GetResultAsync();
            double DatePlusTwoValueMinDiff = 0.11;
            var items = result.dateFlexibility.Find(p => p.code == 2).minValueDiff;
            Assert.Equal(DatePlusTwoValueMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_AA_PriceMinDiff
        [Fact]
        public async Task Airline_AA_PriceMinDiff()
        {
            await GetResultAsync();
            double Airline_AA_PriceMinDiff = -1100;
            var items = result.airlines.Find(p => p.code == "AA").minPriceDiff;
            Assert.Equal(Airline_AA_PriceMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_AA_PriceMaxDiff
        [Fact]
        public async Task Airline_AA_PriceMaxDiff()
        {
            await GetResultAsync();
            double Airline_AA_PriceMaxDiff = 200;
            var items = result.airlines.Find(p => p.code == "AA").maxPriceDiff;
            Assert.Equal(Airline_AA_PriceMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_AA_RatingMaxDiff
        [Fact]
        public async Task Airline_AA_RatingMaxDiff()
        {
            await GetResultAsync();
            double Airline_AA_RatingMaxDiff = 0.60;
            var items = result.airlines.Find(p => p.code == "AA").maxRatingDiff;
            Assert.Equal(Airline_AA_RatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_AA_RatingMinDiff
        [Fact]
        public async Task Airline_AA_RatingMinDiff()
        {
            await GetResultAsync();
            double Airline_AA_RatingMinDiff = -0.50;
            var items = result.airlines.Find(p => p.code == "AA").minRatingDiff;
            Assert.Equal(Airline_AA_RatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_AA_ValueMaxDiff
        [Fact]
        public async Task Airline_AA_ValueMaxDiff()
        {
            await GetResultAsync();
            double Airline_AA_ValueMaxDiff = 1.33;
            var items = result.airlines.Find(p => p.code == "AA").maxValueDiff;
            Assert.Equal(Airline_AA_ValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_AA_ValueMinDiff
        [Fact]
        public async Task Airline_AA_ValueMinDiff()
        {
            await GetResultAsync();
            double Airline_AA_ValueMinDiff = 0.11;
            var items = result.airlines.Find(p => p.code == "AA").minValueDiff;
            Assert.Equal(Airline_AA_ValueMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_BA_ValueMinDiff
        [Fact]
        public async Task Airline_BA_ValueMinDiff()
        {
            await GetResultAsync();
            double Airline_BA_ValueMinDiff = 0.01;
            var items = result.airlines.Find(p => p.code == "BA").minValueDiff;
            Assert.Equal(Airline_BA_ValueMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_BA_ValueMaxDiff
        [Fact]
        public async Task Airline_BA_ValueMaxDiff()
        {
            await GetResultAsync();
            double Airline_BA_ValueMaxDiff = 1.13;
            var items = result.airlines.Find(p => p.code == "BA").maxValueDiff;
            Assert.Equal(Airline_BA_ValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_BA_PriceMinDiff
        [Fact]
        public async Task Airline_BA_PriceMinDiff()
        {
            await GetResultAsync();
            double Airline_BA_PriceMinDiff = -600;
            var items = result.airlines.Find(p => p.code == "BA").minPriceDiff;
            Assert.Equal(Airline_BA_PriceMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_BA_PriceMaxDiff
        [Fact]
        public async Task Airline_BA_PriceMaxDiff()
        {
            await GetResultAsync();
            double Airline_BA_PriceMaxDiff = 400;
            var items = result.airlines.Find(p => p.code == "BA").maxPriceDiff;
            Assert.Equal(Airline_BA_PriceMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_BA_RatingMaxDiff
        [Fact]
        public async Task Airline_BA_RatingMaxDiff()
        {
            await GetResultAsync();
            double Airline_BA_RatingMaxDiff = 0.70;
            var items = result.airlines.Find(p => p.code == "BA").maxRatingDiff;
            Assert.Equal(Airline_BA_RatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_BA_RatingMinDiff
        [Fact]
        public async Task Airline_BA_RatingMinDiff()
        {
            await GetResultAsync();
            double Airline_BA_RatingMinDiff = 0.70;
            var items = result.airlines.Find(p => p.code == "BA").minRatingDiff;
            Assert.Equal(Airline_BA_RatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        //#region Cabin_Y_RatingMinDiff
        //[Fact]
        //public async Task Cabin_Y_RatingMinDiff()
        //{
        //    await GetResultAsync();
        //    double Cabin_Y_RatingMinDiff = 0;
        //    var items = result.cabins.Find(p => p.code == "ECONOMY").minRatingDiff;
        //    Assert.Equal(Cabin_Y_RatingMinDiff.ToString(), items.ToString());
        //}
        //#endregion

        //#region Cabin_Y_RatingMaxDiff
        //[Fact]
        //public async Task Cabin_Y_RatingMaxDiff()
        //{
        //    await GetResultAsync();
        //    double Cabin_Y_RatingMaxDiff = 0;
        //    var items = result.cabins.Find(p => p.code == "ECONOMY").maxRatingDiff;
        //    Assert.Equal(Cabin_Y_RatingMaxDiff.ToString(), items.ToString());
        //}
        //#endregion

        //#region Cabin_Y_RatingMaxDiff
        //[Fact]
        //public async Task Cabin_Y_PriceMaxDiff()
        //{
        //    await GetResultAsync();
        //    double Cabin_Y_PriceMaxDiff = 0;
        //    var itemsx = result.cabins.Find(p => p.code == "ECONOMY");
        //    var items = result.cabins.Find(p => p.code == "ECONOMY").maxPriceDiff;
        //    Assert.Equal(Cabin_Y_PriceMaxDiff.ToString(), items.ToString());
        //}
        //#endregion

        //#region Cabin_Y_PriceMinDiff
        //[Fact]
        //public async Task Cabin_Y_PriceMinDiff()
        //{
        //    await GetResultAsync();
        //    double Cabin_Y_PriceMinDiff = 0;
        //    var items = result.cabins.Find(p => p.code == "ECONOMY").minPriceDiff;
        //    Assert.Equal(Cabin_Y_PriceMinDiff.ToString(), items.ToString());
        //}
        //#endregion

        //#region Cabin_Y_ValueMinDiff
        //[Fact]
        //public async Task Cabin_Y_ValueMinDiff()
        //{
        //    await GetResultAsync();
        //    double Cabin_Y_ValueMinDiff = 0;
        //    var items = result.cabins.Find(p => p.code == "ECONOMY").minValueDiff;
        //    Assert.Equal(Cabin_Y_ValueMinDiff.ToString(), items.ToString());
        //}
        //#endregion

        //#region Cabin_Y_ValueMaxDiff
        //[Fact]
        //public async Task Cabin_Y_ValueMaxDiff()
        //{
        //    await GetResultAsync();
        //    double Cabin_Y_ValueMaxDiff = 0;
        //    var items = result.cabins.Find(p => p.code == "ECONOMY").maxValueDiff;
        //    Assert.Equal(Cabin_Y_ValueMaxDiff.ToString(), items.ToString());
        //}
        //#endregion

        #region Cabin_PE_ValueMaxDiff
        [Fact]
        public async Task Cabin_PE_ValueMaxDiff()
        {
            await GetResultAsync();
            double Cabin_PE_ValueMaxDiff = 1.33;
            var items = result.cabins.Find(p => p.code == "PREMIUM_ECONOMY").maxValueDiff;
            Assert.Equal(Cabin_PE_ValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_PE_ValueMinDiff
        [Fact]
        public async Task Cabin_PE_ValueMinDiff()
        {
            await GetResultAsync();
            double Cabin_PE_ValueMinDiff = 0.40;
            var items = result.cabins.Find(p => p.code == "PREMIUM_ECONOMY").minValueDiff;
            Assert.Equal(Cabin_PE_ValueMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_PE_PriceMinDiff
        [Fact]
        public async Task Cabin_PE_PriceMinDiff()
        {
            await GetResultAsync();
            double Cabin_PE_PriceMinDiff = -1100;
            var items = result.cabins.Find(p => p.code == "PREMIUM_ECONOMY").minPriceDiff;
            Assert.Equal(Cabin_PE_PriceMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_PE_PriceMaxDiff
        [Fact]
        public async Task Cabin_PE_PriceMaxDiff()
        {
            await GetResultAsync();
            double Cabin_PE_PriceMaxDiff = -600;
            var items = result.cabins.Find(p => p.code == "PREMIUM_ECONOMY").maxPriceDiff;
            Assert.Equal(Cabin_PE_PriceMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_PE_RatingMaxDiff
        [Fact]
        public async Task Cabin_PE_RatingMaxDiff()
        {
            await GetResultAsync();
            double Cabin_PE_RatingMaxDiff = -0.40;
            var items = result.cabins.Find(p => p.code == "PREMIUM_ECONOMY").maxRatingDiff;
            Assert.Equal(Cabin_PE_RatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_PE_RatingMaxDiff
        [Fact]
        public async Task Cabin_PE_RatingMinDiff()
        {
            await GetResultAsync();
            double Cabin_PE_RatingMinDiff = -0.50;
            var items = result.cabins.Find(p => p.code == "PREMIUM_ECONOMY").minRatingDiff;
            Assert.Equal(Cabin_PE_RatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_J_RatingMaxDiff
        [Fact]
        public async Task Cabin_J_RatingMaxDiff()
        {
            await GetResultAsync();
            double Cabin_J_RatingMaxDiff = 0.70;
            var items = result.cabins.Find(p => p.code == "BUSINESS").maxRatingDiff;
            Assert.Equal(Cabin_J_RatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_J_RatingMinDiff
        [Fact]
        public async Task Cabin_J_RatingMinDiff()
        {
            await GetResultAsync();
            double Cabin_J_RatingMinDiff = 0.60;
            var items = result.cabins.Find(p => p.code == "BUSINESS").minRatingDiff;
            Assert.Equal(Cabin_J_RatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_J_ValueMaxDiff
        [Fact]
        public async Task Cabin_J_ValueMaxDiff()
        {
            await GetResultAsync();
            double Cabin_J_ValueMaxDiff = 1.13;
            var items = result.cabins.Find(p => p.code == "BUSINESS").maxValueDiff;
            Assert.Equal(Cabin_J_ValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_J_ValueMinDiff
        [Fact]
        public async Task Cabin_J_ValueMinDiff()
        {
            await GetResultAsync();
            double Cabin_J_ValueMinDiff = 0.01;
            var items = result.cabins.Find(p => p.code == "BUSINESS").minValueDiff;
            Assert.Equal(Cabin_J_ValueMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_J_PriceMinDiff
        [Fact]
        public async Task Cabin_J_PriceMinDiff()
        {
            await GetResultAsync();
            double Cabin_J_PriceMinDiff = -600;
            var items = result.cabins.Find(p => p.code == "BUSINESS").minPriceDiff;
            Assert.Equal(Cabin_J_PriceMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_J_PriceMaxDiff
        [Fact]
        public async Task Cabin_J_PriceMaxDiff()
        {
            await GetResultAsync();
            double Cabin_J_PriceMaxDiff = 400;
            var items = result.cabins.Find(p => p.code == "BUSINESS").maxPriceDiff;
            Assert.Equal(Cabin_J_PriceMaxDiff.ToString(), items.ToString());
        }
        #endregion
    }
}
