using System.Diagnostics.Contracts;
using fluxPay.DTOs.AuthDtos;
using fluxPay.Helpers;
using fluxPay.Services;

namespace fluxPay.Interfaces.Services
{
    public interface IWalletTransfer
    {
        Task<FineractApiResponse> PerformWalletTransfer(WalletTransferRequestDto walletTransferRequestDto);
        Task<FineractApiResponse> ExecuteWalletTransfer(WalletTransferRequestDto walletTransferRequestDto, WebhookEventPayload payload);
        Task<FineractApiResponse> ProcessWebhookPayloadAsync(WebhookEventPayload payload);
    }

}
