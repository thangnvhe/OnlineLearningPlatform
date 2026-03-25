using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineLearningPlatform.Domain.Setting
{
    public class PagedResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;

        public PagedResult()
        {
            Items = new List<T>();
        }

        public PagedResult(IEnumerable<T> items, int totalItems, int currentPage, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
        }
    }
}
