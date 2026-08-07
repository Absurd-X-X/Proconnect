namespace Application.Common.Pagenation
{
    public class PageResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int PageCount => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
