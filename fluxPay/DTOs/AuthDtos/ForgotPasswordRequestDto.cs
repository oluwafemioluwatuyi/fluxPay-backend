using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
    public interface ForgotPasswordRequestDto
    {
        [Required]
        public string Email { get; set; }
    }
}
