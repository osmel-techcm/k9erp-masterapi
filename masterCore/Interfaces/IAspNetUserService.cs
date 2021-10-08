using masterCore.Entities;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface IAspNetUserService
    {
        Task<responseData> GetUsersByCustomer(int customer);
        Task<responseData> PostAspNetUserUser(AspNetUser applicationUser);
        Task<responseData> PutAspNetUserUser(AspNetUser applicationUser);
        Task<responseData> GetUsersByTenant(PaginatorData paginatorData, string userId);
        Task<responseData> GetUserByTenant(string currentUserId, string userId);
        Task<responseData> PutUserByTenant(string currentUserId, AspNetUser user);
        Task<responseData> PostUserByTenant(string currentUserId, AspNetUser user);
        Task<responseData> GetUsers(PaginatorData paginatorData);
        Task<responseData> GetUser(string id);
        Task<responseData> GenerateSetupCode(string issuer, string email, string key);
        Task<responseData> EnableTwoFactorAuth(string issuer, string code, string key);
    }
}
