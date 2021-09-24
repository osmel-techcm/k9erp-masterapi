using masterCore.Entities;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface IUsersTenantsRelationService
    {
        Task<responseData> GetUsersTenantsRelations(PaginatorData paginatorData);
        Task<responseData> GetUsersTenantsRelation(int id);
        Task<responseData> PostUsersTenantsRelation(UsersTenantsRelation usersTenantsRelation);
        Task<responseData> PutUsersTenantsRelation(int id, UsersTenantsRelation usersTenantsRelation);
        Task<responseData> DeleteUsersTenantsRelation(int id);
        Task<responseData> UsersTenantsRelationExist(UsersTenantsRelation usersTenantsRelation);
        Task<responseData> SyncUsersTenantsRelation(int customerId);
    }
}
