using Parsers.Enums;

namespace Parsers.Models;

public class Filter
{
    public string Name { get; set; }
    public string GetParametrName { get; set; }
    public string GetParametrValue { get; set; }
    public FilterType FilterType { get; set; }

    public Filter(string name, string getParametrName, string getParametrValue, FilterType filterType)
    {
        Name = name;
        GetParametrName = getParametrName;
        GetParametrValue = getParametrValue;
        FilterType = filterType;
    }

}
