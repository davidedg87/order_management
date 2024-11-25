namespace PhotoSiTest.Common.BaseTypes
{
    public class PagedFilter
    {
        public int PageNumber { get; set; } = 1; // Default alla prima pagina
        public int PageSize { get; set; } = 10;  // Default a 10 elementi per pagina
    }
}
