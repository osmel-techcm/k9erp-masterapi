using masterCore.Entities;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface IAspNetUserRepo
    {
        Task<responseData> GetUsersByCustomer(int customer);
        Task<responseData> PostAspNetUserUser(AspNetUser applicationUser);
        Task<responseData> PutAspNetUserUser(AspNetUser applicationUser);
        Task<responseData> GetSiblingUsers(PaginatorData paginatorData, string userId);
        Task<responseData> GetUser(string currentUserId, string userId);
        Task<responseData> GetUser(string userId);
        Task<responseData> GetUsers(PaginatorData paginatorData);
    }
}
