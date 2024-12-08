using AutoMapper;
using fluxPay.DTOs;
using fluxPay.DTOs.AuthDtos;
using fluxPay.Interfaces.Repositories;
using fluxPay.Interfaces.Services;
using FluxPay.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace fluxPay.Services
{
    public class AuthService : IAuthService
    {
        private readonly IFineractApiService _fineractApiService;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        private IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration _configuration;

        public AuthService(IFineractApiService fineractApiService, IUserRepository userRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _fineractApiService = fineractApiService;
            _configuration = configuration;
            this.userRepository = userRepository;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;

        }

        // // Register client method
        // public async Task<string> RegisterClientAsync(CreateClientRequestDto createClientRequestDto)
        // {
        //     // Here you can do any preprocessing or validation if needed
        //     // For example, check if the client already exists or other validations

        //     try
        //     {
        //         // Create the client via the FineractApiService
        //         var clientId = await _fineractApiService.CreateClientAsync(createClientRequestDto);
        //         return clientId;
        //     }
        //     catch (Exception ex)
        //     {
        //         // Handle any errors that might occur during client registration
        //         throw new Exception($"An error occurred while registering the client: {ex.Message}");
        //     }
        // }
        // public async Task<string> GetClientAsync(int clientId)
        // {
        //     try
        //     {
        //         var client = await _fineractApiService.GetClientAsync(clientId);
        //         return client;
        //     }
        //     catch (Exception ex)
        //     {
        //         // Handle any errors that might occur while fetching client details
        //         throw new Exception($"An error occurred while retrieving the client: {ex.Message}");
        //     }
        // }
       
        public Task<object> Login(LoginRequestDto loginRequestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<object> Register(RegisterRequestDto registerRequestDto, AccountNumberFormatDto accountNumberFormat, int clientId, int productId, DateTime submittedOnDate)
        {
            // using var transaction = await userRepository.BeginTransactionAsync();

        try
        {
           //var existingUser = await userRepository.GetUserByEmail(registerRequestDto.Email);
           // if (existingUser != null)
           // {
           //     throw new InvalidOperationException("Email already registered.");
           // }

             var accountNumberFormatResponse = await _fineractApiService.ConfigureAccountNumber(accountNumberFormat);
            if (accountNumberFormatResponse is null)
            {
               throw new InvalidOperationException("Email already registered.");
            }

            // 3 Find the relevant account number format based on the account type (assumed logic here)
          

           // var accountNumberFormat = accountNumberFormatResponse;

            // Step 4: Generate Account Number Using Switch Statement
            string accountNumber = string.Empty;
            switch (accountNumberFormat.AccountType.Value)
            {
               case "CLIENT":
          accountNumber = $"{accountNumberFormat.PrefixType.Value}-CLIENT";
            break;

        case "LOAN":
            accountNumber = $"{accountNumberFormat.PrefixType.Value}-LOAN";
            break;

                default:
                   throw new InvalidOperationException("Email already registered.");
            }

          // Step 4: Call Fineract API to create the account with the generated account number
        var createAccountResponse = await _fineractApiService.CreateAccountNumber(accountNumberFormat, registerRequestDto.accountType, clientId, productId, DateTime.Now);
        if (createAccountResponse is null)
        {
            throw new InvalidOperationException("Failed to create account in Fineract.");
        }
           // await transaction.CommitAsync();

            return (new { Success = true, Message = "User registered successfully." });
        }
        catch (Exception ex)
        {
            // If any operation fails, rollback the transaction to revert all changes
           // await transaction.RollbackAsync();
            return (500, new { Success = false, Message = ex.Message });
        }
           
        }

        public Task<object> VerifyEmail(VerifyEmailRequestDto verifyEmailRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<object> ResendEmailVerification(RegisterRequestDto registerRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<object> ForgotPassword(ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<object> ChangePassword(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<object> CreatePin(CreatePinRequestDto createPinRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<object> ChangePin(ResetPinRequestDto resetPinRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<object> ForgotPin(ForgotPinRequestDto forgotPinRequestDto)
        {
            throw new NotImplementedException();
        }
        public Task<object> DisableTwoFactorAuthentication()
         { 
            throw new NotImplementedException(); 
        }

        public Task<object> EnableTwoFactorAuthentication(EnableTwoFactorAuthenticationRequestDto enableTwoFactorAuthenticationRequestDto)
        {
            throw new NotImplementedException();
        }

        public Task<object> ValidateBiometricAuthentication(BiometricAuthenticationRequestDto biometricAuthenticationRequestDto)
        {
            throw new NotImplementedException();
        }
    }
}
