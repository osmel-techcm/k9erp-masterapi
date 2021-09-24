using masterCore.DTOs;
using masterCore.Entities;
using masterCore.Interfaces;
using masterInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace masterInfrastructure.Repositories
{
    class AspNetUsersGroupRepo : IAspNetUsersGroupsRepo
    {
        private readonly AppDbContext _context;

        public AspNetUsersGroupRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<responseData> GetAspNetUsersGroup(int id)
        {
            responseData responseData = new();
            try
            {
                responseData.data = await _context.AspNetUsersGroup.FindAsync(id);
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

        public async Task<responseData> GetAspNetUsersGroups()
        {
            responseData responseData = new();
            try
            {
                responseData.data = await _context.AspNetUsersGroup.ToListAsync();
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

        public async Task<responseData> PostAspNetUsersGroup(AspNetUsersGroup aspNetUsersGroup)
        {
            responseData responseData = new();
            try
            {
                _context.AspNetUsersGroup.Add(aspNetUsersGroup);
                await _context.SaveChangesAsync();
                responseData.data = aspNetUsersGroup;
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

        public async Task<responseData> PutAspNetUsersGroup(int id, AspNetUsersGroup aspNetUsersGroup)
        {
            responseData responseData = new();
            try
            {
                if (id != aspNetUsersGroup.id)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Not Found!";
                    return responseData;
                }

                _context.Entry(aspNetUsersGroup).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                responseData.data = aspNetUsersGroup;
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

        public async Task<responseData> DeleteAspNetUsersGroup(int id)
        {
            var responseData = new responseData
            {
                description = "Deleted!"
            };

            try
            {
                var aspNetUsersGroup = await _context.AspNetUsersGroup.FindAsync(id);
                if (aspNetUsersGroup == null)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Not Found!";
                    return responseData;
                }

                _context.AspNetUsersGroup.Remove(aspNetUsersGroup);

                var menuItemUserGroups = _context.MenuItemUserGroups.Where(x => x.IdUserGroup == aspNetUsersGroup.id);
                _context.MenuItemUserGroups.RemoveRange(menuItemUserGroups);

                await _context.SaveChangesAsync();
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

        public async Task<responseData> GetAspNetUsersGroupByCustomer(int idCustomer)
        {
            responseData responseData = new();
            try
            {
                responseData.data = await _context.AspNetUsersGroup.Where(x=>x.idCustomer == idCustomer).ToListAsync();
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

        public async Task<responseData> GetSiblingUserGroups(PaginatorData paginatorData, string userId)
        {
            responseData responseData = new();

            var tenantRel = await _context.UsersTenantsRelations.FirstOrDefaultAsync(x => x.UserId == userId);
            if (tenantRel != null)
            {
                var tenant = await _context.Tenants.FirstOrDefaultAsync(x=>x.Id == tenantRel.TenantId);

                var userGroups = from ug in _context.AspNetUsersGroup
                                where ug.idCustomer == tenant.Customer
                                select ug;

                if (!string.IsNullOrEmpty(paginatorData.filterDataSt))
                {
                    userGroups = insertFilters(userGroups, paginatorData.filterDataSt);
                }

                try
                {
                    if (string.IsNullOrEmpty(paginatorData.orderField))
                    {
                        paginatorData.orderField = "Id";
                    }

                    responseData.data = await userGroups
                        .OrderBy(paginatorData.orderField + (paginatorData.descending ? " desc" : ""))
                        .ToListAsync();
                }
                catch (Exception e)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = e.Message;
                    responseData.data = e;
                }
            }
            else
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "Relation not found!";
                return responseData;
            }

            return responseData;
        }

        public async Task<responseData> GetGroup(int userGroup, string userId)
        {
            responseData responseData = new();
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(x=>x.Id == userId);            
            if (user != null)
            {
                var userGroupCurrent = await _context.AspNetUsersGroup.FirstOrDefaultAsync(x=>x.id == user.IdGroup);
                var userGroups = await _context.AspNetUsersGroup.FirstOrDefaultAsync(x => x.id == userGroup);
                if (userGroups == null)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Group not found!";
                    return responseData;
                }

                if (userGroupCurrent.idCustomer == userGroups.idCustomer)
                {
                    responseData.data = userGroups;
                }
                else
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Group not found!";
                    return responseData;
                }
            }
            else
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "User not found!";
                return responseData;
            }

            return responseData;
        }        

        public async Task<responseData> PutGroup(AspNetUsersGroup userGroup, string userId)
        {
            responseData responseData = new();
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(x=>x.Id == userId); 
            if (user != null) {

                var userGroupCurrent = await _context.AspNetUsersGroup.AsNoTracking().FirstOrDefaultAsync(x=>x.id == user.IdGroup);
                if (userGroupCurrent.idCustomer == userGroup.idCustomer)
                {
                    responseData = await PutAspNetUsersGroup(userGroup.id, userGroup);
                }
                else
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Group not found!";
                    return responseData;
                }

            } else{
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "User not found!";
                return responseData;
            }

            return responseData;
        }

        public async Task<responseData> PostGroup(AspNetUsersGroup userGroup, string userId)        
        {
            responseData responseData = new();
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(x=>x.Id == userId);
            if (user != null) {
                var userGroupCurrent = await _context.AspNetUsersGroup.AsNoTracking().FirstOrDefaultAsync(x=>x.id == user.IdGroup);
                userGroup.idCustomer = userGroupCurrent.idCustomer;
                responseData = await PostAspNetUsersGroup(userGroup);
            } else{
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "User not found!";
                return responseData;
            }

            return responseData;
        }

        public async Task<responseData> DeleteGroup(AspNetUsersGroup userGroup, string userId)
        {
            responseData responseData = new();
            var user = await _context.AspNetUsers.FirstOrDefaultAsync(x=>x.Id == userId);
            if (user != null) {
                var userGroupCurrent = await _context.AspNetUsersGroup.AsNoTracking().FirstOrDefaultAsync(x=>x.id == user.IdGroup);
                if (userGroupCurrent.idCustomer == userGroup.idCustomer)
                {
                    responseData = await DeleteAspNetUsersGroup(userGroup.id);
                }
                else
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Group not found!";
                    return responseData;
                }
            } else{
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "User not found!";
                return responseData;
            }

            return responseData;
        }

        private IQueryable<AspNetUsersGroup> insertFilters(IQueryable<AspNetUsersGroup> usersGroups, string filterDataSt)
        {
            var jFilterDataSt = JObject.Parse(filterDataSt);
            foreach (var item in jFilterDataSt)
            {
                switch (item.Key.ToLower())
                {
                    case "name":
                        usersGroups = usersGroups.Where(c => c.name.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "inactive":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            usersGroups = usersGroups.Where(c => c.inactive == Convert.ToBoolean(item.Value));
                        }
                        break;
                    case "administrator":
                        if (!string.IsNullOrEmpty(item.Value.ToString()))
                        {
                            usersGroups = usersGroups.Where(c => c.administrator == Convert.ToBoolean(item.Value));
                        }
                        break;
                }
            }

            return usersGroups;
        }

        public async Task<responseData> getAdministratorGroups(int? idCustomer)
        {
            responseData responseData = new();

            idCustomer = idCustomer ?? 0;

            responseData.data = await _context.AspNetUsersGroup.AsNoTracking().CountAsync(x=>x.administrator == true && x.idCustomer == idCustomer);

            return responseData;
        }

        public async Task<responseData> inUseGroup(int id)
        {
            responseData responseData = new();            

            responseData.data = await _context.AspNetUsers.AsNoTracking().CountAsync(x=>x.IdGroup == id) > 0;

            return responseData;
        }

        public async Task<responseData> GetPermissionByGroup(int userGroup, bool all)
        {
            var responseData = new responseData();

            var menuItems = await (from mi in _context.MenuItems.Where(x => x.ParentIdMenu == null)
                                   from mig in _context.MenuItemUserGroups.Where(x => x.IdMenuItem == mi.IdMenu && x.IdUserGroup == userGroup)
                                   select new MenuItemDTO
                                   {
                                       Id = mi.Id,
                                       IdMenu = mi.IdMenu,
                                       DisplayName = mi.DisplayName,
                                       IconName = mi.IconName,
                                       Route = mi.Route,
                                       ParentIdMenu = mi.ParentIdMenu,
                                       Active = mig.Active,
                                       IdMenuItemUserGroup = mig.Id
                                   }).ToListAsync();

            if (!all)
            {
                menuItems = menuItems.Where(x=>x.Active).ToList();
            }

            foreach (var item in menuItems)
            {
                item.SubMenuItems = await GetMenuSubItems(userGroup, item, all);
            }

            responseData.data = menuItems;

            return responseData;
        }

        private async Task<List<MenuItemDTO>> GetMenuSubItems(int userGroup, MenuItemDTO item, bool all)
        {
            var menuSubItems = await (from mi in _context.MenuItems.Where(x => x.ParentIdMenu == item.IdMenu)
                                      from mig in _context.MenuItemUserGroups.Where(x => x.IdMenuItem == mi.IdMenu && x.IdUserGroup == userGroup)
                                      select new MenuItemDTO
                                      {
                                          Id = mi.Id,
                                          IdMenu = mi.IdMenu,
                                          DisplayName = mi.DisplayName,
                                          IconName = mi.IconName,
                                          Route = mi.Route,
                                          ParentIdMenu = mi.ParentIdMenu,
                                          Active = mig.Active,
                                          IdMenuItemUserGroup = mig.Id
                                      }).ToListAsync();

            if (!all)
            {
                menuSubItems = menuSubItems.Where(x => x.Active).ToList();
            }

            foreach (var subItem in menuSubItems)
            {
                subItem.SubMenuItems = await GetMenuSubItems(userGroup, subItem, all);
            }

            return menuSubItems;
        }
    }
}
