using masterCore.Entities;
using masterCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace masterApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;

        public TenantsController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        // GET: api/Tenants
        [HttpGet]
        public async Task<responseData> GetTenants([FromHeader] PaginatorData paginatorData)
        {
            return await _tenantService.GetTenants(paginatorData);
        }

        // GET: api/Tenants/5
        [HttpGet("{id}")]
        public async Task<responseData> GetTenant(int id)
        {
            return await _tenantService.GetTenant(id);
        }

        // PUT: api/Tenants/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<responseData> PutTenant(int id, Tenant tenant)
        {
            return await _tenantService.PutTenant(id, tenant);
        }

        // POST: api/Tenants
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<responseData> PostTenant(Tenant tenant)
        {
            return await _tenantService.PostTenant(tenant);
        }

        // DELETE: api/Tenants/5
        [HttpDelete("{id}")]
        public async Task<responseData> DeleteTenant(int id)
        {
            return await _tenantService.DeleteTenant(id);
        }

        [HttpGet]
        [Route("getGenantsByCustomer")]
        public async Task<responseData> getGenantsByCustomer([FromHeader] PaginatorData paginatorData, int customer) 
        {
            return await _tenantService.getGenantsByCustomer(paginatorData, customer);
        }
    }
}
