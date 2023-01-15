using Login.Controllers;
using Login.Interfaces;
using Login.Models;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Login.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;
        private readonly ILogger<TokenService> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public TokenService(IConfiguration config, ILoggerFactory loggerFactory)
        {
            _config= config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<TokenService>();
        }
        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                    new Claim(JwtRegisteredClaimNames.NameId, user.Username),

            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {

                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string CreateTokenN(User user)
        {

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var header = new JwtHeader(credentials)
            {
                { "kid", _config["TokenKey"] }
            };

            var claims = new List<Claim>
            {
                    new Claim(JwtRegisteredClaimNames.NameId, user.Username),
                    new Claim("userId", user.Id.ToString()),


            };

            var Payload = new JwtPayload(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, DateTime.UtcNow, DateTime.UtcNow.AddHours(24));

            var token = new JwtSecurityToken(header, Payload);

            var tokenN = new JwtSecurityTokenHandler().WriteToken(token);
            if (IsTokenValid(tokenN)) { return tokenN; }
            else
            {
                _logger.LogError("Token is not valid at CreateToken");
                throw new Exception("token is not valid");
            }



        }
        public bool IsTokenValid(string token)
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
           
              
              
                // Token is valid
                return true;
            }
            catch (SecurityTokenException)
            {
                // Token is invalid
                return false;
            }
        }

        public string GetIdFromToken(string token)
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
                var jwt = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var claims = jwt.Claims;
              
                foreach (Claim claim in claims)
                {
                    if (claim.Type == "userId")
                    {
                        return claim.Value;
                        
                    }
                }
                return "";
            }
            catch (SecurityTokenException)
            {
                // Token is invalid
                _logger.LogError("Token is not valid at CreateToken");
                throw new Exception("Error in GetIdFromToken");
            }
        }
    }
}
