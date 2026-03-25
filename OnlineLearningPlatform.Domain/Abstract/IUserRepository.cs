using OnlineLearningPlatform.Domain.Entities;
using OnlineLearningPlatform.Domain.Setting;

namespace OnlineLearningPlatform.Domain.Abstract
{
    public interface IUserRepository : IRepository<ApplicationUser, Guid>
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<PagedResult<ApplicationUser>> GetPagedAsync(PagingFilterBase filters);

    }
}
