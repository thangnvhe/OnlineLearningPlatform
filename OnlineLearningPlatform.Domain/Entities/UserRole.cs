using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Domain.Entities
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public virtual ApplicationUser? User { get; set; }
        public virtual IdentityRole<Guid>? Role { get; set; }
    }
}
