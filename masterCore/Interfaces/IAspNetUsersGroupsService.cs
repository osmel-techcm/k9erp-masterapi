using masterCore.DTOs;
using masterCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface IAspNetUsersGroupsService
    {
        Task<responseData> GetAspNetUsersGroups(PaginatorData paginatorData);
        Task<responseData> GetAspNetUsersGroup(int id);
        Task<responseData> GetAspNetUsersGroupByCustomer(int idCustomer);
        Task<responseData> PostAspNetUsersGroup(AspNetUsersGroup AspNetUsersGroup);
        Task<responseData> PutAspNetUsersGroup(int id, AspNetUsersGroup AspNetUsersGroup);
        Task<responseData> DeleteAspNetUsersGroup(int id);
        Task<responseData> GetGroupsByTenant(PaginatorData paginatorData, string userId);
        Task<responseData> GetGroupByTenant(int userGroup, string userId);
        Task<responseData> PutGroupByTenant(AspNetUsersGroup userGroup, string userId);
        Task<responseData> PostGroupByTenant(AspNetUsersGroup userGroup, string userId);
        Task<responseData> DeleteGroupByTenant(int userGroup, string userId);
        Task<responseData> GetPermissionByGroup(int userGroup, bool all);
        Task<responseData> UpdatePermissionByGroup(List<MenuItemDTO> menuItems);
    }
}
