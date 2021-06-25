using System.Collections.Generic;

namespace TestStub.Models
{
    public class OfferCompareResponse
    {
        public int Id { get; set; }

        public List<OfferCompare> OfferCompareList { get; set; }
    }

    public class OfferCompare
    {
        public string FieldName { get; set; }

        public string ActualValue { get; set; }

        public string ExpectedValue { get; set; }

        public bool Result { get; set; }
    }

}
