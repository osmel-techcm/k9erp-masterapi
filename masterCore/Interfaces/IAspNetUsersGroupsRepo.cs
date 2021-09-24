using masterCore.DTOs;
using masterCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface IAspNetUsersGroupsRepo
    {
        Task<responseData> GetAspNetUsersGroups();
        Task<responseData> GetAspNetUsersGroup(int id);
        Task<responseData> GetAspNetUsersGroupByCustomer(int idCustomer);
        Task<responseData> PostAspNetUsersGroup(AspNetUsersGroup AspNetUsersGroup);
        Task<responseData> PutAspNetUsersGroup(int id, AspNetUsersGroup AspNetUsersGroup);
        Task<responseData> DeleteAspNetUsersGroup(int id);
        Task<responseData> GetSiblingUserGroups(PaginatorData paginatorData, string userId);
        Task<responseData> GetGroup(int userGroup, string userId);
        Task<responseData> PutGroup(AspNetUsersGroup userGroup, string userId);
        Task<responseData> PostGroup(AspNetUsersGroup userGroup, string userId);
        Task<responseData> DeleteGroup(AspNetUsersGroup userGroup, string userId);
        Task<responseData> getAdministratorGroups(int? idCustomer);
        Task<responseData> inUseGroup(int id);
        Task<responseData> GetPermissionByGroup(int userGroup, bool all);
    }
}
