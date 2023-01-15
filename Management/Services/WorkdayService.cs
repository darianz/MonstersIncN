using Management.Controllers;
using Management.DTOs;
using Management.Models;
using Management.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Management.Services
{
    public class WorkdayService
    {

        private readonly ILogger<WorkdayService> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly DoorService _doorService;
        private readonly UserService _userService;
        private readonly TokenService _tokenService;
        private readonly WorkdayCacheService _workdayCacheService;

        public WorkdayService(IHttpClientFactory clientFactory, ILoggerFactory loggerFactory, UserService userService, TokenService tokenService, WorkdayCacheService workdayCacheService, DoorService doorService)
        {

            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<WorkdayService>();
            _userService = userService;
            _tokenService = tokenService;
            _workdayCacheService = workdayCacheService;
            _doorService = doorService;
        }

        public async Task<string> GetAvailableWorkday(string authHeader)
        {

            return await _doorService.GetAvailableDoors(authHeader);

        }

        public async Task<string> StartWorkday(int doorId, string authHeader)
        {
            _logger.LogInformation("Entered HttpPatch(\"StartWorkday\")");

            try
            {
                var userId = _tokenService.GetIdFromToken(authHeader);
                var StartScareResponse = await _doorService.StartScare(doorId, authHeader);
                // Check if StartWorkday Date is Init
                string userDetails = await _userService.GetUserById(authHeader);
                UserDetailsDto userDetailsDto = JsonConvert.DeserializeObject<UserDetailsDto>(userDetails);
                DateTime currentTime = DateTime.Now;
                // If not worked before update the date of first workday
                if (userDetailsDto.ScaringStartDate == DateTime.MinValue)
                {
                    var UpdateFirstWorkdayTimeResponse = _userService.UpdateScaringStartDate(currentTime, authHeader);
                }
             
                return StartScareResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> EndWorkday(int doorId, string authHeader)
        {
            

            try
            {
                var userId = _tokenService.GetIdFromToken(authHeader);
                var EndScareResponse = await _doorService.EndScare(doorId, authHeader);
                // Check if StartWorkday Date is Init
               
  
            
             
                //int DailyGoal = CalculateDailyGoal(currentTime, userDetailsDto.ScaringStartDate);
                // Update Cache
                await _workdayCacheService.UpdateCacheFromEndScare(doorId, EndScareResponse, userId);
               
             
                return EndScareResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception(ex.Message);
            }
        }

        private int CalculateDailyGoal(DateTime currentTime, DateTime UserScaringStartDate)
        {
            // Bring User Exp and see if he worked before, calculate energy demand
            int yearsDelta = currentTime.Year - UserScaringStartDate.Year;
            if (yearsDelta < 0)
            {
                return 100;
            }
            else
            {
                return ((yearsDelta * 20) + 100);
            }
        }
    }
}