using masterCore.Entities;
using masterCore.Interfaces;
using masterInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace masterInfrastructure.Repositories
{
    public class UsersTenantsRelationRepo : IUsersTenantsRelationRepo
    {
        private readonly AppDbContext _context;

        public UsersTenantsRelationRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<responseData> GetUsersTenantsRelation(int id)
        {
            var responseData = new responseData();

            try
            {
                responseData.data = await _context.UsersTenantsRelations.FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<responseData> GetUsersTenantsRelations()
        {
            var responseData = new responseData();

            try
            {
                responseData.data = await _context.UsersTenantsRelations.ToListAsync();
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

        public async Task<responseData> PostUsersTenantsRelation(UsersTenantsRelation usersTenantsRelation)
        {
            var responseData = new responseData();

            try
            {
                _context.UsersTenantsRelations.Add(usersTenantsRelation);
                await _context.SaveChangesAsync();
                responseData.data = usersTenantsRelation;
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

        public async Task<responseData> PutUsersTenantsRelation(int id, UsersTenantsRelation usersTenantsRelation)
        {
            var responseData = new responseData();

            try
            {
                if (id != usersTenantsRelation.Id)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Not Found!";
                    return responseData;
                }

                _context.Entry(usersTenantsRelation).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                responseData.data = usersTenantsRelation;
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

        public async Task<responseData> DeleteUsersTenantsRelation(int id)
        {
            var responseData = new responseData
            {
                description = "Deleted!"
            };

            try
            {
                var usersTenantsRelation = await _context.UsersTenantsRelations.FindAsync(id);
                if (usersTenantsRelation == null)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Not Found!";
                    return responseData;
                }

                _context.UsersTenantsRelations.Remove(usersTenantsRelation);
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

        public async Task<responseData> UsersTenantsRelationExist(UsersTenantsRelation usersTenantsRelation)
        {
            var responseData = new responseData();

            try
            {
                responseData.data = await _context.UsersTenantsRelations.AnyAsync(x => x.UserId == usersTenantsRelation.UserId && x.TenantId == usersTenantsRelation.TenantId);
            }
            catch (Exception e)
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = e.Message;
                responseData.data = e;
                return responseData;
            }

            return responseData;
        }

        public async Task<responseData> SyncUsersTenantsRelation(int customerId)
        {
            var responseData = new responseData();

            try
            {
                var userTenantSync = await (from u in _context.AspNetUsers
                                     join g in _context.AspNetUsersGroup on u.IdGroup equals g.id
                                     from t in _context.Tenants
                                     where g.idCustomer == customerId && t.Customer == customerId
                                     && !_context.UsersTenantsRelations.Any(rel => rel.UserId == u.Id && rel.TenantId == t.Id)
                                     select new UsersTenantsRelation { 
                                        UserId = u.Id,
                                        TenantId = t.Id
                                     }).ToListAsync();

                foreach (var item in userTenantSync)
                {
                    responseData = await PostUsersTenantsRelation(item);
                    if (responseData.error)
                    {
                        return responseData;
                    }
                }
            }
            catch (Exception e)
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = e.Message;
                responseData.data = e;
                return responseData;
            }

            return responseData;
        }
    }
}
