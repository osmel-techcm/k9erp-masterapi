using masterCore.Entities;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface IMenuItemService
    {
        Task<responseData> GetMenuItems();

        Task<responseData> GetMenuRawItems();
    }
}
