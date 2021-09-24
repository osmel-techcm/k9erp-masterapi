using masterCore.Entities;
using masterCore.Interfaces;
using System.Threading.Tasks;

namespace masterCore.Services
{
    public class ConfigService: IConfigService
    {
        private readonly IConfigRepo _iconfigRepo;

        public ConfigService(IConfigRepo iconfigRepo)
        {
            _iconfigRepo = iconfigRepo;
        }

        public async Task<responseData> GetConfig(int id)
        {
            return await _iconfigRepo.GetConfig(id);
        }

        public async Task<responseData> GetConfigs()
        {
            return await _iconfigRepo.GetConfigs();
        }

        public async Task<responseData> PutConfig(int id, Config config)
        {
            return await _iconfigRepo.PutConfig(id, config);
        }

        public async Task<responseData> GetConfigByName(string propName)
        {
            return await _iconfigRepo.GetConfigByName(propName);
        }
    }
}
