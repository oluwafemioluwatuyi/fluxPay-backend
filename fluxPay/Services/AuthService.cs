using AutoMapper;
using fluxPay.Constants;
using fluxPay.DTOs;
using fluxPay.DTOs.AuthDtos;
using fluxPay.Helpers;
using fluxPay.Interfaces.Repositories;
using fluxPay.Interfaces.Services;
using FluxPay.Models;
using FluxPay.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace fluxPay.Services
{
    public class AuthService : IAuthService
    {
        private readonly IFineractApiService _fineractApiService;
        private readonly ITempUserRepository tempUserRepository;

        private readonly IEmailService _emailService;
        private readonly IMapper mapper;

        private IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IClientService _clientService;
        private readonly OtpService _otpService;
        private readonly Dictionary<string, OtpDto> _otpCache = new();
        private readonly IOtpService _otpService1;

        public AuthService(IFineractApiService fineractApiService, ITempUserRepository tempUserRepository, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IEmailService emailService, IClientService clientService, OtpService otpService)
        {
            _fineractApiService = fineractApiService;
            _configuration = configuration;
            this.tempUserRepository = tempUserRepository;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _clientService = clientService;
            _otpService = otpService;
           // _otpService1 = otpService1;

        }
       
        public async Task<object> Login(LoginRequestDto loginRequestDto)
        {
            // Step 1: Check username and password
        var user = await _clientService.FindByEmailAsync(loginRequestDto.Username);
        if (user == null)
        {
            return "Invalid username or password";
        }
        // Step 2: Send OTP (if 2FA is enabled)
        //   var otpSent = await _otpService1.RequestOtpAsync(loginRequestDto.Email);
        // if (!otpSent)
        // {
        //     return "Failed to send OTP. Please try again.";
        // }

        // // Step 3: Validate OTP
        // var isOtpValid = await _otpService1.ValidateOtpEmailAsync(loginRequestDto.Email, loginRequestDto.Otp);
        // if (!isOtpValid)
        // {
        //     return "Invalid OTP. Please try again.";
        // }
         return new ServiceResponse<string>(
                        ResponseStatus.Error,
                        AppStatusCodes. EmailNotVerified,
                        "Invalid request.",
                        null);
        }
        public async Task<ServiceResponse<string>> Register(RegisterRequestDto registerRequestDto)
        {
                        if (registerRequestDto == null)
            {
                return new ServiceResponse<string>(
                    ResponseStatus.Error,
                    AppStatusCodes.EmailNotVerified,
                    "Invalid request.",
                    null);
            }

            // Create a temporary user model for data validation
            var tempUser = new TempUser
            {
                Email = registerRequestDto.Email,
                PhoneNumber = registerRequestDto.PhoneNumber,
                FirstName = registerRequestDto.FirstName,
                LastName = registerRequestDto.LastName,
                DateOfBirth = registerRequestDto.DateOfBirth
            };

            // Step 1: Email validation
            var emailExists = await _clientService.FindByEmailAsync(tempUser.Email); // Check if email already exists
            if (emailExists != null)
            {
                return new ServiceResponse<string>(
                    ResponseStatus.Error,
                    AppStatusCodes.EmailNotVerified,
                    "Email already registered.",
                    null);
            }

            // Step 2: Phone Number Verification
            var phoneExists = await _clientService.FindByPhoneNumberAsync(tempUser.PhoneNumber); // Check if phone already exists
            if (phoneExists != null)
            {
                return new ServiceResponse<string>(
                    ResponseStatus.Error,
                    AppStatusCodes.PhoneAlreadyExists,
                    "The phone number is already registered.",
                    null);
            }

            // Step 3: Passport Verification (Conditional)
            if (tempUser.Passport != null)  // Ensure Passport is provided before verification
            {
                var (isPassportValid, passportMessage) = await VerifyPassport(new VerifyPassportDto
                {
                    PassportNumber = tempUser.Passport,
                    Name = $"{tempUser.FirstName} {tempUser.LastName}",
                    DateOfBirth = tempUser.DateOfBirth
                });

                if (!isPassportValid)
                {
                    return new ServiceResponse<string>(
                        ResponseStatus.Error,
                        AppStatusCodes.ValidationError,
                        passportMessage,
                        null);
                }
            }

            // Step 4: Face Recognition (if required)
            // Implement Face Recognition here if needed

            // Step 5: Sending OTP to user's email
            var generatedToken = await SendOtpEmail(tempUser.Email);
            if (generatedToken == null)
            {
                return new ServiceResponse<string>(
                    ResponseStatus.Error,
                    AppStatusCodes.EmailNotVerified,
                    "Error sending OTP.",
                    null);
            }

            return new ServiceResponse<string>(
                ResponseStatus.Success,
                AppStatusCodes.EmailNotVerified,
                "Token sent successfully.",
                null);   
}

         public Task<ServiceResponse<string>> VerifyAndCreateUserProfile(string token, string email, string phoneNumber, RegisterRequestDto registerRequestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<object> VerifyEmail(VerifyEmailRequestDto verifyEmailRequestDto)
        {
            // Validate the input
                if (string.IsNullOrEmpty(verifyEmailRequestDto.Email) || string.IsNullOrEmpty(verifyEmailRequestDto.Token))
            {
                return new ServiceResponse<string>(
                    ResponseStatus.Error,
                    AppStatusCodes.ValidationError,
                    "Email and verification token are required.",
                    null);
            }
            // Step 1: Find the user by email (temporary check)
        var tempUser = await tempUserRepository.GetUserByEmail(verifyEmailRequestDto.Email);  // Query your tempUser store/database
        if (tempUser is null)
        {
            return new ServiceResponse<string>(
                ResponseStatus.Error,
                AppStatusCodes.InvalidData,
                "User not found with the provided email.",
                null);
        }
        // Step 3: Verify email
            //Call the service method to validate the OTP
            var isOtpValid = await _otpService1.ValidateOtpEmailAsync(verifyEmailRequestDto.Email, verifyEmailRequestDto.Token);

            if (!isOtpValid)
            {
                 return new ServiceResponse<string>(
                            ResponseStatus.BadRequest,
                            AppStatusCodes.InvalidData,
                            "Token incorrect.",
                            null);
            }
              tempUser.IsEmailVerified = true;  // Mark the email as verified
              tempUser.VerificationToken = null;
             // Step 4: Update the user in your database (or temporary store)
        var updateResult = await tempUserRepository.SaveChangesAsync(tempUser);  // Update the user object in your store
            if (!updateResult)
            {
                return new ServiceResponse<string>(
                    ResponseStatus.Error,
                    AppStatusCodes.InvalidData,
                    "Error updating user email verification status.",
                    null);
            }

        return new ServiceResponse<string>(
            ResponseStatus.Success,
            AppStatusCodes.Success,
            "Email successfully verified.",
            null);
      
    }

       public async Task<ServiceResponse<string>> FinializeRegister(RegisterRequestDto registerRequestDto)
        {
             // Step 1: Retrieve the temporary user data (after email verification)
            var tempUser = await _clientService.FindByEmailAsync(registerRequestDto.Email);
            if (tempUser == null)
            {
                return new ServiceResponse<string>(
                    ResponseStatus.Error,
                    AppStatusCodes.InvalidData,
                    "User not found.",
                    null);
            }

            // Step 2: Set the password for the user (after email verification)
            var hashedPassword = HashPassword(registerRequestDto.Password); // Assuming you have a password hashing function
            tempUser.Password = hashedPassword;
            // Step 3: Prepare the CreateClientRequestDto and call the Fineract API to create the client
            var createClientRequestDto = new CreateClientRequestDto
            {
                Email = tempUser.Email,
                FirstName = tempUser.FirstName,
                LastName = tempUser.LastName,
                Password = tempUser.Password,
                // Add any other fields required by Fineract
            };

            var createClient= await _fineractApiService.CreateClientAsync(createClientRequestDto);
            if (createClient is null)
            {
                return new ServiceResponse<string>(
                    ResponseStatus.Error,
                    AppStatusCodes.InvalidData,
                    "Error creating client account.",
                    null);
            }
                // Step 4: Determine Savings Product ID based on Account Type
            int savingsProductId = registerRequestDto.accountType switch
            {
                AccountType.Customer_account => 1,
                AccountType.Agent_account => 2,   // Agent account has productId 4
                AccountType.Merchant_account => 3, // Merchant account has productId 5
                AccountType.Paystore_account=> 4, // Corporate account has productId 6
                _ => 1 // Default productId for Standard account
            };

            // Step 5: Create Savings Account based on Account Type
            var createSavingsAccountRequest = new CreateSavingsAccountRequestDto
            {
                clientId = tempUser.Id, // Assuming tempUser has an Id property after client creation
                productId = savingsProductId,
                Locale = "en",
                DateFormat = "dd MMMM yyyy",
                SubmittedOnDate = "01 March 2011" // Example, use actual registration date
            };

            var createSavingAccountResponse = await _fineractApiService.CreateSavingAccount(createSavingsAccountRequest);
            if (createSavingAccountResponse is null)
            {
                return new ServiceResponse<string>(
                    ResponseStatus.Error,
                    AppStatusCodes.InvalidData,
                    "Error creating saving account.",
                    null);
            }

            return new ServiceResponse<string>(
                ResponseStatus.Success,
                AppStatusCodes.Success,
                "Registration complete and accounts created successfully.",
                null);
        }
        public async Task<object> ResendEmailVerification(ResendEmailVerificationDto resendEmailVerificationDto)
        {
             // Step 1: Check if user exists by email
        var user = await _clientService.FindByEmailAsync(resendEmailVerificationDto.Email);
        if (user == null)
        {
            return new ServiceResponse<string>(
                ResponseStatus.Error,
                AppStatusCodes.InvalidData,
                "User with the provided email does not exist.",
                null);
        }

        // Step 2: Check if email is already verified
        // if (user.EmailVerified)
        // {
        //     return new ServiceResponse<string>(
        //         ResponseStatus.Error,
        //         AppStatusCodes.EmailNotVerified,
        //         "Email has already been verified.",
        //         null);
        // }

        // Step 3: Generate verification token (this can be a unique token generation logic)
        //   var generatedToken = await _otpService1.RequestOtpAsync(resendEmailVerificationDto.Email);
        //      if(generatedToken is false)
        //     return new ServiceResponse<string>(
        //                     ResponseStatus.Error,
        //                     AppStatusCodes. EmailNotVerified,
        //                     "Invalid request.",
        //                     null);

        // Step 5: Return success response
        return new ServiceResponse<string>(
            ResponseStatus.Success,
            AppStatusCodes.Success,
            "Verification email sent successfully.",
            null);
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

        
        private async Task SendVerificationMail(TempUser user, string emailVerificationToken)

                
        {
            try
                {
                    await _emailService.SendEmailAsync(user.Email, "Verify your email", "VerifyEmail", new
                    {
                        FrontendBaseUrl = "http://example.com", // Replace with your actual frontend URL when ready
                        EmailVerificationToken = emailVerificationToken,
                        UserId = user.Id,
                        Email = user.Email
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending verification email to {user.Email}: {ex.Message}");
                }
         }
    
              
            public string GenerateEmailVerificationToken(TempUser user)          
         {
             
             List<Claim> claims = new List<Claim>{
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            // Read secret key, issuer, and audience directly from the configuration
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];

            // Creating a new SymmetricKey from the secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            // Declaring signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Creating new JWT object
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Token expires in 1 day
                signingCredentials: creds
            );

            // Write JWT to a string
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
         }

        private string HashPassword(string password)
        {

            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        private bool VerifyPassword(string password, string passwordHash)
        {

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
         
            private async Task<(bool, string)> VerifyPassport(VerifyPassportDto verifyPassportDto)
        {
            var message = "Passport verified successfully";

            // Simulate async operation
            await Task.Delay(0);

            // TODO: Re-enable this when passport verification integration starts working
            // try
            // {
            //     var response = await _passportVerificationService.VerifyPassportAsync(
            //         mapper.Map<VerifyPassportDto, PassportVerificationRequestDto>(verifyPassportDto));

            //     // Check name match
            //     if (response.ResponseBody.Name.MatchPercentage < 80)
            //     {
            //         message = "Name does not sufficiently match passport details.";
            //         return (false, message);
            //     }

            //     // Check date of birth
            //     if (!response.ResponseBody.DateOfBirth.Equals(verifyPassportDto.DateOfBirth, StringComparison.OrdinalIgnoreCase))
            //     {
            //         message = "Date of birth does not match passport details.";
            //         return (false, message);
            //     }

            //     // Check passport number
            //     if (!response.ResponseBody.PassportNumber.Equals(verifyPassportDto.PassportNumber, StringComparison.OrdinalIgnoreCase))
            //     {
            //         message = "Passport number does not match provided details.";
            //         return (false, message);
            //     }
            // }
            // catch (Exception e)
            // {
            //     var parts = e.Message.Split(':');

            //     if (parts.Length > 1 && parts[0] == "99")
            //     {
            //         message = parts[1];
            //     }
            //     else
            //     {
            //         message = "Unable to verify passport details.";
            //     }

            //     return (false, message);
            // }

            return (true, message);
        }

        public async Task<FineractApiResponse> SendOtpEmail(string email)
        {
     try
    {
        // Step 1: Fetch OTP configuration from the Fineract database
        var otpConfig = await _clientService.GetOtpConfigureFromDb();

        // Step 2: Generate OTP using the length from the configuration
        var otpCode = _otpService.GenerateOtpCode(otpConfig.OtpTokenLength);

        // Step 3: Cache OTP with expiry time
        var expiryDate = DateTime.UtcNow.AddSeconds(otpConfig.OtpTokenExpiryTime);

        var otpDto = new OtpDto
        {
            Code = otpCode,
            ExpirationTime = expiryDate,
            IsValid = true,
            OtpType = "Login",
            GeneratedAt = DateTime.UtcNow
        };

        // Assuming _otpCache is implemented properly (e.g., IMemoryCache or a distributed cache)
        _otpCache[email] = otpDto;

        // Step 4: Prepare the email content
        var subject = otpConfig.OtpSubjectTemplate;
        var body = otpConfig.OtpBodyTemplate
            .Replace("{{token}}", otpCode)
            .Replace("{{expiryTime}}", (otpConfig.OtpTokenExpiryTime / 60).ToString());

        var emailModel = new OtpEmailModel
        {
            Token = otpCode,
            ExpiryTimeInMinutes = otpConfig.OtpTokenExpiryTime / 60
        };

        // Step 5: Send the email
        await _emailService.SendOtpEmailAsync(email, subject, body, emailModel);

        return new FineractApiResponse
        {
            Success = true,
            Message = "OTP sent to email successfully."
        };
    }
    catch (Exception ex)
    {
        // Log exception for troubleshooting (if logging is implemented)
        // _logger.LogError(ex, "Error while sending OTP email");

        return new FineractApiResponse
        {
            Success = false,
            Message = $"Error generating OTP or sending email: {ex.Message}"
        };
    }

   }

    }
}
