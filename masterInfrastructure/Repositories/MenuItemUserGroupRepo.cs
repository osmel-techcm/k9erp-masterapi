using masterCore.Entities;
using masterCore.Interfaces;
using masterInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace masterInfrastructure.Repositories
{
    public class MenuItemUserGroupRepo : IMenuItemUserGroupRepo
    {
        private readonly AppDbContext _context;

        public MenuItemUserGroupRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MenuItemUserGroup> GetMenuItemUserGroup(int id)
        {
            return await _context.MenuItemUserGroups.FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<responseData> PostMenuItemUserGroupRepo(MenuItemUserGroup menuItemUserGroup)
        {
            var responseData = new responseData();

            try
            {
                _context.MenuItemUserGroups.Add(menuItemUserGroup);
                await _context.SaveChangesAsync();
                responseData.data = menuItemUserGroup;
            }
            catch (Exception e)
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = e.Message;
                responseData.data = e;
            }

            return responseData;
        }

        public async Task<responseData> PutMenuItemUserGroup(MenuItemUserGroup menuItemUserGroup)
        {
            var responseData = new responseData();

            try
            {
                _context.Entry(menuItemUserGroup).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                responseData.data = menuItemUserGroup;
            }
            catch (Exception e)
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = e.Message;
                responseData.data = e;
            }

            return responseData;
        }
    }
}
