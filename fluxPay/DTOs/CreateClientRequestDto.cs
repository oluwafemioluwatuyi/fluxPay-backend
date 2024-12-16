using System;
using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs;

public class CreateClientRequestDto
{
    public string Email { get; set; }
    public string FirstName { get; set; }
     public string LastName { get; set; }
    public string Password { get; set; }

}