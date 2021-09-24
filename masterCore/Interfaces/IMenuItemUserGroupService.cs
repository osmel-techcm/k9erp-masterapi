using masterCore.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface IMenuItemUserGroupService
    {
        Task<responseData> AddMenuItemUserGroupByGroup(int IdGroup);

        Task<responseData> PutMenuItemUserGroup(MenuItemUserGroup menuItemUserGroup);

        Task<MenuItemUserGroup> GetMenuItemUserGroup(int id);
    }
}
