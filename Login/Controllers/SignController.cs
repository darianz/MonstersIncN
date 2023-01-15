using Login.DTOs;
using Login.Interfaces;
using Login.Migrations;
using Login.Models;
using Login.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Login.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SignController : Controller
    {
        private readonly ILogger<SignController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly UserService _userService;
        private readonly ITokenService _tokenService;
       

        public SignController(UserService userService,AppDbContext context, ITokenService tokenService, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<SignController>();
            _tokenService = tokenService;
   
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<UserDetailsDto>> GetUserById()
        {
            _logger.LogInformation("Entered HttpGet(\"GetUserById\")");
            try
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];
                string userId = _tokenService.GetIdFromToken(authHeader);

                var user = await _userService.GetUserById(int.Parse(userId));
                if (user == null)
                {
                    return NotFound();
                }
                var userDetails = new UserDetailsDto
                {
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    TentaclesNumber = user.TentaclesNumber,
                    ScaringStartDate = user.ScaringStartDate
                };
                return Ok(userDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            _logger.LogInformation("Entered HttpPost(\"register\")");
            try
            {
                var user = await _userService.RegisterUser(registerDto);
                return new UserDto
                {
                    Username = user.Username,
                    Token = _tokenService.CreateTokenN(user)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Entered HttpPost(\"login\")");
            try
            {
                var user = await _userService.Login(loginDto);
                return new UserDto
                {
                    Username = user.Username,
                    Token = _tokenService.CreateTokenN(user)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("UpdateUserDetails")]
        public async Task<IActionResult> UpdateUserById( [FromBody] UpdateUserDto patchDoc)
        {
            _logger.LogInformation("Entered HttpPatch(\"UpdateUserById\")");
            string authHeader = HttpContext.Request.Headers["Authorization"];
            string userId = _tokenService.GetIdFromToken(authHeader);
           
            try
            {
               var Response =  await _userService.UpdateUser(int.Parse(userId), patchDoc);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("UpdateScaringStartDate")]
        public async Task<IActionResult> UpdateScaringStartDate([FromBody] DateTime ScaringStartDate)
        {
            _logger.LogInformation("Entered HttpPatch(\"UpdateUserById\")");
            string authHeader = HttpContext.Request.Headers["Authorization"];
            string userId = _tokenService.GetIdFromToken(authHeader);
            try
            {
                var Response = await _userService.UpdateScaringStartDate(int.Parse(userId), ScaringStartDate);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}






