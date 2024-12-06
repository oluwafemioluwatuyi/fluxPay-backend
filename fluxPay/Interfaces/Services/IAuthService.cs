using fluxPay.DTOs.AuthDtos;

namespace fluxPay.Interfaces.Services
{
    public interface IAuthService
    {
        Task<object> Login(LoginRequestDto loginRequestDto);
        Task<object> Register(RegisterRequestDto registerRequestDto);
        Task<object> VerifyEmail(VerifyEmailRequestDto verifyEmailRequestDto);
        Task<object> ResendEmailVerification(RegisterRequestDto registerRequestDto);
        Task<object> ForgotPassword(ForgotPasswordRequestDto forgotPasswordRequestDto);
        Task<object> ChangePassword(ResetPasswordRequestDto resetPasswordRequestDto);
        Task<object> CreatePin(CreatePinRequestDto createPinRequestDto);
        Task<object> ChangePin(ResetPinRequestDto resetPinRequestDto);
        Task<object> ForgotPin(ForgotPinRequestDto forgotPinRequestDto);
        Task<object> EnableTwoFactorAuthentication(EnableTwoFactorAuthenticationRequestDto enableTwoFactorAuthenticationRequestDto);
        Task<object> DisableTwoFactorAuthentication();
        Task<object> ValidateBiometricAuthentication(BiometricAuthenticationRequestDto biometricAuthenticationRequestDto);             
    }
}
