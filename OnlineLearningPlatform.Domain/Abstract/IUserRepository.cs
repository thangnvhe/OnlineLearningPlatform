using OnlineLearningPlatform.Domain.Common;
using OnlineLearningPlatform.Domain.Entities;

namespace OnlineLearningPlatform.Domain.Abstract
{
    public interface IUserRepository : IRepository<ApplicationUser, Guid>
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<PagedResult<ApplicationUser>> GetPagedAsync(PagingFilterBase filters);

    }
}
