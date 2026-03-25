using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Domain.Entities.Base
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; } 
    }
}
