using masterCore.Entities;
using masterCore.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace masterCore.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepo _customerRepo;

        public CustomerService(ICustomerRepo customerRepo)
        {
            _customerRepo = customerRepo;
        }

        public async Task<responseData> GetCustomer(int id)
        {
            return await _customerRepo.GetCustomer(id);
        }

        public async Task<responseData> GetCustomers(PaginatorData paginatorData)
        {
            var customersResponse = await _customerRepo.GetCustomers(paginatorData);

            if (customersResponse.error)
            {
                return customersResponse;
            }

            var customers = (List<Customer>)customersResponse.data;

            var paginatorResult = Paginator<Customer>.Create(customers, paginatorData);

            addFields(paginatorResult);

            customersResponse.data = paginatorResult;

            return customersResponse;
        }

        private void addFields(Paginator<Customer> paginatorResult)
        {
            var _fieldData = new fieldData
            {
                order = 1,
                name = "",
                field = "Id",
                type = "text",
                display = false,
                colSize = 1
            };

            paginatorResult.fields = new List<fieldData>
            {
                _fieldData
            }; 
            
            _fieldData = new fieldData
            {
                order = 2,
                name = "Name",
                field = "name",
                type = "text",
                display = true,
                colSize = 20,
                sort = "asc"
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 3,
                name = "Email",
                field = "email",
                type = "text",
                display = true,
                colSize = 10
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 3,
                name = "Phone",
                field = "phone",
                type = "text",
                display = true,
                colSize = 5
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 4,
                name = "Address",
                field = "address",
                type = "text",
                display = true,
                colSize = 10
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 5,
                name = "City",
                field = "city",
                type = "text",
                display = true,
                colSize = 5
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 6,
                name = "State",
                field = "state",
                type = "text",
                display = true,
                colSize = 5
            };

            paginatorResult.fields.Add(_fieldData);

            _fieldData = new fieldData
            {
                order = 7,
                name = "ZIP",
                field = "zip",
                type = "text",
                display = true,
                colSize = 5
            };

            paginatorResult.fields.Add(_fieldData);
        }

        public async Task<responseData> PostCustomer(Customer customer)
        {
            return await _customerRepo.PostCustomer(customer);
        }

        public async Task<responseData> PutCustomer(int id, Customer customer)
        {
            return await _customerRepo.PutCustomer(id, customer);
        }

        public async Task<responseData> DeleteCustomer(int id)
        {
            return await _customerRepo.DeleteCustomer(id);
        }
    }
}
