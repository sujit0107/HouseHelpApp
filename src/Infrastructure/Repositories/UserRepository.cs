using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Repositories;
using HouseHelp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HouseHelp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
        => _dbContext.Users.Include(u => u.HelperProfile).FirstOrDefaultAsync(u => u.Phone == phone, cancellationToken);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _dbContext.Users.Include(u => u.HelperProfile).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        => await _dbContext.Users.AddAsync(user, cancellationToken);

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _dbContext.Users.Update(user);
        return Task.CompletedTask;
    }
}
