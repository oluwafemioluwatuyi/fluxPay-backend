using fluxPay.DTOs;
using fluxPay.Helpers;

namespace fluxPay.Interfaces.Services
{
    public interface IWalletTransferService
    {
        Task<ServiceResponse<string>> TransferToWallet(TransferToWalletRequestDto transferToWalletRequestDto);
        Task<ServiceResponse<string>> TransferToAgent(TransferToWalletRequestDto transferToWalletRequestDto);
        //Task<ServiceResponse<string>> TransferToBank(TransferToBankRequestDto transferToBankRequestDto);

    }
}