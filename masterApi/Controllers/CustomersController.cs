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
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<responseData> GetCustomers([FromHeader] PaginatorData paginatorData)
        {
            return await _customerService.GetCustomers(paginatorData);
        }

        [HttpGet("{id}")]
        public async Task<responseData> GetCustomer(int id)
        {
            return await _customerService.GetCustomer(id);
        }

        [HttpPut("{id}")]
        public async Task<responseData> PutCustomer(int id, Customer customer)
        {
            return await _customerService.PutCustomer(id, customer);
        }


        [HttpPost]
        public async Task<responseData> PostCustomer(Customer customer)
        {
            return await _customerService.PostCustomer(customer);
        }

        [HttpDelete("{id}")]
        public async Task<responseData> DeleteCustomer(int id)
        {
            return await _customerService.DeleteCustomer(id);
        }
        
    }
}
