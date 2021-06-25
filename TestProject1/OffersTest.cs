using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class OffersTest
    {
         

        #region Response_Not_Null
        [Fact]
        public void Outbound()
        {
            Assert.NotNull("");

            //outbound
            Assert.Equal("class", "");
            Assert.Equal("flight", "");

            //departure
            Assert.Equal("code", "");
            Assert.Equal("name", "");
            Assert.Equal("at", "");

            //arrival
            Assert.Equal("code", "");
            Assert.Equal("name", "");
            Assert.Equal("at", "");

            //carrier
            Assert.Equal("code", "");
            Assert.Equal("name", "");

            //operating
            Assert.Equal("code", "");
            Assert.Equal("name", "");

            //aircraft
            Assert.Equal("code", "");
            Assert.Equal("name", "");

            //ratings
            Assert.Equal("random", "");
            Assert.Equal("seatWidth", "");
            Assert.Equal("seatRecline", "");
            Assert.Equal("seatPitch", "");
            Assert.Equal("videoType", "");
            Assert.Equal("laptopPower", "");
            Assert.Equal("wiFi", "");
            Assert.Equal("seatType", "");
            Assert.Equal("seatPrivacy", "");
            Assert.Equal("bedLenth", "");

        }
        #endregion


        #region Response_Not_Null
        [Fact]
        public void Inbound()
        {
            Assert.NotNull("");

            //outbound
            Assert.Equal("class", "");
            Assert.Equal("flight", "");

            //departure
            Assert.Equal("code", "");
            Assert.Equal("name", "");
            Assert.Equal("at", "");

            //arrival
            Assert.Equal("code", "");
            Assert.Equal("name", "");
            Assert.Equal("at", "");

            //carrier
            Assert.Equal("code", "");
            Assert.Equal("name", "");

            //operating
            Assert.Equal("code", "");
            Assert.Equal("name", "");

            //aircraft
            Assert.Equal("code", "");
            Assert.Equal("name", "");

            //ratings
            Assert.Equal("random", "");
            Assert.Equal("seatWidth", "");
            Assert.Equal("seatRecline", "");
            Assert.Equal("seatPitch", "");
            Assert.Equal("videoType", "");
            Assert.Equal("laptopPower", "");
            Assert.Equal("wiFi", "");
            Assert.Equal("seatType", "");
            Assert.Equal("seatPrivacy", "");
            Assert.Equal("bedLenth", "");

        }
        #endregion

        #region Cost
        [Fact]
        public void Cost()
        {
            //ratings
            Assert.Equal("random", "");
            Assert.Equal("seatWidth", "");

        }
        #endregion


    }
}
