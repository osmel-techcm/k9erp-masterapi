using masterCore.DTOs;
using masterCore.Entities;
using masterCore.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace masterCore.Services
{
    public class AspNetUsersGroupsService : IAspNetUsersGroupsService
    {
        private readonly IAspNetUsersGroupsRepo _aspNetUsersGroupsRepo;
        private readonly IMenuItemUserGroupService _menuItemUserGroupService;

        public AspNetUsersGroupsService(IAspNetUsersGroupsRepo aspNetUsersGroupsRepo, IMenuItemUserGroupService menuItemUserGroupService)
        {
            _aspNetUsersGroupsRepo = aspNetUsersGroupsRepo;
            _menuItemUserGroupService = menuItemUserGroupService;
        }

        public async Task<responseData> GetAspNetUsersGroup(int id)
        {
            return await _aspNetUsersGroupsRepo.GetAspNetUsersGroup(id);
        }

        public async Task<responseData> GetAspNetUsersGroups(PaginatorData paginatorData)
        {
            var aspNetUsersGroupsResponse = await _aspNetUsersGroupsRepo.GetAspNetUsersGroups();

            if (aspNetUsersGroupsResponse.error)
            {
                return aspNetUsersGroupsResponse;
            }

            var aspNetUsersGroups = (List<AspNetUsersGroup>)aspNetUsersGroupsResponse.data;

            aspNetUsersGroupsResponse.data = Paginator<AspNetUsersGroup>.Create(aspNetUsersGroups, paginatorData);

            return aspNetUsersGroupsResponse;
        }

        public async Task<responseData> PostAspNetUsersGroup(AspNetUsersGroup aspNetUsersGroup)
        {
            var postAspNetUsersGroupResponse = await _aspNetUsersGroupsRepo.PostAspNetUsersGroup(aspNetUsersGroup);
            if (postAspNetUsersGroupResponse.error)
            {
                return postAspNetUsersGroupResponse;
            }

            if (aspNetUsersGroup.idCustomer != null)
            {
                var _aspNetUsersGroup = (AspNetUsersGroup)postAspNetUsersGroupResponse.data;

                var addMenuItemUserGroupByGroupResponse = await _menuItemUserGroupService.AddMenuItemUserGroupByGroup(_aspNetUsersGroup.id);
                if (addMenuItemUserGroupByGroupResponse.error)
                {
                    return addMenuItemUserGroupByGroupResponse;
                }
            }

            return postAspNetUsersGroupResponse;
        }

        public async Task<responseData> PutAspNetUsersGroup(int id, AspNetUsersGroup aspNetUsersGroup)
        {
            return await _aspNetUsersGroupsRepo.PutAspNetUsersGroup(id, aspNetUsersGroup);
        }

        public async Task<responseData> DeleteAspNetUsersGroup(int id)
        {
            //
            return await _aspNetUsersGroupsRepo.DeleteAspNetUsersGroup(id);
        }

        public async Task<responseData> GetAspNetUsersGroupByCustomer(int idCustomer)
        {
            return await _aspNetUsersGroupsRepo.GetAspNetUsersGroupByCustomer(idCustomer);
        }

        public async Task<responseData> GetGroupsByTenant(PaginatorData paginatorData, string userId)
        {
            var userGroupResponse = await _aspNetUsersGroupsRepo.GetSiblingUserGroups(paginatorData, userId);
            if (userGroupResponse.error)
            {
                return userGroupResponse;
            }

            var aspNetUser = (List<AspNetUsersGroup>)userGroupResponse.data;
            var paginatorResult = Paginator<AspNetUsersGroup>.Create(aspNetUser, paginatorData);

            addFields(paginatorResult);

            userGroupResponse.data = paginatorResult;

            return userGroupResponse;
        }

        public async Task<responseData> GetGroupByTenant(int userGroup, string userId)
        {
            return await _aspNetUsersGroupsRepo.GetGroup(userGroup, userId);
        }

        private void addFields(Paginator<AspNetUsersGroup> paginatorResult)
        {
            var _fieldData = new fieldData
            {
                order = 1,
                name = "id",
                field = "id",
                type = "text",
                display = false,
                colSize = 1
            };

            paginatorResult.fields = new List<fieldData>
            {
                _fieldData
            };

            _fieldData = new fieldData
            {
                order = 2,
                name = "Name",
                field = "name",
                type = "text",
                display = true,
                colSize = 20,
                sort = "asc"
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 3,
                name = "Inactive",
                field = "inactive",
                type = "bool",
                display = true,
                colSize = 10
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 4,
                name = "Administrator",
                field = "administrator",
                type = "bool",
                display = true,
                colSize = 10
            };

            paginatorResult.fields.Add(_fieldData);
        }

        public async Task<responseData> PutGroupByTenant(AspNetUsersGroup userGroup, string userId)
        {
            responseData responseData = new();
            if (userGroup.inactive)
            {
                var inUseGroupResponse = await _aspNetUsersGroupsRepo.inUseGroup(userGroup.id);
                var inUseGroup = (bool)inUseGroupResponse.data;
                if (inUseGroup)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "The group is in use, can't be inactive!";
                    var userGroupData = await _aspNetUsersGroupsRepo.GetAspNetUsersGroup(userGroup.id);
                    responseData.data = userGroupData.data;
                    return responseData;
                }
            }

            return await _aspNetUsersGroupsRepo.PutGroup(userGroup, userId);
        }

        public async Task<responseData> PostGroupByTenant(AspNetUsersGroup userGroup, string userId)
        {

            var postAspNetUsersGroupResponse = await _aspNetUsersGroupsRepo.PostGroup(userGroup, userId);
            if (postAspNetUsersGroupResponse.error)
            {
                return postAspNetUsersGroupResponse;
            }

            var _aspNetUsersGroup = (AspNetUsersGroup)postAspNetUsersGroupResponse.data;

            var addMenuItemUserGroupByGroupResponse = await _menuItemUserGroupService.AddMenuItemUserGroupByGroup(_aspNetUsersGroup.id);
            if (addMenuItemUserGroupByGroupResponse.error)
            {
                return addMenuItemUserGroupByGroupResponse;
            }

            return postAspNetUsersGroupResponse;
        }

        public async Task<responseData> DeleteGroupByTenant(int idGroup, string userId)
        {
            responseData responseData = new();
            var userGroupData = await _aspNetUsersGroupsRepo.GetAspNetUsersGroup(idGroup);
            if (userGroupData.error)
            {
                return userGroupData;
            }

            var userGroup = (AspNetUsersGroup)userGroupData.data;

            if (userGroup.administrator == true)
            {
                var countGroups = await _aspNetUsersGroupsRepo.getAdministratorGroups(userGroup.idCustomer);
                if (countGroups.error)
                {
                    return countGroups;
                } 
                else 
                {
                    var admins = (int)countGroups.data;
                    if (admins <= 1)
                    {
                        responseData.error = true;
                        responseData.errorValue = 2;
                        responseData.description = "Cannot delete all the administrators!";
                        responseData.data = null;
                        return responseData;
                    }

                    var inUseGroupResponse = await _aspNetUsersGroupsRepo.inUseGroup(userGroup.id);
                    var inUseGroup = (bool)inUseGroupResponse.data;
                    if (inUseGroup)
                    {
                        responseData.error = true;
                        responseData.errorValue = 2;
                        responseData.description = "The group is in use, can't be deleted!";
                        responseData.data = null;
                        return responseData;
                    }
                }
            }

            return await _aspNetUsersGroupsRepo.DeleteGroup(userGroup, userId);
        }

        public async Task<responseData> GetPermissionByGroup(int userGroup, bool all)
        {
            return await _aspNetUsersGroupsRepo.GetPermissionByGroup(userGroup, all);
        }

        public async Task<responseData> UpdatePermissionByGroup(List<MenuItemDTO> menuItems)
        {
            var responseData = new responseData();

            foreach (var item in menuItems)
            {
                var menuItemUserGroup = await _menuItemUserGroupService.GetMenuItemUserGroup(item.IdMenuItemUserGroup);
                if (menuItemUserGroup != null)
                {
                    menuItemUserGroup.Active = item.Active;
                    responseData = await _menuItemUserGroupService.PutMenuItemUserGroup(menuItemUserGroup);
                    await UpdatePermissionByGroup(item.SubMenuItems);
                }
            }

            return responseData;
        }
    }
}
