using fluxPay.DTOs.AuthDtos;

namespace fluxPay.Interfaces.Services
{

    public interface IOtpService
    {
        Task<bool> RequestOtpAsync(string email);
        Task<bool> ValidateOtpEmailAsync(string email, string otp);
    }

}
