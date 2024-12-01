using System;
using System.Collections.Generic;

namespace FluxPay.Models;

public class User : AuditableEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; } // For hashed passwords
    public bool IsSelfServiceUser { get; set; }
    public bool SendPasswordToEmail { get; set; }
    public ICollection<Role> Roles { get; set; }
    public int OfficeId { get; set; } = 1; // Default value
}
