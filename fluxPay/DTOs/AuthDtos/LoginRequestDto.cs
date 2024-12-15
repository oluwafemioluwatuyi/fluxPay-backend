using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
    public interface LoginRequestDto
    {
        [Required]
        public string Username { get; set; }  
        [Required]
        public string Password { get; set; }
        
        public string Email {get; set;}
        public string Otp {get; set;}
    }
}
