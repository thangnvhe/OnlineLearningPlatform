using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Domain.Setting
{
    public abstract class PagingFilterBase
    {
        const int MaxPageSize = 50;
        public int Page { get; set; } = 1;
        private int _size  = 10;
        public int Size
        {
            get => _size;
            set => _size = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDescending { get; set; } = false;
    }
}
