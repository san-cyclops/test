using System.Collections.Generic;


namespace Offers.Models.Common
{
    public class ErrorRs
    {
        public string title { get; set; }
        public string detail { get; set; }
    }

    public class Errors
    {
        public List<ErrorRs> errors { get; set; }
    }
}
