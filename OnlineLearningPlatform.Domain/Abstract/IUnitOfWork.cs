using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Domain.Abstract
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
