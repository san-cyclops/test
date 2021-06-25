using Offers.Models.Auth0;
using Offers.Models.BaseEntities;
using System.Threading.Tasks;


namespace Offers.Services.Auth0
{
    public interface IAuthService<T> where T : AuthBaseEntity
    {
        public Task<AuthRs> AuthCallerAsync(AuthRq RequestObj);
    }
}
