using masterCore.Entities;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface ICustomerRepo
    {
        Task<responseData> GetCustomers(PaginatorData paginatorData);
        Task<responseData> GetCustomer(int id);
        Task<responseData> PostCustomer(Customer customer);
        Task<responseData> PutCustomer(int id, Customer customer);
        Task<responseData> DeleteCustomer(int id);
    }
}
