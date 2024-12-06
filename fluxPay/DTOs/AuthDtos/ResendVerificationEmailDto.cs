using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
    public interface ResendEmailVerification
    {
     
        [Required]
        public string Email { get; set; }
    }
}
