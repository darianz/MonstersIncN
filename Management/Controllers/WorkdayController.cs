using Management.DTOs;
using Management.Models;
using Management.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Net.Http.Headers;
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
        private readonly IDistributedCache _cache;
        
        public WorkdayController(IHttpClientFactory clientFactory, TokenService tokenService, IDistributedCache cache, ILoggerFactory loggerFactory)
        {
            
            _loginClient = clientFactory.CreateClient("LoginClient");
            _doorsClient = clientFactory.CreateClient("DoorsClient");
            _tokenService= tokenService;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<WorkdayController>();
            _cache = cache;

        }

        
        [HttpGet("doors")]
        public async Task<ActionResult<IEnumerable<string>>> GetAvailableDoors()
        {
            _logger.LogInformation("Entered HttpGet(\"doors\")");
            string authHeader = HttpContext.Request.Headers["Authorization"];
            _tokenService.IsTokenValid(authHeader);
            _doorsClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authHeader);
            var response = await _doorsClient.GetAsync($"doors");
            string responseBody = await response.Content.ReadAsStringAsync();

            return Ok(responseBody);
        }

        
        [HttpGet("WorkerDetails")]
        public async Task<ActionResult<UpdateUserDto>> GetUserById(int id)
        {
            _logger.LogInformation("Entered HttpGet(\"WorkerDetails\")");
            try
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];
                _tokenService.IsTokenValid(authHeader);
                _loginClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authHeader);
                var response = await _loginClient.GetAsync($"");
                string responseBody = await response.Content.ReadAsStringAsync();

                return Ok(responseBody);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
       
            
        }


        [HttpPatch("StartScare/{id}")]
        public async Task<ActionResult<IEnumerable<string>>> StartScare(int id)
        {
            _logger.LogInformation("Entered HttpPatch(\"StartScare\")");
            var doorUpdateRequest = new DoorUpdateRequest
            {
                Used = true,
            };
            var requestBody = JsonConvert.SerializeObject(doorUpdateRequest);

            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
           
            var response = await _doorsClient.PatchAsync($"doors/{id}", httpContent);
          
            string responseBody = await response.Content.ReadAsStringAsync();

            return Ok(responseBody);
        }

        [HttpPatch("EndScare/{id}")]
        public async Task<ActionResult<IEnumerable<string>>> EndScare(int id)
        {
            _logger.LogInformation("Entered HttpPatch(\"EndScare\")");
            var doorUpdateRequest = new DoorUpdateRequest
            {
                Used = null,
            };
            var requestBody = JsonConvert.SerializeObject(doorUpdateRequest);

            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var response = await _doorsClient.PatchAsync($"doors/{id}", httpContent);

            string responseBody = await response.Content.ReadAsStringAsync();

            return Ok(responseBody);
        }


    }
}
