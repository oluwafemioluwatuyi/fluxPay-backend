using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
    public class ResendEmailVerificationDto
    {
       
        [Required]
        public string Email { get; set; } 
      
    }
}
