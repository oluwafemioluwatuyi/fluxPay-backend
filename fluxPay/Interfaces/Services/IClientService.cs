using fluxPay.DTOs;
using fluxPay.DTOs.AuthDtos;
using fluxPay.Services;

namespace fluxPay.Interfaces.Services
{
    public interface IClientService
    {
        Task<Client> FindByEmailAsync(string mail);
        Task<object> FindByPhoneNumberAsync(string phoneNumber);
        Task<OtpConfigDto> GetOtpConfigureFromDb();
        Task<SmtpSettings>  GetSmtpSettingsFromDb();
          
    }
}
