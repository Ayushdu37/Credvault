using System;
using System.Collections.Generic;
using System.Text;

namespace CredVault.Shared.Contracts.Common
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public static PaginatedResult<T> Create(
            List<T> items, int totalCount, int page, int pageSize)
        {
            return new PaginatedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
