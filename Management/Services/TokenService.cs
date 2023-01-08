
using Management.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;

namespace Management.Services
{
    public class TokenService 
    {
      
        private readonly IConfiguration _config;
        private readonly ILogger<TokenService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        public TokenService(IConfiguration config, ILoggerFactory loggerFactory)
        {
            _config= config;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<TokenService>();

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
                _logger.LogError("Error in IsTokenValid");
                throw new Exception("Unauthorized");


            }
        }
    }
}
