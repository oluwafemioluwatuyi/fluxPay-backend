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

        // Step 1: Generate Account Number
        string accountNumber = GenerateAccountNumber(accountNumberFormat, accountType);

            // Step 2: Prepare Payload
            var createAccountRequest = new
            {
                clientId = 1, // Replace with the actual client ID
                productId = 1, // Replace with the actual product ID
                locale = "en", // Assuming English locale
                dateFormat = "dd MMMM yyyy", // Ensure the correct date format is used
                submittedOnDate = DateTime.UtcNow.ToString("dd MMMM yyyy") // Format the date as required
            };

            _fineractClient._client.DefaultRequestHeaders.Accept.Add(
              new MediaTypeWithQualityHeaderValue("application/json")
);

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


            private string GenerateAccountNumber(AccountNumberFormatDto accountNumberFormat, AccountTypeDto accountType)
        {
                // Default prefix if none provided
            string prefix = accountNumberFormat.PrefixType?.Value ?? "00";

            // Use the account type value to determine the account type code
            string accountTypeCode = accountType.Value switch
            {
                "CLIENT" => "01",
                "LOAN" => "02",
                _ => "99" // Default for unknown types
            };

            // Generate a unique identifier (e.g., timestamp-based or GUID)
            string uniqueId = DateTime.UtcNow.Ticks.ToString().Substring(0, 10); // First 10 characters of ticks

            // Combine prefix, account type code, and unique ID
            return $"{prefix}{accountTypeCode}{uniqueId}";
                }

}
    
}


public enum AccountType
{
    Savings,
    Deposit,
    Loan
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



