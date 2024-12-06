namespace fluxPay.DTOs.AuthDtos
{
    public interface EnableTwoFactorAuthenticationRequestDto
    {
        public string AuthenticationMethod { get; set; } 
        public string VerificationCode { get; set; }
    }
}
