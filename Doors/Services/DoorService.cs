using Doors.Exceptions;
using Doors.Exceptions.Doors.Exceptions;
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

        public async Task<object> UpdateDoor(int id, DoorUpdateRequest request)
        {
            var door = await _context.Door.FindAsync(id);

            if (door == null)
            {
                _logger.LogError("Door not found");
                throw new DoorNotFoundException("Door not found");
            }
            if (request.Used.HasValue)
            {
                if (door.Used) { throw new DoorAlreadyUsedException("Door Already Used"); }
                door.Used = true;
                await _context.SaveChangesAsync();
                return door;
            }
            else
            {
                var energyTaken = door.Energy;
                door.Energy = 0;
                await _context.SaveChangesAsync();
                return new Door { Energy = energyTaken , Id = door.Id, Used= door.Used};
                
            }
        }
    }
}
