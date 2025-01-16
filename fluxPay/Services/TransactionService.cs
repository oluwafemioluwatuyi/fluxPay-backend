using Microsoft.EntityFrameworkCore.Storage;
using fluxPay.Data;
using fluxPay.Interfaces.Services;

namespace fluxPay.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IFineractApiService _fineractApiService;

        public TransactionService(IFineractApiService fineractApiService)
        {
            _fineractApiService = fineractApiService;
        }

        public Task<bool> BeginTransactionAsync(Func<Task<bool>> transactionalOperation)
        {
            throw new NotImplementedException();
        }
    }
}