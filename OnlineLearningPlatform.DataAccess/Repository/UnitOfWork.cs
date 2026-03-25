using OnlineLearningPlatform.DataAccess.Data;
using OnlineLearningPlatform.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IUserRepository Users { get; }
        public UnitOfWork(AppDbContext context, IUserRepository userRepository) 
        {
            _context = context;
            Users = userRepository;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return  _context.SaveChangesAsync(cancellationToken);
        }
    }
}
