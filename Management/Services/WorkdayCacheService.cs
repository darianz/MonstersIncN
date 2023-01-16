using Management.Controllers;
using Management.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Text;

namespace Management.Services
{
    public class WorkdayCacheService
    {
        public readonly IMemoryCache _cache;
        private readonly ILogger<WorkdayController> _logger;
        private readonly ILoggerFactory _loggerFactory;




        public WorkdayCacheService(IMemoryCache cache, ILoggerFactory loggerFactory)
        {
            _cache = cache;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<WorkdayController>();

        }

        public async Task UpdateCacheFromEndScare(int id, object doorUpdateResult, string userId)
        {
            _logger.LogInformation("Entered UpdateCacheFromStartScare");
            try
            {
                var currentDate = DateTime.Now.Date;
                var doorUpdateResultList = new List<object>();
                doorUpdateResultList.Add(doorUpdateResult);
                var workdayCacheInfo = new WorkdayCacheInfo
                {
                    Date = currentDate,
                    DoorUpdateResults = doorUpdateResultList
                };
                List<WorkdayCacheInfo> userInfo = _cache.Get(userId) as List<WorkdayCacheInfo>;
                if (userInfo == null)
                {
                    userInfo = new List<WorkdayCacheInfo>();
                    userInfo.Add(workdayCacheInfo);
                    _cache.Set(userId, userInfo);
                }
                else
                {
                    var existingWorkdayCacheInfo = userInfo.FirstOrDefault(w => w.Date == currentDate);
                    if (existingWorkdayCacheInfo != null)
                    {
                        existingWorkdayCacheInfo.DoorUpdateResults.AddRange(doorUpdateResultList);
                    }
                    else
                    {
                        userInfo.Add(workdayCacheInfo);
                    }
                    _cache.Set(userId, userInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpdateCacheFromStartScare: " + ex.Message);
                throw new Exception(ex.Message, ex);
            }
        }

        public void SumEnergyByDate(string userId)
        {
            var userInfo = _cache.Get(userId) as List<WorkdayCacheInfo>;
            if (userInfo != null)
            {
                var groupByDate = userInfo.GroupBy(w => w.Date);
                foreach (var dateGroup in groupByDate)
                {
                    var totalEnergy = dateGroup.SelectMany(w => w.DoorUpdateResults.Cast<DoorUpdateResult>()).Sum(d => d.Energy);

                    _logger.LogInformation($"Total energy on {dateGroup.Key.ToShortDateString()} is {totalEnergy}");
                }
            }
        }

        public List<WorkdayCacheInfo> GetAllCacheData(string userId)
        {
            return _cache.Get(userId) as List<WorkdayCacheInfo>;
        }


        private int CalculateDailyGoal(DateTime UserScaringStartDate)
        {
            DateTime currentTime = DateTime.Now;
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
        public List<WorkdayCacheInfo> GetWorkdayCacheInfoByDateRange(string userId, DateTime startDate, DateTime endDate)
        {
            List<WorkdayCacheInfo> userInfo = _cache.Get(userId) as List<WorkdayCacheInfo>;
            if (userInfo == null)
            {
                return new List<WorkdayCacheInfo>();
            }
            else
            {
                return userInfo.Where(w => w.Date >= startDate && w.Date <= endDate).ToList();
            }
        }
    }



}





