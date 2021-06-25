using Microsoft.Extensions.Logging;
using Moq;
using Offers.Models.BaseEntities;
using Offers.Services.UserRating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Offers.Models.Common;
using Offers.Models.Common.Weightnings;
using Xunit;

namespace TestProject1
{
    public class UserWeightingsTest
    {
        #region GetResponse
        public static async Task<double> GetResultAsync(double valSW, double valSP, double valBL, double valSPR, double valSR, double valBSW, double valBSP, double valBSPR,double valBSR)
        {
            var mock = new Mock<ILogger<UserRatingLogicService>>();
            ILogger<UserRatingLogicService> MockLogger = mock.Object;


            var controller = new UserRatingLogicService(MockLogger);

            Ratings Ratings = new Ratings();
            Ratings.seatWidth = valSW;
            Ratings.seatPitch = valSP;
            Ratings.bedLenth = valBL;
            Ratings.seatPrivacy = valSPR;
            Ratings.seatRecline = valSR;

            Base64Weightings Base64Weightings = new Base64Weightings();
            Weightings Weightings = new Weightings();
            Outbound_UserRating Outbound_UserRating = new Outbound_UserRating();

            Outbound_UserRating.seatWidth = valBSW;
            Outbound_UserRating.seatPitch = valBSP;
            Outbound_UserRating.seatPrivacy = valBSPR;
            Outbound_UserRating.seatRecline = valBSR;

            Weightings.outbound = Outbound_UserRating;
            Base64Weightings.weightings = Weightings;

            return await controller.UserRatingCalculation(Ratings, Base64Weightings);
        }
        #endregion

        #region All_On_Width
        [Fact]
        public async void All_On_Width()
        {
            double AllOnWidth = await GetResultAsync(1,2,0,3,4,10,0,0,0);
            Assert.Equal("1",AllOnWidth.ToString());
        }
        #endregion

        #region All_On_Pitch
        [Fact]
        public async void All_On_Pitch()
        {
            double All_On_Pitch = await GetResultAsync(1, 2, 0, 3, 4, 0, 10, 0, 0);
            Assert.Equal("2", All_On_Pitch.ToString());
        }
        #endregion

        #region All_On_Privacy
        [Fact]
        public async void All_On_Privacy()
        {
            double All_On_Privacy = await GetResultAsync(1, 2, 0, 3, 4, 0, 0, 10,0);
            Assert.Equal("3", All_On_Privacy.ToString());
        }
        #endregion

        #region All_on_Recline
        [Fact]
        public async void All_on_Recline()
        {
            double All_on_Recline = await GetResultAsync(1, 2, 0, 3, 4, 0, 0, 0, 10);
            Assert.Equal("4", All_on_Recline.ToString());
        }
        #endregion

        #region Random
        [Fact]
        public async void Random()
        {
            double Random = await GetResultAsync(1, 2, 0, 3, 4, 1, 2, 3, 4);
            Assert.Equal("3", Random.ToString());
        }
        #endregion

        #region First_two
        [Fact]
        public async void First_two()
        {
            double First_two = await GetResultAsync(1, 2, 0, 3, 4, 5, 5, 0, 0);
            Assert.Equal("1.5", First_two.ToString());
        }
        #endregion

        #region First_two
        [Fact]
        public async void Last_two()
        {
            double First_two = await GetResultAsync(1, 2, 0, 3, 4, 0, 0, 10, 10);
            Assert.Equal("3.5", First_two.ToString());
        }
        #endregion

        #region Middle_two
        [Fact]
        public async void Middle_two()
        {
            double Middle_two = await GetResultAsync(1, 2, 0, 3, 4, 0, 3,3, 0);
            Assert.Equal("2.5", Middle_two.ToString());
        }
        #endregion

        #region Outer_two
        [Fact]
        public async void Outer_two()
        {
            double Outer_two = await GetResultAsync(1, 2, 0, 3, 4, 0, 3, 3, 0);
            Assert.Equal("2.5", Outer_two.ToString());
        }
        #endregion

        #region Minimalist
        [Fact]
        public async void Minimalist()
        {
            double Minimalist = await GetResultAsync(1, 2, 0, 3, 4, 1, 1, 1, 1);
            Assert.Equal("2.5", Minimalist.ToString());
        }
        #endregion

        #region Maximalist
        [Fact]
        public async void Maximalist()
        {
            double Maximalist = await GetResultAsync(1, 2, 0, 3, 4, 10, 10, 10, 10);
            Assert.Equal("2.5", Maximalist.ToString());
        }
        #endregion

        #region Maximalist
        [Fact]
        public async void Not_picky()
        {
            double Not_picky = await GetResultAsync(1, 2, 0, 3, 4, 2, 2, 2, 2);
            Assert.Equal("2.5", Not_picky.ToString());
        }
        #endregion

        #region Centrist
        [Fact]
        public async void Centrist()
        {
            double Centrist = await GetResultAsync(1, 2, 0, 3, 4, 5, 5, 5, 5);
            Assert.Equal("2.5", Centrist.ToString());
        }
        #endregion

        #region All_on_Pitch
        [Fact]
        public async void All_on_Pitch()
        {
            double All_on_Pitch = await GetResultAsync(1, 2, 0, 3, 4, 0, 10, 0, 0);
            Assert.Equal("2", All_on_Pitch.ToString());
        }
        #endregion

        #region First_Two
        [Fact]
        public async void First_Two()
        {
            double First_Two = await GetResultAsync(1, 0, 2, 3, 4, 5, 5, 0, 0);
            Assert.Equal("1.5", First_Two.ToString());
        }
        #endregion

        #region First_Two
        [Fact]
        public async void Middle_Two()
        {
            double Middle_Two = await GetResultAsync(1, 0, 2, 3, 4, 0, 3, 3, 0);
            Assert.Equal("2.5", Middle_Two.ToString());
        }
        #endregion

        #region Centerist
        [Fact]
        public async void Centerist()
        {
            double Centerist = await GetResultAsync(1, 0, 2, 3, 4, 5, 5, 5, 5);
            Assert.Equal("2.5", Centerist.ToString());
        }
        #endregion

        #region All_On_Pitch_2
        [Fact]
        public async void All_On_Pitch_2()
        {
            double All_On_Pitch_2 = await GetResultAsync(1, 2, 2, 3, 4, 0, 10, 0, 0);
            Assert.Equal("2", All_On_Pitch_2.ToString());
        }
        #endregion

        #region First_Two_2
        [Fact]
        public async void First_Two_2()
        {
            double All_On_Pitch_2 = await GetResultAsync(1, 2, 2, 3, 4, 5, 5, 0, 0);
            Assert.Equal("1.5", All_On_Pitch_2.ToString());
        }
        #endregion

        #region Middle_Two_2
        [Fact]
        public async void Middle_Two_2()
        {
            double Middle_Two_2 = await GetResultAsync(1, 2, 2, 3, 4, 0, 3, 3, 0);
            Assert.Equal("2.5", Middle_Two_2.ToString());
        }
        #endregion

        #region Centrist_2
        [Fact]
        public async void Centrist_2()
        {
            double Centrist_2 = await GetResultAsync(1, 2, 2, 3, 4, 5, 5, 5, 5);
            Assert.Equal("2.5", Centrist_2.ToString());
        }
        #endregion
    }
}
