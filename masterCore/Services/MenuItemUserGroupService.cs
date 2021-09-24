using masterCore.Entities;
using masterCore.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace masterCore.Services
{
    public class MenuItemUserGroupService : IMenuItemUserGroupService
    {
        private readonly IMenuItemUserGroupRepo _menuItemUserGroupRepo;
        private readonly IMenuItemService _menuItemService;

        public MenuItemUserGroupService(IMenuItemUserGroupRepo menuItemUserGroupRepo, IMenuItemService menuItemService)
        {
            _menuItemUserGroupRepo = menuItemUserGroupRepo;
            _menuItemService = menuItemService;
        }

        public async Task<responseData> AddMenuItemUserGroupByGroup(int IdGroup)
        {
            var responseData = new responseData();

            var menuItemsResponse = await _menuItemService.GetMenuRawItems();
            if (menuItemsResponse.error)
            {
                return menuItemsResponse;
            }

            var menuItems = (List<MenuItem>)menuItemsResponse.data;
            foreach (var item in menuItems)
            {
                var menuItemUserGroup = new MenuItemUserGroup
                {
                    IdUserGroup = IdGroup,
                    IdMenuItem = item.IdMenu,
                    Active = true
                };

                responseData = await _menuItemUserGroupRepo.PostMenuItemUserGroupRepo(menuItemUserGroup);
                if (responseData.error)
                {
                    return responseData;
                }
            }

            return responseData;
        }

        public async Task<MenuItemUserGroup> GetMenuItemUserGroup(int id)
        {
            return await _menuItemUserGroupRepo.GetMenuItemUserGroup(id);
        }

        public async Task<responseData> PutMenuItemUserGroup(MenuItemUserGroup menuItemUserGroup)
        {
            return await _menuItemUserGroupRepo.PutMenuItemUserGroup(menuItemUserGroup);
        }
    }
}
