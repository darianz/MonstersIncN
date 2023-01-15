using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Text.Json;

namespace Doors.Models
{
    public class Seed
    {
        public static async Task SeedDoors(AppDbContext context)
        {
            try
            {
                //if (await context.Door.AnyAsync()) return;
                
                // reset the Door Table
                await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Door");

                var doorsData = await File.ReadAllTextAsync("./Migrations/DoorsSeedData.json");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var doors = JsonSerializer.Deserialize<List<Door>>(doorsData);

                foreach (var door in doors)
                {
                    door.Used = door.Used;
                    door.Energy = door.Energy;

                    context.Door.Add(door);
                }

                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}
