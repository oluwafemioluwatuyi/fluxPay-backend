using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
    public class VerifyEmailRequestDto
    {
        public string? Email { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
