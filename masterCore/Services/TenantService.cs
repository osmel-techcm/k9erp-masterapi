using masterCore.Entities;
using masterCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace masterCore.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepo _tenantRepo;
        private readonly IUsersTenantsRelationService _usersTenantsRelationService;

        public TenantService(ITenantRepo tenantRepo, IUsersTenantsRelationService usersTenantsRelationService)
        {
            _tenantRepo = tenantRepo;
            _usersTenantsRelationService = usersTenantsRelationService;
        }

        public async Task<responseData> GetTenant(int id)
        {
            return await _tenantRepo.GetTenant(id);
        }

        public async Task<responseData> GetTenants(PaginatorData paginatorData)
        {
            var tenantsResponse = await _tenantRepo.GetTenants();

            if (tenantsResponse.error)
            {
                return tenantsResponse;
            }

            var tenants = (List<Tenant>)tenantsResponse.data;

            tenantsResponse.data = Paginator<Tenant>.Create(tenants, paginatorData);

            return tenantsResponse;
        }

        public async Task<responseData> PostTenant(Tenant tenant)
        {
            var responseData = new responseData();

            if (tenant.Customer == 0)
            {
                responseData.error = true;
                responseData.errorValue = 2;
                responseData.description = "Missing Customer!";
                return responseData;
            }

            tenant.ConnectionString = Guid.NewGuid().ToString();

            responseData = await _tenantRepo.PostTenant(tenant);
            if (responseData.error)
            {
                return responseData;
            }

            responseData = await _usersTenantsRelationService.SyncUsersTenantsRelation(tenant.Customer);
            if (responseData.error)
            {
                return responseData;
            }

            responseData.data = tenant;

            return responseData;
        }

        public async Task<responseData> PutTenant(int id, Tenant tenant)
        {
            return await _tenantRepo.PutTenant(id, tenant);
        }

        public async Task<responseData> DeleteTenant(int id)
        {
            return await _tenantRepo.DeleteTenant(id);
        }

        public async Task<responseData> getGenantsByCustomer(PaginatorData paginatorData, int customer)
        {
            var tenantsResponse = await _tenantRepo.GetTenantsByCustomer(customer);

            if (tenantsResponse.error)
            {
                return tenantsResponse;
            }

            var tenants = (List<Tenant>)tenantsResponse.data;

            tenantsResponse.data = Paginator<Tenant>.Create(tenants, paginatorData);

            return tenantsResponse;
        }
    }
}
