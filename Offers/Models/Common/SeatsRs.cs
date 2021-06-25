using System.Collections.Generic;

namespace Offers.Models.Common
{
    #region SeatModel
    public class SeatsRs
    {
        public string id { get; set; }
        public Media media { get; set; }
        public RatingsSeat ratings { get; set; }
        public string error { get; set; }
    }
    #endregion

    #region Media
    public class Media
    {
        public string[] pictures { get; set; }
        public string? video { get; set; }
    }
    #endregion

    #region RatingsSeat
    public class RatingsSeat
    {
        public int id { get; set; }
        public double? system { get; set; }
        public PerAttribute perAttribute { get; set; }
    }
    #endregion

    #region SeatPrivacy
    public class SeatPrivacy
    {
        public string value { get; set; }
        public double? rating { get; set; }
    }
    #endregion

    #region PerAttribute
    public class PerAttribute
    {
        public SeatWidth seatWidth { get; set; }
        public SeatPitch seatPitch { get; set; }
        public SeatRecline seatRecline { get; set; }
        public SeatPrivacy seatPrivacy { get; set; }

    }
    #endregion

    #region SeatWidth
    public class SeatWidth
    {
        public double value { get; set; }
        public double? rating { get; set; }
    }
    #endregion

    #region SeatRecline
    public class SeatRecline
    {
        public string value { get; set; }
        public double? rating { get; set; }
    }
    #endregion

    #region SeatPitch
    public class SeatPitch
    {
        public double value { get; set; }
        public double? rating { get; set; }
    }
    #endregion
}
