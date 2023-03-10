using Parsers.Constants;
using Parsers.Utilities;

namespace Parsers.Models;

public class Filter
{
    public int Id { get; set; }
    public string Name { get; set; }
    public FilterCategory Category { get; set; }
    public GetParameter GetParameter { get; set; }
    public FilterType FilterType { get; set; }

    public Filter(string name, FilterCategory category, GetParameter getParametr, FilterType filterType)
    {
        Id = UniqueIntGenerator.Generate();
        Name = name;
        FilterType = filterType;
        Category = category;
        GetParameter = getParametr;
    }
}
