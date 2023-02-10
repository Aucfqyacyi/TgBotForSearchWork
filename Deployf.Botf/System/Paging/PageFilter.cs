namespace Deployf.Botf;

public class PageFilter
{
    public int? Page { get; set; }
    public int? Count { get; set; }

    public PageFilter()
    {    }

    public PageFilter(int page, int limit = 10)
    {
        Page = page;
        Count = limit;
    }
}
