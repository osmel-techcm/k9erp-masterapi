using masterCore.DTOs;
using masterCore.Entities;
using masterCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace masterApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AspNetUsersGroupsController : ControllerBase
    {
        private readonly IAspNetUsersGroupsService _aspNetUsersGroupsService;

        public AspNetUsersGroupsController(IAspNetUsersGroupsService aspNetUsersGroupsService)
        {
            _aspNetUsersGroupsService = aspNetUsersGroupsService;
        }

        // GET: api/AspNetUsersGroups
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<responseData> GetAspNetUsersGroup([FromHeader] PaginatorData paginatorData)
        {
            return await _aspNetUsersGroupsService.GetAspNetUsersGroups(paginatorData);
        }

        // GET: api/AspNetUsersGroups/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<responseData> GetAspNetUsersGroup(int id)
        {
            return await _aspNetUsersGroupsService.GetAspNetUsersGroup(id);
        }

        // PUT: api/AspNetUsersGroups/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<responseData> PutAspNetUsersGroup(int id, AspNetUsersGroup aspNetUsersGroup)
        {
            return await _aspNetUsersGroupsService.PutAspNetUsersGroup(id, aspNetUsersGroup);
        }

        // POST: api/AspNetUsersGroups
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<responseData> PostAspNetUsersGroup(AspNetUsersGroup aspNetUsersGroup)
        {
            return await _aspNetUsersGroupsService.PostAspNetUsersGroup(aspNetUsersGroup);
        }

        // DELETE: api/AspNetUsersGroups/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<responseData> DeleteAspNetUsersGroup(int id)
        {
            return await _aspNetUsersGroupsService.DeleteAspNetUsersGroup(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAspNetUsersGroupByCustomer")]
        public async Task<responseData> GetAspNetUsersGroupByCustomer(int idCustomer)
        {
            return await _aspNetUsersGroupsService.GetAspNetUsersGroupByCustomer(idCustomer);
        }

        [HttpGet]
        [Route("GetGroupsByTenant")]
        public async Task<responseData> GetGroupsByTenant([FromHeader] PaginatorData paginatorData)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return await _aspNetUsersGroupsService.GetGroupsByTenant(paginatorData, userId);
        }

        [HttpGet]
        [Route("GetGroupByTenant")]
        public async Task<responseData> GetGroupByTenant(int userGroup)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return await _aspNetUsersGroupsService.GetGroupByTenant(userGroup, userId);
        }

        [HttpPut]
        [Route("PutGroupByTenant")]
        public async Task<responseData> PutGroupByTenant(AspNetUsersGroup userGroup)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return await _aspNetUsersGroupsService.PutGroupByTenant(userGroup, userId);
        }

        [HttpPost]
        [Route("PostGroupByTenant")]
        public async Task<responseData> PostGroupByTenant(AspNetUsersGroup userGroup)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return await _aspNetUsersGroupsService.PostGroupByTenant(userGroup, userId);
        }

        [HttpDelete]
        [Route("DeleteGroupByTenant")]
        public async Task<responseData> DeleteGroupByTenant(int userGroup)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return await _aspNetUsersGroupsService.DeleteGroupByTenant(userGroup, userId);
        }

        [HttpGet]
        [Route("GetPermissionByGroup")]
        public async Task<responseData> GetPermissionByGroup(int userGroup, bool all)
        {            
            return await _aspNetUsersGroupsService.GetPermissionByGroup(userGroup, all);
        }

        [HttpPut]
        [Route("UpdatePermissionByGroup")]
        public async Task<responseData> UpdatePermissionByGroup(List<MenuItemDTO> menuItems)
        {            
            return await _aspNetUsersGroupsService.UpdatePermissionByGroup(menuItems);
        }


    }
}
