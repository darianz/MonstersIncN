using Doors.Exceptions;
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
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogError(ex, "Token has expired");
                throw new InvalidTokenException("Token has expired");
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                _logger.LogError(ex, "Invalid token signature");
                throw new InvalidTokenException("Invalid token signature");
            }
            catch (SecurityTokenInvalidLifetimeException ex)
            {
                _logger.LogError(ex, "Invalid token lifetime");
                throw new InvalidTokenException("Invalid token lifetime");
            }
            catch (SecurityTokenInvalidIssuerException ex)
            {
                _logger.LogError(ex, "Invalid token issuer");
                throw new InvalidTokenException("Invalid token issuer");
            }
            catch (SecurityTokenInvalidAudienceException ex)
            {
                _logger.LogError(ex, "Invalid token audience");
                throw new InvalidTokenException("Invalid token audience");
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Invalid token");
                throw new InvalidTokenException("Invalid token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while validating the token");
                throw new InvalidTokenException("An error occured while validating the token");
            }
        }
    }
}



//using Doors.Exceptions;
//using Doors.Models;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Reflection.PortableExecutable;
//using System.Security.Claims;
//using System.Text;

//namespace Doors.Services
//{
//    public class TokenService 
//    {
//        private readonly ILogger<TokenService> _logger;
//        private readonly ILoggerFactory _loggerFactory;
//        private readonly IConfiguration _config;
//        public TokenService(IConfiguration config, ILoggerFactory loggerFactory)
//        {
//            _loggerFactory = loggerFactory;
//            _logger = _loggerFactory.CreateLogger<TokenService>();
//            _config = config;

//        }

//        public void IsTokenValid(string token)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var validationParameters = new TokenValidationParameters
//            {
//                ValidateActor = false,
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,
//                ValidIssuer = _config["Jwt:Issuer"],
//                ValidAudience = _config["Jwt:Audience"],
//                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]))
//            };

//            try
//            {
//                // Validate the token
//                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);


//            }
//            catch 
//            {
//                _logger.LogError("Unauthorized");
//                throw new InvalidTokenException("Unauthorized");


//            }
//        }
//    }
//}
