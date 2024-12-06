using FluxPay.Models;
using Microsoft.EntityFrameworkCore;

namespace fluxPay.Data
{
    public class fluxPayDbContext: DbContext
    {
        public fluxPayDbContext(DbContextOptions<fluxPayDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
