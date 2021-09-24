using masterCore.Entities;
using masterCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace masterApi.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigsController : ControllerBase
    {
        private readonly IConfigService _config;

        public ConfigsController(IConfigService config)
        {
            _config = config;
        }

        // GET: api/Configs
        [HttpGet]
        public async Task<responseData> GetConfigs()
        {
            return await _config.GetConfigs();
        }

        // GET: api/Configs/5
        [HttpGet("{id}")]
        public async Task<responseData> GetConfig(int id)
        {
            return await _config.GetConfig(id);
        }

        [HttpPut("{id}")]
        public async Task<responseData> PutConfig(int id, Config config)
        {
            return await _config.PutConfig(id, config);
        }

    }
}
