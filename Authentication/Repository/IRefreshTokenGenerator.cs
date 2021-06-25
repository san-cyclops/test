namespace Authentication.Repository
{
    public interface IRefreshTokenGenerator
    {
        string GenerateToken();
    }
}
