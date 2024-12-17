using System;
using System.Collections.Generic;


namespace fluxPay.DTOs;
public class otpResponse
{
    public string Token { get; set; }
    public long ValidFrom { get; set; }
    public long ValidTo { get; set; }
  
}

  