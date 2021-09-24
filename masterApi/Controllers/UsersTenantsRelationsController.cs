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
    public class UsersTenantsRelationsController : ControllerBase
    {
        private readonly IUsersTenantsRelationService _usersTenantsRelationService;

        public UsersTenantsRelationsController(IUsersTenantsRelationService usersTenantsRelationService)
        {
            _usersTenantsRelationService = usersTenantsRelationService;
        }

        [HttpGet]
        public async Task<responseData> GetUsersTenantsRelations([FromHeader] PaginatorData paginatorData)
        {
            return await _usersTenantsRelationService.GetUsersTenantsRelations(paginatorData);
        }

        [HttpGet("{id}")]
        public async Task<responseData> GetUsersTenantsRelation(int id)
        {
            return await _usersTenantsRelationService.GetUsersTenantsRelation(id);
        }

        [HttpPut("{id}")]
        public async Task<responseData> PutUsersTenantsRelation(int id, UsersTenantsRelation usersTenantsRelation)
        {
            return await _usersTenantsRelationService.PutUsersTenantsRelation(id, usersTenantsRelation);
        }

        [HttpPost]
        public async Task<responseData> PostUsersTenantsRelation(UsersTenantsRelation usersTenantsRelation)
        {
            return await _usersTenantsRelationService.PostUsersTenantsRelation(usersTenantsRelation);
        }

        [HttpDelete("{id}")]
        public async Task<responseData> DeleteUsersTenantsRelation(int id)
        {
            return await _usersTenantsRelationService.DeleteUsersTenantsRelation(id);
        }
        
    }
}
