using System;
using System.Collections.Generic;

namespace FluxPay.Models;

public class TempUser
{
   public Guid Id { get; set; }
    public string Email { get; set; }
    public string? DateOfBirth {get; set;}
    public string LastName {get;set;}
    public string FirstName {get; set;}
    public string? PhoneNumber { get; set; }

    public string? VerificationToken {get; set;}

    public bool? IsTokenVerified {get; set;}

    public bool? IsEmailVerified {get; set;}
    public string? Passport {get; set;}
    public string? Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
