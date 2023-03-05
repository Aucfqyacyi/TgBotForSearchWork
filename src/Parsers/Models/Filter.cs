using Parsers.Constants;
using Parsers.Utilities;

namespace Parsers.Models;

public class Filter
{
    public int Id { get; set; }
    public string Name { get; set; }
    public FilterCategory Category { get; set; }
    public string GetParameterValue { get; set; }
    public GetParameter GetParameter { get => new(Category.GetParameterName, GetParameterValue); }
    public FilterType FilterType { get; set; }

    public Filter(string name, FilterCategory category, string getParametrValue, FilterType filterType)
    {
        Id = UniqueIntGenerator.Generate();
        Name = name;
        FilterType = filterType;
        Category = category;
        GetParameterValue = getParametrValue;
    }

}
