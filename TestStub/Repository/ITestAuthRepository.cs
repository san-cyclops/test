using System.Threading.Tasks;
using TestStub.Models;

namespace TestStub.Repository
{
    public interface ITestAuthRepository<T> where T : IAuthBaseEntity
    {
        public Task<AuthResponse> AuthCallerAsync(AuthRequest RequestObj);
    }
}
