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
        private readonly IWalletTransfer _walletTransfer;
        private readonly ILogger<TransferController> _logger;

        public TransferController(IWalletTransfer walletTransfer, ILogger<TransferController> logger)
        {
            _walletTransfer = walletTransfer;
            _logger = logger;
        }

        [HttpPost("wallet-transfer")]
        public async Task<IActionResult> ExecuteWalletTransfer([FromBody] WalletTransferRequestDto transferRequest)
        {
            if (transferRequest == null)
            {
                _logger.LogError("Received null transfer request");
                return BadRequest("Transfer request cannot be null");
            }

            _logger.LogInformation("Executing wallet transfer from {SourceWalletId} to {DestinationWalletId} of amount {Amount}",
                transferRequest.accountNumber, transferRequest.bankNumber, transferRequest.accountNumber);
            var payload = new WebhookEventPayload();

            var response = await _walletTransfer.ExecuteWalletTransfer(transferRequest, payload);

            if (response != null)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(500, response);
            }
        }
    }
}