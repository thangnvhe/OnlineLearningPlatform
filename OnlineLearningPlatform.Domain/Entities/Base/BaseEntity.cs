using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Domain.Entities.Base
{
    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        [Key]
        public TKey Id { get; set; } = default!;
    }
}
