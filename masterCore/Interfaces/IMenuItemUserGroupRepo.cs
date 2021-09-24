using masterCore.Entities;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface IMenuItemUserGroupRepo
    {
        Task<responseData> PostMenuItemUserGroupRepo(MenuItemUserGroup menuItemUserGroup);

        Task<responseData> PutMenuItemUserGroup(MenuItemUserGroup menuItemUserGroup);

        Task<MenuItemUserGroup> GetMenuItemUserGroup(int id);
    }
}
