using System.Text;
using fluxPay.Clients;
using fluxPay.Helpers;
using fluxPay.Interfaces.Services;
using Newtonsoft.Json;

namespace fluxPay.Services
{
    public class WalletTransfer : IWalletTransfer
    {

        private readonly FineractClient _fineractClient;
        private readonly IEmailService _emailService;
        private readonly ILogger<WalletTransfer> _logger;
        public WalletTransfer(FineractClient fineractClient, IEmailService emailService, ILogger<WalletTransfer> logger)
        {
            _fineractClient = fineractClient;
            _emailService = emailService;
            _logger = logger;

        }
        public async Task<FineractApiResponse> PerformWalletTransfer(WalletTransferRequestDto walletTransferRequestDto)
        {
            // Implement the logic to perform the wallet transfer
            // This is a placeholder implementation and should be replaced with actual transfer logic

            // Example:
            var transferPayload = new WalletTransferRequestDto
            {
                locale = walletTransferRequestDto.locale,
                dateFormat = walletTransferRequestDto.dateFormat,
                transactionDate = walletTransferRequestDto.transactionDate,
                transactionAmount = walletTransferRequestDto.transactionAmount,
                paymentTypeId = walletTransferRequestDto.paymentTypeId,
                accountNumber = walletTransferRequestDto.accountNumber,
                checkNumber = walletTransferRequestDto.checkNumber,
                routingCode = walletTransferRequestDto.routingCode,
                receiptNumber = walletTransferRequestDto.receiptNumber,
                bankNumber = walletTransferRequestDto.bankNumber
            };

            // Use JsonContent.Create to create the request content
            var requestContent = JsonContent.Create(transferPayload);

            // Log the JSON payload
            _logger.LogInformation("Sending JSON payload: {JsonPayload}", JsonConvert.SerializeObject(transferPayload));

            var response = await _fineractClient._client.PostAsync("/fineract-provider/api/v1/savingsaccounts/1/transactions?command=deposit", requestContent);

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response Status Code: {StatusCode}", response.StatusCode);
            _logger.LogInformation("Response Content: {ResponseContent}", responseContent);

            if (response.IsSuccessStatusCode)
            {
                return new FineractApiResponse
                {
                    Success = true,
                    Message = "Wallet transfer successful."
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Failed to perform wallet transfer. Status Code: {response.StatusCode}, Error: {errorContent}");
            }
            return new FineractApiResponse
            {
                Success = false,
                Message = "perform wallet trasaction."
            };
        }

        public async Task<FineractApiResponse> ExecuteWalletTransfer(WalletTransferRequestDto walletTransferRequestDto, WebhookEventPayload payload)
        {
            // Step 1: Perform the wallet transfer
            var transferResult = await PerformWalletTransfer(walletTransferRequestDto);

            if (transferResult.Success.GetValueOrDefault())
            {
                // Step 2: Notify Fineract about the transfer event
                var fineractEventPayload = new WebhookEventPayload
                {
                    actionName = "TRANSFER",
                    entityName = "WALLET",
                    // referenceId = walletTransferRequestDto.TransactionId, // Optional, for tracking purposes
                    metadata = new WalletTransferRequestDto
                    {
                        locale = walletTransferRequestDto.locale,
                        dateFormat = walletTransferRequestDto.dateFormat,
                        transactionDate = walletTransferRequestDto.transactionDate,
                        transactionAmount = walletTransferRequestDto.transactionAmount,
                        paymentTypeId = walletTransferRequestDto.paymentTypeId,
                        accountNumber = walletTransferRequestDto.accountNumber,
                        checkNumber = walletTransferRequestDto.checkNumber,
                        routingCode = walletTransferRequestDto.routingCode,
                        receiptNumber = walletTransferRequestDto.receiptNumber,
                        bankNumber = walletTransferRequestDto.bankNumber
                    }
                };
                _logger.LogInformation("Wallet transfer successful. Proceeding to notify Fineract...");
                await ProcessWebhookPayloadAsync(fineractEventPayload);

                return new FineractApiResponse
                {
                    Success = true,
                    Message = "Wallet transfer and internal event notification successful."
                };
            }
            else
            {
                _logger.LogError("Wallet transfer failed: {ErrorMessage}", transferResult.Message);
                // If transfer fails, return failure response
                return new FineractApiResponse
                {
                    Success = false,
                    Message = "Wallet transfer failed."
                };
            }
        }
        private async Task SendTransferNotificationAsync(dynamic metadata)
        {
            // Extract metadata information
            var PaymentTypeId = metadata.paymentTypeId;
            var RoutingCode = metadata.routingCode;
            var Amount = metadata.transactionAmount;

            _logger.LogInformation("Sending notification for transfer from {SourceWalletId} to {DestinationWalletId} of amount {Amount}", (int)PaymentTypeId, (string)RoutingCode, (decimal)Amount);

            // Prepare email template data
            var emailTemplateData = new
            {
                paymentTypeId = PaymentTypeId,
                routingCode = RoutingCode,
                amount = Amount
            };
            string recipientEmail = "recipient@example.com";

            // Send email notification using the email service
            await _emailService.SendOtpEmailAsync(recipientEmail, "Transfer Notification", "WalletTransfer", emailTemplateData);
        }

        public async Task<FineractApiResponse> ProcessWebhookPayloadAsync(WebhookEventPayload payload)
        {
            // Extract relevant information from the payload
            var actionName = payload.actionName;
            var entityName = payload.entityName;
            var metadata = payload.metadata;

            // Perform necessary actions based on the payload
            if (actionName == "TRANSFER" && entityName == "WALLET")
            {
                _logger.LogInformation("Webhook payload: {Payload}", JsonConvert.SerializeObject(payload));
                // Send the webhook payload to the configured URL 
                var webhookResponse = await SendWebhookPayloadAsync(payload);
                if (webhookResponse.IsSuccessStatusCode)
                {
                    await SendTransferNotificationAsync(metadata);
                    return new FineractApiResponse
                    { Success = true, Message = "Wallet transfer and event notification successful." };
                }
                else
                {
                    var errorContent = await webhookResponse.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to send webhook payload. Status Code: {StatusCode}, Error: {ErrorContent}",
                    webhookResponse.StatusCode, errorContent);
                    return new FineractApiResponse
                    { Success = false, Message = "Failed to send webhook payload." };
                }
            }

            return new FineractApiResponse
            {
                Success = false,
                Message = "Process webhook."
            };
        }

        private async Task<HttpResponseMessage> SendWebhookPayloadAsync(WebhookEventPayload payload)
        {
            // Log the JSON payload
            _logger.LogInformation("Sending webhook payload to URL: {PayloadUrl}", "http://wallettransfer.com/api/webhook/");
            _logger.LogInformation("Webhook payload: {JsonPayload}", JsonConvert.SerializeObject(payload));

            // Send the webhook payload to the configured URL using PostAsJsonAsync
            var response = await _fineractClient._client.PostAsJsonAsync("http://wallettransfer.com/api/webhook/", payload);

            return response;
        }

    }
}


public class WalletTransferRequestDto
{
    public string locale { get; set; }               // Default to "en" as shown in the example
    public string dateFormat { get; set; }  // Default format
    public string transactionDate { get; set; }               // "transactionDate" format: "27 May 2013"
    public decimal transactionAmount { get; set; }             // Ensure this is a string if it should be sent as a string
    public int paymentTypeId { get; set; }
    public string accountNumber { get; set; }
    public string checkNumber { get; set; }
    public string routingCode { get; set; }
    public string receiptNumber { get; set; }
    public string bankNumber { get; set; }


}

public class WebhookEventPayload
{
    public string actionName { get; set; }
    public string entityName { get; set; }
    public dynamic metadata { get; set; }
}