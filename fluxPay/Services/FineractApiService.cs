using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using fluxPay.Clients;
using fluxPay.DTOs; 
using fluxPay.Interfaces.Services;
using FluxPay.Models;
using fluxPay.Helpers;
using System.Net.Http.Headers;
using fluxPay.DTOs.AuthDtos;

namespace fluxPay.Services
{
    public class FineractApiService : IFineractApiService
    {
        private readonly FineractClient _fineractClient;


        public FineractApiService(FineractClient fineractClient)
        {
            _fineractClient = fineractClient;
        }

        public async Task<AccountNumberFormatDto[]> ConfigureAccountNumber(AccountNumberFormatDto accountNumberFormat)
        {
     try
        {
        var requestUrl = $"/fineract-provider/api/v1/accountnumberformats";
        var response = await _fineractClient._client.GetAsync(requestUrl);
      if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            // Deserialize the response content into a list of AccountNumberFormatDto objects
            var accountNumberFormats = JsonConvert.DeserializeObject<List<AccountNumberFormatDto>>(content);
            return accountNumberFormats.ToArray();
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error: {errorMessage}");
        }
    }
       catch (Exception ex)
    {
        // Handle the exception (optional logging)
        throw new Exception($"An exception occurred: {ex.Message}");
    }
    }


        public async Task<FineractApiResponse> CreateAccountNumber(AccountNumberFormatDto accountNumberFormat, AccountTypeDto accountType, int clientId, int productId, DateTime submittedOnDate)
        {
            if (accountNumberFormat == null)
        {
            throw new ArgumentNullException(nameof(accountNumberFormat), "Account number format cannot be null.");
        }

            // Step 2: Prepare Payload
            var createAccountRequest = new
            {
                clientId = 3, // Replace with the actual client ID
                productId = 1, // Replace with the actual product ID
                locale = "en", // Assuming English locale
                dateFormat = "dd MMMM yyyy", 
                submittedOnDate = DateTime.UtcNow.ToString("dd MMMM yyyy") // Format the date as required
            };

            // Step 3: Make API Call
            var response = await _fineractClient._client.PostAsJsonAsync("/fineract-provider/api/v1/savingsaccounts", createAccountRequest);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Status Code: {response.StatusCode}, Response: {errorContent}");

                return new FineractApiResponse
                {
                    Success = false,
                    Message = $"Failed to create account in Fineract. Status: {response.StatusCode}, Details: {errorContent}"
                };
            }

            return new FineractApiResponse
        {
            Success = true,
            Message = "Account created successfully."
        };
            
        }

        public async Task<FineractApiResponse> CreateClientAsync(CreateClientRequestDto createClientRequestDto)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };

            var jsonContent = System.Text.Json.JsonSerializer.Serialize(createClientRequestDto, options);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _fineractClient.PostAsync("/clients", content);

            if (response.IsSuccessStatusCode)
            {
                var successContent = await response.Content.ReadAsStringAsync();
                return new FineractApiResponse
                {
                     Success = true,
                     Message = "Account number configured successfully.",
                    Data = successContent
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error creating client: {response.StatusCode}, {errorContent}");
            }
        }

        public Task<FineractApiResponse> CreateSavingAccount(CreateSavingsAccountRequestDto createSavingsAccountRequestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<FineractApiResponse> GetClientAsync(int clientId)
        {
            var requestUri = $"/fineract-provider/api/v1/clients/{clientId}";

            Console.WriteLine("Request URI: " + requestUri);
            Console.WriteLine("Authorization Header: " + _fineractClient._client.DefaultRequestHeaders.Authorization);

            var response = await _fineractClient._client.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var successContent = await response.Content.ReadAsStringAsync();
                  return new FineractApiResponse
                {
                     Success = true,
                     Message = "Account number configured successfully.",
                    Data = successContent
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error retrieving client: {response.StatusCode}, {errorContent}");
            }
        }

        public async Task<OtpConfigDto> GetOtpConfigure()
        {
             var requestUrl = $"/fineract-provider/api/v1/twofactor/configure";
            var response = await _fineractClient._client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var settings = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(content);

                var otpConfig = new OtpConfigDto
                {
                    OtpTokenLength = int.Parse(settings.FirstOrDefault(kvp => kvp.Key == "otp-token-length").Value ?? "6"),
                    OtpTokenExpiryTime = int.Parse(settings.FirstOrDefault(kvp => kvp.Key == "otp-token-live-time").Value ?? "300") ,
                    OtpSubjectTemplate = settings.FirstOrDefault(kvp => kvp.Key == "otp-delivery-email-subject").Value ?? "Your OTP Code", // Default subject
                   OtpBodyTemplate = settings.FirstOrDefault(kvp => kvp.Key == "otp-delivery-email-body").Value ?? "Your OTP code is {{token}}. It will expire in {{expiryTime}} minutes." // Default body
                };

                return otpConfig;
            }
            else
            {
                throw new Exception("Failed to retrieve OTP configuration from Fineract.");
            }
        }

        public async Task<SmtpSettings> GetSmtpAsync()
        {
            var requestUrl = $"/fineract-provider/api/v1/externalservice/SMTP";
            var response = await  _fineractClient._client.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

        var settings = await response.Content.ReadFromJsonAsync<List<KeyValuePair<string, string>>>();

                foreach (var kvp in settings)
            {
                Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
            }

        var smtpSettings = new SmtpSettings
        {
            Host = settings.FirstOrDefault(kvp => kvp.Key == "host").Value,
            Port = int.Parse(settings.FirstOrDefault(kvp => kvp.Key == "port").Value ?? "25"),
            UseTLS = bool.Parse(settings.FirstOrDefault(kvp => kvp.Key == "useTLS").Value ?? "false"),
            Username = settings.FirstOrDefault(kvp => kvp.Key == "username").Value,
            Password = settings.FirstOrDefault(kvp => kvp.Key == "password").Value,
            FromEmail = settings.FirstOrDefault(kvp => kvp.Key == "fromEmail").Value,
            FromName = settings.FirstOrDefault(kvp => kvp.Key == "fromName").Value
        };

         return smtpSettings;
 
            
        }

}
    
}


public enum AccountType
{
    Agent_account,
    Paystore_account,
    Merchant_account,
    Customer_account
    // Add other account types as needed
}


public class AccountNumberFormatDto
{
    public int Id { get; set; }
    public AccountTypeDto AccountType { get; set; }
    public PrefixTypeDto PrefixType { get; set; }
    public string PrefixCharacter { get; set; }
}

public class AccountTypeDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Value { get; set; }
}

public class PrefixTypeDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Value { get; set; }
}







