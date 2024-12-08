using fluxPay.Data;
using fluxPay.Interfaces.Repositories;
using FluxPay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace fluxPay.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly fluxPayDbContext dbContext;
        public UserRepository(fluxPayDbContext dbContext) 
        {
            this.dbContext = dbContext;

        }
        public void AddAsync(User user)
        {
            dbContext.Add(user);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await dbContext.Database.BeginTransactionAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User> GetUserById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Remove(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
