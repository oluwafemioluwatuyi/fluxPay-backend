using System.Text;
using fluxPay.Clients;
using fluxPay.Helpers;
using fluxPay.Interfaces.Services;
using Newtonsoft.Json;
using fluxPay.DTOs;
using fluxPay.Constants;

namespace fluxPay.Services
{
    public class WalletTransferService : IWalletTransferService
    {
        private readonly IFineractApiService _fineractApiService;
        private readonly IEmailService _emailService;
        private readonly ILogger<WalletTransfer> _logger;
        public WalletTransferService(IFineractApiService fineractApiService, ILogger<WalletTransfer> logger)
        {
            _fineractApiService = fineractApiService;
            _logger = logger;

        }
        public Task<ServiceResponse<string>> TransferToAgent(TransferToWalletRequestDto transferToWalletRequestDto)
        {
            throw new NotImplementedException();
        }

        // public Task<ServiceResponse<string>> TransferToBank(TransferToBankRequestDto transferToBankRequestDto)
        // {
        //     throw new NotImplementedException();
        // }

        public async Task<ServiceResponse<string>> TransferToWallet(TransferToWalletRequestDto transferToWalletRequestDto)
        {
            try
            {
                // Check if the source account exists and has sufficient balance
                var sourceAccount = await _fineractApiService.GetAgentWalletAsync(transferToWalletRequestDto.AccountId);
                if (sourceAccount == null)
                {
                    return new ServiceResponse<string>(
                        ResponseStatus.Error,
                        AppStatusCodes.InvalidAccountNumber,
                        "Source account not found.",
                        null
                    );
                }

                if (sourceAccount.Balance < transferToWalletRequestDto.TransactionAmount)
                {
                    return new ServiceResponse<string>(
                        ResponseStatus.Error,
                        AppStatusCodes.InsufficientFunds,
                        "Insufficient balance in source account.",
                        null
                    );
                }

                // Check if the destination wallet exists
                var destinationWallet = await _fineractApiService.GetAgentWalletAsync(transferToWalletRequestDto.DestinationAccountId);
                if (destinationWallet == null)
                {
                    return new ServiceResponse<string>(
                        ResponseStatus.Error,
                        AppStatusCodes.InvalidAccountNumber,
                        "Destination wallet not found.",
                        null
                    );
                }

                // Debit the source account
                await _fineractApiService.DebitAsync(transferToWalletRequestDto.AccountId, new TransferDto
                {
                    Locale = "en",
                    DateFormat = "dd MMMM yyyy",
                    TransactionDate = DateTime.Now.ToString("dd MMMM yyyy"),
                    TransactionAmount = transferToWalletRequestDto.TransactionAmount,
                    PaymentTypeId = transferToWalletRequestDto.PaymentTypeId
                });

                // Credit the destination wallet
                await _fineractApiService.CreditAsync(transferToWalletRequestDto.DestinationAccountId, new TransferDto
                {
                    Locale = "en",
                    DateFormat = "dd MMMM yyyy",
                    TransactionDate = DateTime.Now.ToString("dd MMMM yyyy"),
                    TransactionAmount = transferToWalletRequestDto.TransactionAmount,
                    PaymentTypeId = transferToWalletRequestDto.PaymentTypeId
                });

                // Return a successful response
                return new ServiceResponse<string>(
                    ResponseStatus.Success,
                    AppStatusCodes.Success,
                    "Transfer completed successfully.",
                    "Transfer successful"
                );
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                Console.WriteLine($"Error during transfer: {ex.Message}");

                // Return a failure response with the exception message
                return new ServiceResponse<string>(
                    ResponseStatus.Error,
                    AppStatusCodes.InternalServerError,
                    $"Transfer failed: {ex.Message}",
                    null
                );
            }
        }
    }
}

