namespace Authentication.Model
{
    public class AuthenticationResponse
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
