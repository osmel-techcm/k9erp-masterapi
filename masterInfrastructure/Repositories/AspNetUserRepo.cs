using masterCore.Entities;
using masterCore.Interfaces;
using masterInfrastructure.Data;
using masterInfrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace masterInfrastructure.Repositories
{
    class AspNetUserRepo : IAspNetUserRepo
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AspNetUserRepo(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<responseData> GetUsersByCustomer(int customer)
        {
            var responseData = new responseData();

            try
            {
                var usersData = await (from u in _context.AspNetUsers
                                       from ug in _context.AspNetUsersGroup.Where(x=> x.id == u.IdGroup && x.idCustomer == customer)
                                       select new AspNetUser {
                                          Id = u.Id,
                                          AccessFailedCount = u.AccessFailedCount,
                                          ConcurrencyStamp = u.ConcurrencyStamp,
                                          AspNetUserClaims = u.AspNetUserClaims,
                                          AspNetUserLogins = u.AspNetUserLogins,
                                          AspNetUserRoles = u.AspNetUserRoles,
                                          AspNetUserTokens = u.AspNetUserTokens,
                                          CustomerId = u.CustomerId,
                                          Email = u.Email,
                                          EmailConfirmed = u.EmailConfirmed,
                                          IdGroup = u.IdGroup,
                                          IdGroupName = ug.name,
                                          inactive = u.inactive,
                                          LastName = u.LastName,
                                          LockoutEnabled = u.LockoutEnabled,
                                          LockoutEnd = u.LockoutEnd,
                                          masterDealer = u.masterDealer,
                                          Name = u.Name,
                                          NormalizedEmail = u.NormalizedEmail,
                                          NormalizedUserName = u.NormalizedUserName,
                                          Password = u.Password,
                                          PasswordHash = u.PasswordHash,
                                          PhoneNumber = u.PhoneNumber,
                                          PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                                          SecurityStamp = u.SecurityStamp,
                                          TwoFactorEnabled = u.TwoFactorEnabled,
                                          UserName = u.UserName
                                       }
                                       ).ToListAsync();

                var usersDataRes = usersData.Distinct();
                foreach (var item in usersDataRes)
                {
                    item.CustomerId = customer;
                }

                responseData.data = usersDataRes;
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

        public async Task<responseData> PostAspNetUserUser(AspNetUser applicationUser)
        {
            var responseData = new responseData();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = applicationUser.Email,
                    Email = applicationUser.Email,
                    Name = applicationUser.Name,
                    LastName = applicationUser.LastName,
                    masterDealer = false,
                    IdGroup = applicationUser.IdGroup ?? 0
                };

                var result = await _userManager.CreateAsync(user, applicationUser.Password);
                if (result.Succeeded)
                {
                    var aspNetUser = new AspNetUser
                    {
                        Id = user.Id,
                        Name = user.Name,
                        LastName = user.LastName
                    };

                    responseData.data = aspNetUser;
                    return responseData;
                }
                else
                {
                    responseData.error = true;
                    responseData.description = "Bad Request";
                    responseData.othersValidations = result;
                    return responseData;
                }
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

        public async Task<responseData> PutAspNetUserUser(AspNetUser applicationUser)
        {
            var responseData = new responseData();

            try
            {
                var dbUser = await _context.AspNetUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == applicationUser.Id);
                if (dbUser == null)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "User not found!";
                    return responseData;
                }

                if (dbUser.PasswordHash != applicationUser.Password)
                {
                    var passwordHasher = new PasswordHasher<ApplicationUser>();
                    var user = new ApplicationUser
                    {
                        UserName = applicationUser.Email,
                        Email = applicationUser.Email,
                        Name = applicationUser.Name,
                        LastName = applicationUser.LastName,
                        masterDealer = false,
                        IdGroup = applicationUser.IdGroup ?? 0
                    };

                    applicationUser.PasswordHash = passwordHasher.HashPassword(user, applicationUser.Password);
                }

                _context.Entry(applicationUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = e.Message;
                responseData.data = e;
                return responseData;
            }  

            responseData.data = applicationUser;

            return responseData;
        }

        public async Task<responseData> GetSiblingUsers(PaginatorData paginatorData, string userId)
        {
            var responseData = new responseData();

            var tenant = await _context.UsersTenantsRelations.FirstOrDefaultAsync(x=>x.UserId == userId);
            if (tenant != null)
            {
                var users = from u in _context.AspNetUsers
                            join utr in _context.UsersTenantsRelations on u.Id equals utr.UserId
                            where utr.TenantId == tenant.TenantId
                            select u;

                if (!string.IsNullOrEmpty(paginatorData.filterDataSt))
                {
                    users = insertFilters(users, paginatorData.filterDataSt);
                }

                try
                {
                    if (string.IsNullOrEmpty(paginatorData.orderField))
                    {
                        paginatorData.orderField = "Id";
                    }

                    responseData.data = await users
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

        public async Task<responseData> GetUser(string currentUserId, string userId)
        {
            var responseData = new responseData();
            var tenant = await _context.UsersTenantsRelations.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == currentUserId);
            if (tenant != null)
            {
                responseData.data = await (from u in _context.AspNetUsers
                           join utr in _context.UsersTenantsRelations on u.Id equals utr.UserId
                           where utr.TenantId == tenant.TenantId && u.Id == userId
                           select u).AsNoTracking().FirstOrDefaultAsync();
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

        public async Task<responseData> GetUsers(PaginatorData paginatorData)
        {
            var responseData = new responseData();

            var users = _context.AspNetUsers.Where(x => x.masterDealer == true);
            if (users != null)
            {
                if (!string.IsNullOrEmpty(paginatorData.filterDataSt))
                {
                    users = insertFilters(users, paginatorData.filterDataSt);
                }

                try
                {
                    if (string.IsNullOrEmpty(paginatorData.orderField))
                    {
                        paginatorData.orderField = "Id";
                    }

                    responseData.data = await users
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

        private IQueryable<AspNetUser> insertFilters(IQueryable<AspNetUser> users, string filterDataSt)
        {
            var jFilterDataSt = JObject.Parse(filterDataSt);
            foreach (var item in jFilterDataSt)
            {
                switch (item.Key.ToLower())
                {
                    case "username":
                        users = users.Where(c => c.UserName.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "email":
                        users = users.Where(c => c.Email.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "phonenumber":
                        users = users.Where(c => c.PhoneNumber.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "name":
                        users = users.Where(c => c.Name.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "lastname":
                        users = users.Where(c => c.LastName.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                }
            }

            return users;
        }

        public async Task<responseData> GetUser(string userId)
        {
            var responseData = new responseData();

            try
            {
                var user = await _context.AspNetUsers.FirstOrDefaultAsync(x => x.Id == userId);
                user.Password = user.PasswordHash;
                responseData.data = user;
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
