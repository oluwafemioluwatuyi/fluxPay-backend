using System;
using System.Collections.Generic;


namespace fluxPay.DTOs;
public class OtpConfigDto
{
   public int OtpTokenLength { get; set; } // Length of the OTP (e.g., 6)
    public int OtpTokenExpiryTime { get; set; } // Expiry time in seconds (e.g., 300)
    public string OtpSubjectTemplate { get; set; } // Email subject template
    public string OtpBodyTemplate { get; set; } 
}

public class OtpDto
{
    public string Code { get; set; } // The OTP code
    public DateTime ExpirationTime { get; set; } // Expiration time of the OTP
    public bool IsValid { get; set; } // Whether the OTP is valid
    public Guid? UserId { get; set; } // The user the OTP is associated with (optional)
    public string OtpType { get; set; } // Type of OTP (optional)
    public DateTime GeneratedAt { get; set; } 
}

 public class OtpEmailModel
    {
        public string Token { get; set; }
        public int ExpiryTimeInMinutes { get; set; }
    }
