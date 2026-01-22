
using BlogPortfolio.Domain.Entities;

namespace BlogPortfolio.Application.Common.Interfaces
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(Guid id);
        Task UpdateAsync(Post post);
        Task AddAsync(Post post);
    }
}
