using masterCore.Entities;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface ITenantService
    {
        Task<responseData> GetTenants(PaginatorData paginatorData);
        Task<responseData> GetTenant(int id);
        Task<responseData> PostTenant(Tenant Tenant);
        Task<responseData> PutTenant(int id, Tenant Tenant);
        Task<responseData> DeleteTenant(int id);
        Task<responseData> getGenantsByCustomer(PaginatorData paginatorData, int customer);
    }
}
