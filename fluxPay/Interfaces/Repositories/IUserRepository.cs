using FluxPay.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace fluxPay.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task <User> GetUserById(Guid id);
        Task <User> GetUserByEmail(string email);
        void AddAsync(User user);
        void Remove(User user);
        Task<bool> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
