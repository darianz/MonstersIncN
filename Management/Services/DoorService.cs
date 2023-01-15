using Management.Controllers;
using Management.DTOs;
using Management.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Management.Services
{
    public class DoorService
    {
        private readonly HttpClient _doorsClient;
        private readonly ILogger<DoorService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly UserService _userService;
        private readonly TokenService _tokenService;
        private readonly WorkdayCacheService _doorCacheService; 

        public DoorService(IHttpClientFactory clientFactory, ILoggerFactory loggerFactory, UserService userService, TokenService tokenService, WorkdayCacheService doorCacheService)
        {
            _doorsClient = clientFactory.CreateClient("DoorsClient");
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<DoorService>();
            _userService = userService;
            _tokenService = tokenService;
            _doorCacheService= doorCacheService;

        }

        public async Task<string> GetAvailableDoors(string authHeader)
        {
            _doorsClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authHeader);
            var response = await _doorsClient.GetAsync($"doors");
            string responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        
        public async Task<string> StartScare(int doorId, string authHeader)
        {
            _logger.LogInformation("Entered HttpPatch(\"StartScare\")");

            try
            {
                var doorUpdateRequest = new DoorUpdateRequest
                {
                    Used = true,
                };
                var requestBody = JsonConvert.SerializeObject(doorUpdateRequest);
                var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                _doorsClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authHeader);
                var response = await _doorsClient.PatchAsync($"doors/{doorId}", httpContent);
                if (!response.IsSuccessStatusCode) { throw new Exception("Door Already Used"); }
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message);
            }

        }

        public async Task<string> EndScare(int doorId, string authHeader)
        {
            _logger.LogInformation("Entered HttpPatch(\"EndScare\")");

            try
            {
                var doorUpdateRequest = new DoorUpdateRequest
                {
                    Used = null,
                };
                var requestBody = JsonConvert.SerializeObject(doorUpdateRequest);
                var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
                _doorsClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authHeader);
                var response = await _doorsClient.PatchAsync($"doors/{doorId}", httpContent);
                if (!response.IsSuccessStatusCode) { throw new Exception("End Scare Error"); }
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message);
            }

        }
    }
}
