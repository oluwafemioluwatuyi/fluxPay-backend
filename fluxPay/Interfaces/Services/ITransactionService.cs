using Microsoft.EntityFrameworkCore.Storage;

public interface ITransactionService
{
    Task<bool> BeginTransactionAsync(Func<Task<bool>> transactionalOperation);
}


