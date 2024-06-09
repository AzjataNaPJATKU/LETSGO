using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO;

public class ClientDTO
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Telephone { get; set; }
    [Required]
    public string Pesel { get; set; }
    
}

public class TripDTO
{
    [Required]
    public int IdTrip { get; set; }
    [Required]
    public string TripName { get; set; }
    public string? PaymentDate { get; set; } = null;
}