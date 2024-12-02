using fluxPay.DTOs;
using fluxPay.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace fluxPay.Services
{
    public class AuthService
    {
        private readonly IFineractApiService _fineractApiService;
        private readonly IConfiguration _configuration;

        public AuthService(IFineractApiService fineractApiService, IConfiguration configuration)
        {
            _fineractApiService = fineractApiService;
            _configuration = configuration;
        }

        // Register client method
        public async Task<string> RegisterClientAsync(CreateClientRequestDto createClientRequestDto)
        {
            // Here you can do any preprocessing or validation if needed
            // For example, check if the client already exists or other validations

            try
            {
                // Create the client via the FineractApiService
                var clientId = await _fineractApiService.CreateClientAsync(createClientRequestDto);
                return clientId;
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur during client registration
                throw new Exception($"An error occurred while registering the client: {ex.Message}");
            }
        }

        // Get client method by clientId
        public async Task<string> GetClientAsync(int clientId)
        {
            try
            {
                var client = await _fineractApiService.GetClientAsync(clientId);
                return client;
            }
            catch (Exception ex)
            {
                // Handle any errors that might occur while fetching client details
                throw new Exception($"An error occurred while retrieving the client: {ex.Message}");
            }
        }
    }
}
