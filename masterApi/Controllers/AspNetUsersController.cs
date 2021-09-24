using masterCore.Entities;
using masterCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace masterApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AspNetUsersController : ControllerBase
    {
        private readonly IAspNetUserService _aspNetUserService;

        public AspNetUsersController(IAspNetUserService aspNetUserService)
        {
            _aspNetUserService = aspNetUserService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<responseData> PostAspNetUserUser([FromBody] AspNetUser applicationUser)
        {
            var responseData = new responseData();

            try
            {
                return await _aspNetUserService.PostAspNetUserUser(applicationUser);
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

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<responseData> PutAspNetUserUser(string id, AspNetUser applicationUser)
        {
            var responseData = new responseData();

            try
            {
                return await _aspNetUserService.PutAspNetUserUser(applicationUser);
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetUsersByCustomer")]
        public async Task<responseData> GetUsersByCustomer(int customer)
        {
            return await _aspNetUserService.GetUsersByCustomer(customer);
        }
        
        [HttpGet]
        [Route("GetUsersByTenant")]
        public async Task<responseData> GetUsersByTenant([FromHeader] PaginatorData paginatorData)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return await _aspNetUserService.GetUsersByTenant(paginatorData, userId);
        }

        [HttpGet]
        [Route("GetUserByTenant")]
        public async Task<responseData> GetUserByTenant(string userId)
        {
            var currentUserId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return await _aspNetUserService.GetUserByTenant(currentUserId, userId);
        }

        [HttpPut]
        [Route("PutUserByTenant")]
        public async Task<responseData> PutUserByTenant(AspNetUser user)
        {
            var currentUserId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return await _aspNetUserService.PutUserByTenant(currentUserId, user);
        }

        [HttpPost]
        [Route("PostUserByTenant")]
        public async Task<responseData> PostUserByTenant(AspNetUser user)
        {
            var currentUserId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return await _aspNetUserService.PostUserByTenant(currentUserId, user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetUsers")]
        public async Task<responseData> GetUsers([FromHeader] PaginatorData paginatorData)
        {
            return await _aspNetUserService.GetUsers(paginatorData);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<responseData> GetUser(string id)
        {
            return await _aspNetUserService.GetUser(id);
        }
    }
}
