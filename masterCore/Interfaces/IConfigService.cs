using masterCore.Entities;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface IConfigService
    {
        Task<responseData> GetConfigs();
        Task<responseData> GetConfig(int id);
        Task<responseData> PutConfig(int id, Config config);
        Task<responseData> GetConfigByName(string propName);
    }
}
