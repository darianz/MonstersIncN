using Management.Controllers;
using Management.DTOs;
using Management.Interfaces;
using Management.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Management.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _loginClient;
        private readonly ILogger<UserService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly TokenService _tokenService;
        private readonly WorkdayCacheService _workdayCacheService;
        public UserService(IHttpClientFactory clientFactory, ILoggerFactory loggerFactory, TokenService tokenService, WorkdayCacheService doorCacheService)
        {
            _loginClient = clientFactory.CreateClient("LoginClient");
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<UserService>();
            _tokenService = tokenService;
            _workdayCacheService = doorCacheService;
        }

        public async Task<string> GetUserById(string authHeader)
        {
            _logger.LogInformation("Entered HttpGet(\"WorkerDetails\")");
            try
            {
               
              
                _loginClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authHeader);
                var response = await _loginClient.GetAsync($"");
                string responseBody = await response.Content.ReadAsStringAsync();
                var userId = _tokenService.GetIdFromToken(authHeader);
                //string cache = _doorCacheService.GetAllCacheData(userId);
                // Include cache data in responseBody

                //responseBody += cache;
                return responseBody;
            }
            catch (Exception ex)
            {
                throw new Exception (ex.Message);
            }


        }

        public async Task<string> GetUserFullDetails(string authHeader)
        {
            _logger.LogInformation("Entered HttpGet(\"WorkerDetails\")");
            try
            {


                _loginClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authHeader);
                var response = await _loginClient.GetAsync($"");
                string responseBody = await response.Content.ReadAsStringAsync();
                var userId = _tokenService.GetIdFromToken(authHeader);
                List<WorkdayCacheInfo> cache = _workdayCacheService.GetAllCacheData(userId);
                // Include cache data in responseBody
                string cacheAsString = JsonConvert.SerializeObject(cache);
                responseBody += cacheAsString;
                return responseBody;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task UpdateUserById([FromBody] UpdateUserDto patchDoc, string authHeader)
        {
            _loginClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authHeader);
            var requestBody = JsonConvert.SerializeObject(patchDoc);
            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            try
            {
                var response = await _loginClient.PatchAsync($"UpdateUserDetails", httpContent);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateScaringStartDate([FromBody] DateTime ScaringStartDate, string authHeader)
        {
            DateTime currentTime = DateTime.Now;

            var requestBody = JsonConvert.SerializeObject(currentTime);

            var httpContent = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var UpdateFirstScareTimeResponse = await _loginClient.PatchAsync($"UpdateScaringStartDate", httpContent);


        }
    }
}
