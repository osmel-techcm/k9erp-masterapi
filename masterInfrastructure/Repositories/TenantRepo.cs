using masterCore.Entities;
using masterCore.Interfaces;
using masterInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace masterInfrastructure.Repositories
{
    public class TenantRepo : ITenantRepo
    {
        private readonly AppDbContext _context;

        public TenantRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<responseData> GetTenant(int id)
        {
            var responseData = new responseData();
            try
            {
                responseData.data = await _context.Tenants.FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<responseData> GetTenants()
        {
            var responseData = new responseData();
            try
            {
                responseData.data = await _context.Tenants.ToListAsync();
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

        public async Task<responseData> PostTenant(Tenant tenant)
        {
            var responseData = new responseData();
            try
            {                
                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync();
                responseData.data = tenant;
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

        public async Task<responseData> PutTenant(int id, Tenant tenant)
        {
            var responseData = new responseData();            

            try
            {
                if (id != tenant.Id)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Not Found!";
                    return responseData;
                }

                var dbTenant = await _context.Tenants.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                if (dbTenant == null)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Tenant not found!";
                    return responseData;
                }

                tenant.ConnectionString = dbTenant.ConnectionString;
                _context.Entry(tenant).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                responseData.data = tenant;
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

        public async Task<responseData> DeleteTenant(int id)
        {
            var responseData = new responseData
            {
                description = "Deleted!"
            };

            try
            {
                var tenants = await _context.Tenants.FindAsync(id);
                if (tenants == null)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Not Found!";
                    return responseData;
                }

                _context.Tenants.Remove(tenants);
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

        public async Task<responseData> GetTenantsByCustomer(int customer)
        {
            var responseData = new responseData();
            try
            {
                responseData.data = await _context.Tenants.Where(x => x.Customer == customer).ToListAsync();
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
