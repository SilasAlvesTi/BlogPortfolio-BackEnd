using BlogPortfolio.Domain.Entities;

namespace BlogPortfolio.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
    }
}
