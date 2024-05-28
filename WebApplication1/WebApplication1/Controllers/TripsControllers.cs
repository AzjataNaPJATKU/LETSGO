using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers;
[ApiController]
[Route("api/{controllers}")]
public class TripsControllers: ControllerBase
{
    private readonly BruhContext _context;

    public TripsControllers(BruhContext bruhContext)
    {
        _context = bruhContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var trips = await _context.Trips.Select(e=>new {Name = e.Name, Countries = e.IdCountries.Select(
            c=>new {Name = c.Name})}).ToListAsync();
        
        return Ok();
    }
}