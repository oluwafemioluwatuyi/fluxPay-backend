using fluxPay.Data;
using fluxPay.Interfaces.Repositories;
using FluxPay.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace fluxPay.Repositories
{
    public class TempUserRepository : ITempUserRepository
    {
        private readonly fluxPayDbContext dbContext;
        public TempUserRepository(fluxPayDbContext dbContext) 
        {
            this.dbContext = dbContext;

        }

        public void AddAsync(TempUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await dbContext.Database.BeginTransactionAsync();
        }
        public void Remove(TempUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveChangesAsync(TempUser tempUser)
        {
            dbContext.TempUsers.Add(tempUser);
             await dbContext.SaveChangesAsync();
             return true;
        }

        public async Task<TempUser> GetUserByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
    {
        throw new ArgumentException("Email cannot be null or empty", nameof(email));
    }

    try
    {
        return await dbContext.TempUsers
            .FirstOrDefaultAsync(u => u.Email == email && u.Status == "Pending");
    }
    catch (Exception ex)
    {
        // Log the error for debugging purposes
        Console.WriteLine($"Error in GetUserByEmail: {ex.Message}");
        throw;
    }
        }

        public Task<TempUser> GetUserById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
