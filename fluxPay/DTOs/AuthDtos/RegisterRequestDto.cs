using System.ComponentModel.DataAnnotations;

namespace fluxPay.DTOs.AuthDtos
{
    public class RegisterRequestDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; } 
        [Required]
        public string PhoneNumber { get; set; } 
        [Required]
        public string Password { get; set; } 
        [Required]
        public string ConfirmPassword { get; set; } 
        [Required]
        public string BVN { get; set; } 
        [Required]
        public VerifyPassportDto Passport { get; set; } 
        [Required]
        public string DateOfBirth { get; set; } 
         [Required]
        public AccountType accountType {get; set;}
 
        public string ReferralCode { get; set; } 
    }
}
