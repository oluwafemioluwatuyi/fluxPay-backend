using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using FluxPay.Models;
using fluxPay.Interfaces.Services;

namespace FluxPay.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly IWalletTransferService _walletTransferService;
        private readonly ILogger<TransferController> _logger;

        public TransferController(IWalletTransferService walletTransferService, ILogger<TransferController> logger)
        {
            _walletTransferService = walletTransferService;
            _logger = logger;
        }

        [HttpPost("wallet-transfer")]
        public async Task<IActionResult> WalletTransfer([FromBody] TransferToWalletRequestDto transferToWalletRequestDto)
        {
            var response = await _walletTransferService.TransferToWallet(transferToWalletRequestDto);
            if (response is null)
            {
                return NotFound();
            }
            {
                return Ok(response);
            }
        }
    }
}