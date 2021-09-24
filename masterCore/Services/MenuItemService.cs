using masterCore.Entities;
using masterCore.Interfaces;
using System.Threading.Tasks;

namespace masterCore.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepo _menuItemRepo;

        public MenuItemService(IMenuItemRepo menuItemRepo)
        {
            _menuItemRepo = menuItemRepo;
        }

        public async Task<responseData> GetMenuItems()
        {
            return await _menuItemRepo.GetMenuItems();
        }

        public async Task<responseData> GetMenuRawItems()
        {
            return await _menuItemRepo.GetMenuRawItems();
        }
    }
}
