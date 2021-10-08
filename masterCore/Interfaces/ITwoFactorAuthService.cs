using masterCore.Entities;
using System.Threading.Tasks;

namespace masterCore.Interfaces
{
    public interface ITwoFactorAuthService
    {
        Task<responseData> GenerateSetupCode(string issuer, string email, string key);
        Task<responseData> ValidateTwoFactorAuth(string key, string code);
        Task<string> EncriptToken(string jwt, string key);
        Task<string> DecriptToken(string jwt, string key);
    }
}
