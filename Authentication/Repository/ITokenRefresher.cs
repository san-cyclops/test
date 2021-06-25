using Authentication.Model;

namespace Authentication.Repository
{
    public interface ITokenRefresher
    {
        AuthenticationResponse Refresh(RefreshCred refreshCred);
    }
}