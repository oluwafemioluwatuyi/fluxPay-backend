using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
    public interface ForgotPinRequestDto
    {
        [Required]
        public string Email { get; set; }
    }
}
