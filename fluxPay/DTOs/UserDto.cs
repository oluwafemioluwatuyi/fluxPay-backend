using System;
using System.Collections.Generic;


namespace fluxPay.DTOs;

public class UserDto
{

   public Guid Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PasswordHash { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  
}

