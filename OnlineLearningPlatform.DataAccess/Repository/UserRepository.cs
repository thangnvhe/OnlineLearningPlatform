using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Domain.Abstract;
using OnlineLearningPlatform.Domain.Entities;
using System.Threading;
using OnlineLearningPlatform.DataAccess.Data;
using OnlineLearningPlatform.Domain.Common;

namespace OnlineLearningPlatform.DataAccess.Repository
{
    public class UserRepository : EfRepository<ApplicationUser, Guid>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
           
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await FindAll().Include(u=>u).ToListAsync();
        }

        public async Task<PagedResult<ApplicationUser>> GetPagedAsync(PagingFilterBase filters)
        {
            var query = FindAll().AsNoTracking();

            if (filters.IsActive != null )
            {
                query = query.Where(u => u.IsActive == filters.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var searchTerm = filters.Search.Trim().ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm) ||
                    u.Email!.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(filters.SortBy))
            {
                query = filters.SortBy.ToLower() switch
                {
                    "name" => filters.IsDescending
                        ? query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName)
                        : query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),

                    "email" => filters.IsDescending
                        ? query.OrderByDescending(u => u.Email)
                        : query.OrderBy(u => u.Email),

                    "dob" => filters.IsDescending
                        ? query.OrderByDescending(u => u.Dob)
                        : query.OrderBy(u => u.Dob),

                    // Mặc định sắp xếp theo ngày tạo (nếu bạn có trường CreatedAt) hoặc Id
                    _ => filters.IsDescending
                        ? query.OrderByDescending(u => u.Id)
                        : query.OrderBy(u => u.Id)
                };
            }
            else
            {
                // Sắp xếp mặc định khi người dùng không truyền tham số SortBy
                query = filters.IsDescending
                        ? query.OrderByDescending(u => u.Id)
                        : query.OrderBy(u => u.Id);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((filters.Page - 1) * filters.Size)
                .Take(filters.Size)
                .ToListAsync();

            var pagedResult = new PagedResult<ApplicationUser>
            {
                Items = items,
                TotalItems = totalItems,
                CurrentPage = filters.Page,
                PageSize = filters.Size,
            };
            return pagedResult;
        }
    }
}
