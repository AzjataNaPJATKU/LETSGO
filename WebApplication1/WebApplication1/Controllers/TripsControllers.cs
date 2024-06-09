using System.Runtime.InteropServices.JavaScript;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

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
        
        return Ok(trips);
    }
    [HttpDelete("/{idClient:int}")]
    public async Task<IActionResult> Delete(int idClient)
    {
        var syntax = _context.ClientTrips.Where(trip => trip.IdClient == idClient).GroupBy(ct => ct.IdClient)
            .Count();
        
        if ( syntax < 1)
        {
            return Conflict($"nie wiem czy prawidłowy kod, ale {idClient} nie może być usunięty, bo ma tripy");
        }

        await _context.Clients.Where(client => client.IdClient == idClient).ExecuteDeleteAsync();
        return Ok();
    }

    [HttpPost("trips/{idTrip:int}/clients")]
    public async Task<IActionResult> Post(String firstname, String lastname, String email, String telefon, String pesel,
        int idTrip, String tripName, DateTime? paymentDate)
    {
        if (await _context.Clients.AnyAsync(client => client.Pesel.Equals(pesel)))
        {
            return Conflict("juz istnieje");
        }

        if (await _context.ClientTrips.AnyAsync(trip =>
                trip.IdClient == _context.Clients.Where(client => client.Pesel == pesel)
                    .Select(client => client.IdClient).First() && trip.IdTrip == idTrip))
        {
            return Conflict("juz jest na tripie");
        }

        if(!await _context.Trips.AnyAsync(trip => trip.IdTrip == idTrip && trip.Name.Equals(tripName)))
        {
            return NotFound("gdzie ten trip");
        }

        if (_context.Trips.Where(trip => trip.IdTrip == idTrip).Select(trip => trip.DateFrom).First() < DateTime.Now)
        {
            return Conflict("juz minelo");
        }

        var newClient = new Client();
        newClient.IdClient = _context.Clients.Select(client => client.IdClient).Max()+1;
        newClient.FirstName=firstname;
        newClient.LastName = lastname;
        newClient.Email = email;
        newClient.Telephone = telefon;
        newClient.Pesel = pesel;

        var newClientTrip = new ClientTrip();
        newClientTrip.IdClient = newClient.IdClient;
        newClientTrip.IdTrip = idTrip;
        newClientTrip.RegisteredAt = DateTime.Now;
        newClientTrip.PaymentDate = paymentDate;

        await _context.Clients.AddAsync(newClient);
        await _context.ClientTrips.AddAsync(newClientTrip);
        
        return Ok();
    }
}