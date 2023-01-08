
using Doors.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;

namespace Doors.Services
{
    public class TokenService 
    {
        private readonly ILogger<TokenService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<TokenService>();
            _config = config;
           
        }

        public void IsTokenValid(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateActor = false,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]))
            };

            try
            {
                // Validate the token
                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

               
            }
            catch (SecurityTokenException)
            {
                _logger.LogError("Unauthorized");
                throw new Exception("Unauthorized");
               
             
            }
        }
    }
}
