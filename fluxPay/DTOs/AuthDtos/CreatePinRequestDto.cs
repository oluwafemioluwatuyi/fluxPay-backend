using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
    public interface CreatePinRequestDto
    {
        [Required]
        public string Pin {  get; set; }
        [Required]
        public string ConfirmPin { get; set; }
    }
}
