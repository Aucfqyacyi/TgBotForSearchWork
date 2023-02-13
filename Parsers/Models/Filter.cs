using Parsers.Constants;
using Parsers.Utilities;

namespace Parsers.Models;

public class Filter
{
    public int Id { get; set; }
    public string Name { get; set; }
    public FilterCategory Category { get; set; }
    public GetParametr GetParametr { get; set; }
    public FilterType FilterType { get; set; }

    public Filter(string name, FilterCategory category, GetParametr getParametr, FilterType filterType)
    {
        Id = UniqueIntGenerator.Generate();
        Name = name;
        GetParametr = getParametr;
        FilterType = filterType;
        Category = category;
    }

}
