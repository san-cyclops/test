using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Offers.Models.BaseEntities;
using Offers.Models.Common;
using Offers.Services.Alliances;
using Offers.Services.FileSave;
using Offers.Services.Signals;
using Offers.Services.UserRating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class SignalsUpgradesTest_Trash
    {
        public static QuadrantsRS result { get; set; }
      

        #region GetResponse
        public static async Task<QuadrantsRS> GetResultAsync()
        {
            List<SignalsBucketRs_v2> SignalsBucketRs_v2 = new List<SignalsBucketRs_v2>
            {
                new SignalsBucketRs_v2
                {
                    Id="01",
                    //Alliances="AA",
                    OfferPrice=1000,
                   // Date=((int)DateTime.Parse( "3/31/2021").Subtract(DateTime.Parse("4/8/2021")).TotalDays),
                    //UserRating =4.5,
                    //RatingDifference=2.5,
                    //PriceDifference=-51,
                    //ValueDifference=2.741833509,
                    //ValueRatio=4.741833509,
                    CabinClass="FF",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LHR",Date="03-31-2021",UserRating= 4,OperatingAirline="AA"},
                    Inbound= new SignalsBucketRs_Inbound{ UserRating=5 }
                },
                 new SignalsBucketRs_v2
                {
                    Id="02",
                   // Alliances="BA",
                      OfferPrice=900,
                   // Date=((int)DateTime.Parse( "3/31/2021").Subtract(DateTime.Parse("4/8/2021")).TotalDays),
                    //UserRating =1.5,
                    //RatingDifference=-0.5,
                    //PriceDifference=-50,
                    //ValueDifference=1.5789,
                    //ValueRatio=-0.421052632,
                    CabinClass="EE",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LHR",Date="03-31-2021",UserRating=1,OperatingAirline="BA"},
                     Inbound= new SignalsBucketRs_Inbound{ UserRating=2 }
                },
                  new SignalsBucketRs_v2
                {
                    Id="03",
                  //  Alliances="AA",
                      OfferPrice=800,
                   // Date=((int)DateTime.Parse( "3/31/2021").Subtract(DateTime.Parse("4/8/2021")).TotalDays),
                    //UserRating =3.25,
                    //RatingDifference=1.25,
                    //PriceDifference=-49,
                    //ValueDifference=3.4175,
                    //ValueRatio=1.41746,
                    CabinClass="PB",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LCY",Date="04-01-2021",UserRating=3,OperatingAirline="AA"},
                      Inbound= new SignalsBucketRs_Inbound{ UserRating=3.5 }
                },
                   new SignalsBucketRs_v2
                {
                    Id="04",
                  //  Alliances="BA",
                      OfferPrice=750,
                  //  Date=((int)DateTime.Parse( "4/1/2021").Subtract(DateTime.Parse("4/8/2021")).TotalDays),
                    //UserRating =1.9,
                    //RatingDifference=-0.1,
                    //PriceDifference=-1,
                    //ValueDifference=1.901901902,
                    //ValueRatio=-0.098098098,
                    CabinClass="BB",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LHR",Date="04-01-2021",UserRating=4,OperatingAirline="BA"},
                     Inbound= new SignalsBucketRs_Inbound{ UserRating=3.5 }
                },
                    new SignalsBucketRs_v2
                {
                    Id="05",
                  //  Alliances="BA",
                      OfferPrice=1000,
                  //  Date=((int)DateTime.Parse( "4/1/2021").Subtract(DateTime.Parse("4/8/2021")).TotalDays),
                    //UserRating =5,
                    //RatingDifference=3,
                    //PriceDifference=0,
                    //ValueDifference=3,
                    //ValueRatio=5,
                    CabinClass="BB",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LHR",Date="04-01-2021",UserRating=5,OperatingAirline="BA"},
                     Inbound= new SignalsBucketRs_Inbound{ UserRating=5 }
                },
                     new SignalsBucketRs_v2
                {
                    Id="07",
                 //   Alliances="AA",
                      OfferPrice=1100,
                  //  Date=((int)DateTime.Parse( "4/1/2021").Subtract(DateTime.Parse("4/8/2021")).TotalDays),
                    //UserRating =3.5,
                    //RatingDifference=1.5,
                    //PriceDifference=0,
                    //ValueDifference=1.5,
                    //ValueRatio=3.5,
                    CabinClass="BP",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LHR",Date="04-01-2021", UserRating=1,OperatingAirline="AA"},
                    Inbound= new SignalsBucketRs_Inbound{ UserRating=1 }
                },
                      new SignalsBucketRs_v2
                {
                    Id="08",
                  //  Alliances="AA",
                      OfferPrice=950,
                  //  Date=((int)DateTime.Parse( "4/1/2021").Subtract(DateTime.Parse("4/8/2021")).TotalDays),
                    //UserRating =2,
                    //RatingDifference=0,
                    //PriceDifference=0,
                    //ValueDifference=0,
                    //ValueRatio=2,
                    CabinClass="EB",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LHR",Date="04-01-2021",UserRating=1,OperatingAirline="AA" },
                     Inbound= new SignalsBucketRs_Inbound{ UserRating=3 }

                },
                       new SignalsBucketRs_v2
                {
                    Id="09",
                  //  Alliances="AA",
                      OfferPrice=780,
                  //  Date=((int)DateTime.Parse( "4/1/2021").Subtract(DateTime.Parse("4/8/2021")).TotalDays),
                    //UserRating =3,
                    //RatingDifference=1,
                    //PriceDifference= 1,
                    //ValueDifference=0.997,
                    //ValueRatio=2.997,
                    CabinClass="PP",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LHR",Date="04-01-2021",UserRating=3 ,OperatingAirline="AA"},
                       Inbound= new SignalsBucketRs_Inbound{ UserRating=3 }
                },
                           new SignalsBucketRs_v2
                {
                    Id="10",
                  //  Alliances="AA",
                      OfferPrice=970,
                 //   Date=((int)DateTime.Parse( "4/2/2021").Subtract(DateTime.Parse("4/9/2021")).TotalDays),
                    //UserRating =4.5,
                    //RatingDifference=2.5,
                    //PriceDifference=-51,
                    //ValueDifference=2.28164,
                    //ValueRatio=4.2816,
                    CabinClass="PB",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LHR",Date="04-02-2021", UserRating=4,OperatingAirline="BA"},
                     Inbound= new SignalsBucketRs_Inbound{ UserRating=3.5 }
                },
                            new SignalsBucketRs_v2
                {
                    Id="11",
                  //  Alliances="AA",
                      OfferPrice=995,
                 //   Date=((int)DateTime.Parse( "4/2/2021").Subtract(DateTime.Parse("4/9/2021")).TotalDays),
                    //UserRating =1.8,
                    //RatingDifference=-0.2,
                    //PriceDifference=100,
                    //ValueDifference=-0.3636,
                    //ValueRatio=1.6364,
                    CabinClass="BB",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LHR",Date="04-02-2021", UserRating=5,OperatingAirline="BA"},
                     Inbound= new SignalsBucketRs_Inbound{ UserRating=4 }
                },
                             new SignalsBucketRs_v2
                {
                    Id="12",
                  //  Alliances="AA",
                      OfferPrice=812,
                  //  Date=((int)DateTime.Parse( "4/2/2021").Subtract(DateTime.Parse("4/9/2021")).TotalDays),
                    //UserRating =5,
                    //RatingDifference=3,
                    //PriceDifference=200,
                    //ValueDifference=2.16667,
                    //ValueRatio=4.1667,
                    CabinClass="FF",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LHR",Date="04-02-2021",UserRating=5,OperatingAirline="AA"},
                    Inbound= new SignalsBucketRs_Inbound{ UserRating=5 }
                },
                              new SignalsBucketRs_v2
                {
                    Id="13",
                  //  Alliances="BA",
                    OfferPrice=1200,
                    //UserRating =1.8,
                    //RatingDifference=-0.2,
                    //PriceDifference=-100,
                    //ValueDifference=-0.3636,
                    //ValueRatio=1.6364,
                    CabinClass="BE",
                    Outbound = new SignalsBucketRs_Outbound{ Destination = "LHR" ,Date="04-02-2021",UserRating=5,OperatingAirline="BA"},
                    Inbound= new SignalsBucketRs_Inbound{ UserRating=5 }
                },

            };
          

            var mock = new Mock<ILogger<SignalsPaidService>>();
            ILogger<SignalsPaidService> MockPaidSignal = mock.Object;

            var mockf = new Mock<IGetAlliances>();
            IGetAlliances MockALLiances = mockf.Object;

            var mocks = new Mock<IFileSaveService<FileSaveBaseEntity>>();
            IFileSaveService<FileSaveBaseEntity> MockFileSave = mocks.Object;

            var mocku = new Mock<IUserRatingService<UserRatingBaseEntity>>();
            IUserRatingService<UserRatingBaseEntity> MockUserRating = mocku.Object;

            var controller = new SignalsPaidService(MockPaidSignal, MockFileSave, MockUserRating, MockALLiances);

           List<SignalsBucketRs_v2> SignalsBucketRs_v2List = await controller.SignalsPaidBucketLogic(SignalsBucketRs_v2, "CONTEXT", "UnitTest", "AA,F950,1,EB,2,950,2021-04-01");

            QuadrantsRS QuadrantsRSObj = await controller.ResponseBuilder(SignalsBucketRs_v2List, "UnitTest", "AA,F950,1,EB,2,950,2021-04-01");
            result = QuadrantsRSObj;
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
            double DestinationMinDiff = 150;
            var item = result.trash.destinations.Find(p => p.code == "LHR").minPriceDiff;
            Assert.Equal(DestinationMinDiff.ToString(), item.ToString());
        }
        #endregion

        #region DestinationMaxDiff
        [Fact]
        public async void Destination_Price_MaxDiff()
        {
            await GetResultAsync();
            double DestinationMaxDiff = 150;
            var items = result.trash.destinations.Find(p => p.code == "LHR").maxPriceDiff;
            Assert.Equal(DestinationMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DestinationRatingMaxDiff
        [Fact]
        public async void Destination_Rating_MaxDiff()
        {
            await GetResultAsync();
            double TestDestinationRatingMaxDiff = -1;
            var items = result.trash.destinations.Find(p => p.code == "LHR").maxRatingDiff;
            Assert.Equal(TestDestinationRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DestinationRatingMinDiff
        [Fact]
        public async Task Destination_Rating_MinDiff()
        {
            await GetResultAsync();
            double TestDestinationRatingMinDiff = -1;
            var items = result.trash.destinations.Find(p => p.code == "LHR").minRatingDiff;
            Assert.Equal(TestDestinationRatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DestinationValueMaxDiff
        [Fact]
        public async Task Destination_Value_MaxDiff()
        {
            await GetResultAsync();
            double TestDestinationValueMaxDiff = -1.196172249;
            var items = result.trash.destinations.Find(p => p.code == "LHR").maxValueDiff;
            Assert.Equal(TestDestinationValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneMinPrice
        [Fact]
        public async Task Date_PlusOne_MinPrice()
        {
            await GetResultAsync();
            double DatePlusvalueMinPrice = 150;
            var items = result.trash.dateFlexibility.Find(p => p.code == 0).minPriceDiff;
            Assert.Equal(DatePlusvalueMinPrice.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneMaxPrice
        [Fact]
        public async Task Date_PlusOne_MaxPrice()
        {
            await GetResultAsync();
            double DatePlusvalueMaxPrice = 150;
            var items = result.trash.dateFlexibility.Find(p => p.code ==0).maxPriceDiff;
            Assert.Equal(DatePlusvalueMaxPrice.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneRatingMinDiff
        [Fact]
        public async Task Date_PlusOne_Rating_MinDiff()
        {
            await GetResultAsync();
            double DatePlusRatingMinDiff = -1;
            var items = result.trash.dateFlexibility.Find(p => p.code == 0).minRatingDiff;
            Assert.Equal(DatePlusRatingMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneRatingMaxDiff
        [Fact]
        public async Task Date_PlusOne_Rating_MaxDiff()
        {
            await GetResultAsync();
            double DatePlusRatingMaxDiff = -1;
            var items = result.trash.dateFlexibility.Find(p => p.code == 0).maxRatingDiff;
            Assert.Equal(DatePlusRatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneValueMaxDiff
        [Fact]
        public async Task Date_PlusOne_Value_MaxDiff()
        {
            await GetResultAsync();
            double DatePlusValueMaxDiff = -1.196172249;
            var items = result.trash.dateFlexibility.Find(p => p.code == 0).maxValueDiff;
            Assert.Equal(DatePlusValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region DatePlusOneValueMinDiff
        [Fact]
        public async Task Date_PlusOne_Value_MinDiff()
        {
            await GetResultAsync();
            double DatePlusValueMinDiff = -1.196172249;
            var items = result.trash.dateFlexibility.Find(p => p.code == 0).minValueDiff;
            Assert.Equal(DatePlusValueMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_BA_ValueMinDiff
        [Fact]
        public async Task Airline_AA_ValueMinDiff()
        {
            await GetResultAsync();
            double Airline_BA_ValueMinDiff = -1.196172249;
            var items = result.trash.airlines.Find(p => p.code == "AA").minValueDiff;
            Assert.Equal(Airline_BA_ValueMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_BA_ValueMaxDiff
        [Fact]
        public async Task Airline_AA_ValueMaxDiff()
        {
            await GetResultAsync();
            double Airline_BA_ValueMaxDiff = -1.196172249;
            var items = result.trash.airlines.Find(p => p.code == "AA").maxValueDiff;
            Assert.Equal(Airline_BA_ValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_BA_PriceMinDiff
        [Fact]
        public async Task Airline_AA_PriceMinDiff()
        {
            await GetResultAsync();
            double Airline_BA_PriceMinDiff = 150;
            var items = result.trash.airlines.Find(p => p.code == "AA").minPriceDiff;
            Assert.Equal(Airline_BA_PriceMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_BA_PriceMaxDiff
        [Fact]
        public async Task Airline_AA_PriceMaxDiff()
        {
            await GetResultAsync();
            double Airline_BA_PriceMaxDiff = 150;
            var items = result.trash.airlines.Find(p => p.code == "AA").maxPriceDiff;
            Assert.Equal(Airline_BA_PriceMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_BA_RatingMaxDiff
        [Fact]
        public async Task Airline_AA_RatingMaxDiff()
        {
            await GetResultAsync();
            double Airline_BA_RatingMaxDiff = -1;
            var items = result.trash.airlines.Find(p => p.code == "AA").maxRatingDiff;
            Assert.Equal(Airline_BA_RatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Airline_AA_RatingMinDiff
        [Fact]
        public async Task Airline_BA_RatingMinDiff()
        {
            await GetResultAsync();
            double Airline_BA_RatingMinDiff = -1;
            var items = result.trash.airlines.Find(p => p.code == "AA").minRatingDiff;
            Assert.Equal(Airline_BA_RatingMinDiff.ToString(), items.ToString());
        }
        #endregion


        #region Cabin_PB_PriceMinDiff
        [Fact]
        public async Task Cabin_PB_PriceMinDiff()
        {
            await GetResultAsync();
            double Cabin_PE_PriceMinDiff = 150;
            var items = result.trash.cabins.Find(p => p.code == "EE").minPriceDiff;
            Assert.Equal(Cabin_PE_PriceMinDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_PE_PriceMaxDiff
        [Fact]
        public async Task Cabin_PB_PriceMaxDiff()
        {
            await GetResultAsync();
            double Cabin_PE_PriceMaxDiff = 150;
            var items = result.trash.cabins.Find(p => p.code == "EE").maxPriceDiff;
            Assert.Equal(Cabin_PE_PriceMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_PE_RatingMaxDiff
        [Fact]
        public async Task Cabin_PB_RatingMaxDiff()
        {
            await GetResultAsync();
            double Cabin_PE_RatingMaxDiff = -1;
            var items = result.trash.cabins.Find(p => p.code == "EE").maxRatingDiff;
            Assert.Equal(Cabin_PE_RatingMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_PE_RatingMaxDiff
        [Fact]
        public async Task Cabin_PB_RatingMinDiff()
        {
            await GetResultAsync();
            double Cabin_PE_RatingMinDiff = -1;
            var items = result.trash.cabins.Find(p => p.code == "EE").minRatingDiff;
            Assert.Equal(Cabin_PE_RatingMinDiff.ToString(), items.ToString());
        }
        #endregion
        #region Cabin_PE_ValueMaxDiff
        [Fact]
        public async Task Cabin_PB_ValueMaxDiff()
        {
            await GetResultAsync();
            double Cabin_PE_ValueMaxDiff = -1.196172249;
            var items = result.trash.cabins.Find(p => p.code == "EE").maxValueDiff;
            Assert.Equal(Cabin_PE_ValueMaxDiff.ToString(), items.ToString());
        }
        #endregion

        #region Cabin_PE_ValueMinDiff
        [Fact]
        public async Task Cabin_PB_ValueMinDiff()
        {
            await GetResultAsync();
            double Cabin_PE_ValueMinDiff = -1.196172249;
            var items = result.trash.cabins.Find(p => p.code == "EE").minValueDiff;
            Assert.Equal(Cabin_PE_ValueMinDiff.ToString(), items.ToString());
        }
        #endregion

       
    }
}
