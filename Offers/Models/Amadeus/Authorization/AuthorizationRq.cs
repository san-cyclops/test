namespace Offers.Models.Amadeus.Authorization
{
    public class AuthorizationRq
    {
        public string Grant_Type { get; set; }
        public string Client_Id { get; set; }
        public string Client_Secret { get; set; }
    }
}
