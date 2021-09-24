using masterCore.Entities;
using masterCore.Interfaces;
using masterInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace masterInfrastructure.Repositories
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly AppDbContext _context;

        public CustomerRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<responseData> GetCustomer(int id)
        {
            var responseData = new responseData();

            try
            {
                responseData.data = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<responseData> GetCustomers(PaginatorData paginatorData)
        {
            var responseData = new responseData();

            var customerDb = _context.Customers.Where("1 = 1");

            if (!string.IsNullOrEmpty(paginatorData.filterDataSt))
            {
                customerDb = insertFilters(customerDb, paginatorData.filterDataSt);
            }            

            try
            {
                if (string.IsNullOrEmpty(paginatorData.orderField))
                {
                    paginatorData.orderField = "Id";
                }

                responseData.data = await customerDb
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

            return responseData;
        }

        public async Task<responseData> PostCustomer(Customer customer)
        {
            var responseData = new responseData();

            try
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                responseData.data = customer;
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

        public async Task<responseData> PutCustomer(int id, Customer customer)
        {
            var responseData = new responseData();

            try
            {
                if (id != customer.Id)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Not Found!";
                    return responseData;
                }

                _context.Entry(customer).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                responseData.data = customer;
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

        public async Task<responseData> DeleteCustomer(int id)
        {
            var responseData = new responseData
            {
                description = "Deleted!"
            };

            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Not Found!";
                    return responseData;
                }

                _context.Customers.Remove(customer);
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

        private IQueryable<Customer> insertFilters(IQueryable<Customer> customerDb, string filterDataSt)
        {
            var jFilterDataSt = JObject.Parse(filterDataSt);
            foreach (var item in jFilterDataSt)
            {
                switch (item.Key.ToLower())
                {
                    case "name":
                        customerDb = customerDb.Where(c => c.Name.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "email":
                        customerDb = customerDb.Where(c => c.Email.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "phone":
                        customerDb = customerDb.Where(c => c.Phone.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "address":
                        customerDb = customerDb.Where(c => c.Address.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "city":
                        customerDb = customerDb.Where(c => c.City.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "state":
                        customerDb = customerDb.Where(c => c.State.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                    case "zip":
                        customerDb = customerDb.Where(c => c.ZIP.ToLower().Contains(item.Value.ToString().ToLower()));
                        break;
                }
            }

            return customerDb;
        }
    }
}
