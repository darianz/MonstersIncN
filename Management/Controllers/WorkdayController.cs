using Management.DTOs;
using Management.Models;
using Management.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkdayController : ControllerBase
    {
        private readonly ILogger<WorkdayController> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly HttpClient _loginClient;
        private readonly HttpClient _doorsClient;
        private readonly TokenService _tokenService;
        private readonly WorkdayCacheService _workdayCacheService;
        private readonly DoorService _doorsService;
        private readonly UserService _userService;
        private readonly WorkdayService _workdayService;

        public WorkdayController(IHttpClientFactory clientFactory, TokenService tokenService, ILoggerFactory loggerFactory, WorkdayCacheService workdayCacheService, DoorService doorsService, UserService userService, WorkdayService workdayService)
        {
            
            _loginClient = clientFactory.CreateClient("LoginClient");
            _doorsClient = clientFactory.CreateClient("DoorsClient");
            _tokenService= tokenService;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<WorkdayController>();
           _workdayCacheService= workdayCacheService;
            _doorsService= doorsService;
            _userService = userService;
            _workdayService= workdayService;
        }

        
        [HttpGet("doors")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableDoors()
        {
            _logger.LogInformation("Entered HttpGet(\"doors\")");
            string authHeader = HttpContext.Request.Headers["Authorization"];
            _tokenService.IsTokenValid(authHeader);
            _doorsClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authHeader);
            var doors = await _doorsService.GetAvailableDoors(authHeader);
            return Ok(doors);
        }

        
        [HttpGet("WorkerDetails")]
        public async Task<ActionResult<UserDetailsDto>> GetUserById()
        {
            _logger.LogInformation("Entered HttpGet(\"WorkerDetails\")");
            try
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];
                _tokenService.IsTokenValid(authHeader);
                var responseBody = await _userService.GetUserFullDetails(authHeader);
                return Ok(responseBody);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
       
            
        }


        [HttpPatch("StartScare/{doorId}")]
        public async Task<ActionResult<IEnumerable<string>>> StartScare(int doorId)
        {
            _logger.LogInformation("Entered HttpPatch(\"StartScare\")");

            try
            {
                // auth
                string authHeader = HttpContext.Request.Headers["Authorization"];
                _tokenService.IsTokenValid(authHeader);
                await _workdayService.StartWorkday(doorId, authHeader);
                return Ok("StartScare Worked !");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
         
        }

        [HttpPatch("EndScare/{doorId}")]
        public async Task<ActionResult<IEnumerable<string>>> EndScare(int doorId)
        {
            _logger.LogInformation("Entered HttpPatch(\"EndScare\")");
         
            try
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];
                _tokenService.IsTokenValid(authHeader);
                await _workdayService.EndWorkday(doorId, authHeader);
                return Ok("EndWorkday Worked !");
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return BadRequest(ex.Message);
            }
          
        }

        [HttpPatch("UpdateUserDetails")]
        public async Task<IActionResult> UpdateUserById([FromBody] UpdateUserDto patchDoc)
        {
            _logger.LogInformation("Entered HttpPatch(\"UpdateUserById\")");
            string authHeader = HttpContext.Request.Headers["Authorization"];
            _tokenService.IsTokenValid(authHeader);
            try
            {
                await _userService.UpdateUserById(patchDoc, authHeader);
                return Ok("the user has been updated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("workdaycache/{startDate}/{endDate}")]
        public IActionResult GetWorkdayCacheByDateRange( DateTime startDate, DateTime endDate)
        {
            DateTime current = DateTime.Now;
            _logger.LogInformation("Entered HttpPatch(\"UpdateUserById\")");
            string authHeader = HttpContext.Request.Headers["Authorization"];
            string userId = _tokenService.GetIdFromToken(authHeader);
            var workdayCacheInfo = _workdayCacheService.GetWorkdayCacheInfoByDateRange(userId, startDate, endDate);
            if (workdayCacheInfo == null)
            {
                return NotFound();
            }
            return Ok(workdayCacheInfo);
        }


    }
}
