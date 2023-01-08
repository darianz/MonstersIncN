using Doors.Models;
using Doors.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace Doors.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DoorsController : Controller
    {
        private readonly ILogger<DoorsController> _logger;
        private DoorService _doorService;
        private TokenService _tokenService;
        private readonly ILoggerFactory _loggerFactory;

        public DoorsController(AppDbContext context, DoorService doorService, TokenService tokenService, ILoggerFactory loggerFactory)
        {
            _tokenService = tokenService;
            _doorService = doorService;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<DoorsController>();
        }


        [HttpGet("doors")]
        
        public async Task<ActionResult<IEnumerable<Door>>> GetAll()
        {
            _logger.LogInformation("Entered HttpGet(\"doors\")");
            try
            {
                string authHeader = HttpContext.Request.Headers["Authorization"];
              _tokenService.IsTokenValid(authHeader);
                    return Ok(await _doorService.GetAllDoors());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("doors/{id}")]
        public async Task<IActionResult> UpdateDoor(int id, [FromBody] DoorUpdateRequest request)
        {
            _logger.LogInformation("Entered HttpPatch(\"doors\")");
            try
            {
                await _doorService.UpdateDoor(id, request);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }
}





