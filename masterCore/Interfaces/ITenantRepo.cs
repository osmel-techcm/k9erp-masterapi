using masterCore.Entities;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface ITenantRepo
    {
        Task<responseData> GetTenants();
        Task<responseData> GetTenant(int id);
        Task<responseData> PostTenant(Tenant Tenant);
        Task<responseData> PutTenant(int id, Tenant Tenant);
        Task<responseData> DeleteTenant(int id);
        Task<responseData> GetTenantsByCustomer(int customer);
    }
}
