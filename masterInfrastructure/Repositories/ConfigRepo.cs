using masterCore.Entities;
using masterCore.Interfaces;
using masterInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace masterInfrastructure.Repositories
{
    public class ConfigRepo : IConfigRepo
    {
        private readonly AppDbContext _context;

        public ConfigRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<responseData> GetConfig(int id)
        {
            var responseData = new responseData();
            try
            {
                responseData.data = await _context.Configs.FirstOrDefaultAsync(x => x.Id == id);
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

        public async Task<responseData> GetConfigs()
        {
            var responseData = new responseData();
            try
            {
                responseData.data = await _context.Configs.ToListAsync();
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

        public async Task<responseData> PutConfig(int id, Config config)
        {
            var responseData = new responseData();
            try
            {
                if (id != config.Id)
                {
                    responseData.error = true;
                    responseData.errorValue = 2;
                    responseData.description = "Not Found!";
                    return responseData;
                }

                _context.Entry(config).State = EntityState.Modified;
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

        public async Task<responseData> GetConfigByName(string propName)
        {
            var responseData = new responseData();
            try
            {
                responseData.data = await _context.Configs.FirstOrDefaultAsync(x => x.propName == propName);
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
    }
}
