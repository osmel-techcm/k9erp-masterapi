using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using masterCore.Entities;
using masterCore.Interfaces;
using masterInfrastructure.Data;

namespace masterInfrastructure.Repositories
{
    public class MenuItemRepo : IMenuItemRepo
    {
        private readonly AppDbContext _context;

        public MenuItemRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<responseData> GetMenuItems()
        {
            var responseData = new responseData();

            var menuItems = await _context.MenuItems.Where(x => x.ParentIdMenu == null).OrderBy(x => x.IdMenu).ToListAsync();
            foreach (var menuItem in menuItems)
            {
                if (string.IsNullOrEmpty(menuItem.Route))
                {
                    menuItem.SubMenuItems = await GetMenuSubItems(menuItem);
                }                
            }

            responseData.data = menuItems;

            return responseData;
        }

        private async Task<List<MenuItem>> GetMenuSubItems(MenuItem menuItem)
        {
            var menuSubItems = await _context.MenuItems.Where(x => x.ParentIdMenu == menuItem.IdMenu).OrderBy(x => x.IdMenu).ToListAsync();
            foreach (var menuSubItem in menuSubItems)
            {
                if (string.IsNullOrEmpty(menuSubItem.Route))
                {
                    menuSubItem.SubMenuItems = await GetMenuSubItems(menuSubItem);
                }
            }

            return menuSubItems;
        }

        public async Task<responseData> GetMenuRawItems()
        {
            var responseData = new responseData
            {
                data = await _context.MenuItems.OrderBy(x => x.IdMenu).ToListAsync()
            };

            return responseData;
        }
    }
}
