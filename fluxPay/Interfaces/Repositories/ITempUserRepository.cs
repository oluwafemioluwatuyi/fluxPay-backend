using FluxPay.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace fluxPay.Interfaces.Repositories
{
    public interface ITempUserRepository
    {
        Task <TempUser> GetUserById(Guid id);
        Task <TempUser> GetUserByEmail(string email);
        void AddAsync(TempUser user);
        void Remove(TempUser user);
        Task<bool> SaveChangesAsync(TempUser tempUser);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
