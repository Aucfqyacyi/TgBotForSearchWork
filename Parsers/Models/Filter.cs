using Parsers.Enums;

namespace Parsers.Models;

public class Filter
{
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public string GetParametrName { get; set; }
    public FilterType FilterType { get; set; }

    public Filter(string name, string categoryName, string getParametrName, FilterType filterType)
    {
        Name = name;
        GetParametrName = getParametrName;
        FilterType = filterType;
        CategoryName = categoryName;
    }

    public override string ToString()
    {
        return Name;
    }

}
