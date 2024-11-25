namespace PhotoSiTest.Common.BaseTypes
{
    public class PaginatedResult<TDto>
    {
        public IEnumerable<TDto> Items { get; set; } = Enumerable.Empty<TDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
