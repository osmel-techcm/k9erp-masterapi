using System.Threading.Tasks;
using masterCore.Entities;

namespace masterCore.Interfaces
{
    public interface IMenuItemRepo
    {
        Task<responseData> GetMenuItems();

        Task<responseData> GetMenuRawItems();
    }
}
