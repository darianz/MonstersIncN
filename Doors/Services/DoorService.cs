using Doors.Models;
using Microsoft.EntityFrameworkCore;

namespace Doors.Services
{
    public class DoorService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DoorService> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public DoorService(AppDbContext context, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<DoorService>();
            _context = context;
        }

        public async Task<IEnumerable<Door>> GetAllDoors()
        {
            return await _context.Door
                .Where(d => !d.Used)
                .ToListAsync();
        }

        public async Task UpdateDoor(int id, DoorUpdateRequest request)
        {
            var door = await _context.Door.FindAsync(id);

            if (door == null)
            {
                _logger.LogError("Door not found");
                throw new Exception("Door not found");
            }
            if (request.Used.HasValue)
            {
                door.Used = true;
            }
            else
            {
                door.Energy = 0;
            }

            await _context.SaveChangesAsync();
        }
    }

}
