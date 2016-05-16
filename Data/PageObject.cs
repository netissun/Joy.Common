
namespace Joy.Common.Data
{
    public class PageObject
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; }

        public PageObject()
        {
            this.PageSize = 0;
            this.PageIndex = 0;
        }
        public PageObject(int pageSize, int pageIndex)
        {
            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
        }
    }
}
