using System;
using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs;

public class CreateClientRequestDto
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public int OfficeId { get; set; }
    public bool Active { get; set; }
    public string ActivationDate { get; set; }
    public string DateFormat { get; set; }
    public string Locale { get; set; }
    public int LegalFormId { get; set; }
}

