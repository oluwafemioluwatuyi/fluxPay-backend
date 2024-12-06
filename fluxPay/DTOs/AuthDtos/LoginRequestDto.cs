using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
    public interface LoginRequestDto
    {
        [Required]
        public string Username { get; set; }  
        [Required]
        public string Password { get; set; }
    }
}
