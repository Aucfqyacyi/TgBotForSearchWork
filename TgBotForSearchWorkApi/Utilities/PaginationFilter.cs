using Deployf.Botf;

namespace TgBotForSearchWorkApi.Utilities;

public class PaginationFilter : PageFilter
{
    public PaginationFilter(int page, int limit = 10)
    {
        Page = page;
        Count = limit;
    }
}
