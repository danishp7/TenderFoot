using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Repos
{
    public class EncryptRepo : IEncryptRepo
    {
        private readonly ILogger<EncryptRepo> _logger;
        private readonly IConfiguration _config;
        public EncryptRepo(ILogger<EncryptRepo> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
        }
        public List<byte[]> CreatePasswordWithEncryption(string password)
        {
            try
            {
                byte[] passwordHash, key;
                using (var hashedPassword = new System.Security.Cryptography.HMACSHA512())
                {
                    passwordHash = hashedPassword.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    key = hashedPassword.Key;
                }
                List<byte[]> password_hash_key = new List<byte[]>();
                password_hash_key.Add(passwordHash);
                password_hash_key.Add(key);
                return password_hash_key;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;
            }
        }
        public Task<bool> IsPassword(string password, byte[] hashPassword, byte[] key)
        {
            try
            {
                return Task.Run(() =>
                {
                    using (var hashedPasswordFromDb = new System.Security.Cryptography.HMACSHA512(key))
                    {
                        var enteredPasswordHash = hashedPasswordFromDb.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                        for (int i = 0; i < enteredPasswordHash.Length; i++)
                        {
                            if (hashPassword[i] != enteredPasswordHash[i])
                                return false;
                        }
                    }
                    return true;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Task.Run(() =>
                {
                    return false;
                });
            }
        }
        public async Task<string> ValidateToken(string token)
        {
            try
            {
                string id = string.Empty;
                await Task.Run(() =>
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);

                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken securityToken);

                    var jwtToken = (JwtSecurityToken)securityToken;
                    id = jwtToken.Claims.First(c => c.Type == "nameid").Value;
                });
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }

        }
    }
}
