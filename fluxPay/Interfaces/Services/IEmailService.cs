using fluxPay.DTOs.AuthDtos;

namespace fluxPay.Interfaces.Services
{

    public interface IEmailService
    {
        Task <bool>SendEmailAsync(string toEmail, string subject, string templateFileName, object model);
        Task<bool>SendOtpEmailAsync(string toEmail, string subject, string body, object model);
    }

}
