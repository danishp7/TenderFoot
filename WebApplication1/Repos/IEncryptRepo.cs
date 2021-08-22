using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Repos
{
    public interface IEncryptRepo
    {
        Task<bool> IsPassword(string password, byte[] hashPassword, byte[] key);
        List<byte[]> CreatePasswordWithEncryption(string password);
        Task<string> ValidateToken(string token);
    }
}