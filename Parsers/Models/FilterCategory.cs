using Parsers.Utilities;

namespace Parsers.Models;

public class FilterCategory : IComparable<FilterCategory>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? GetParameterName { get; set; }

    public FilterCategory(string name, string? getParameterName = null)
    {
        Id = UniqueIntGenerator.Generate();
        Name = name;
        GetParameterName = getParameterName;
    }

    public int CompareTo(FilterCategory? other)
    {
        return Id - other?.Id ?? 0;
    }

    public static implicit operator FilterCategory(int id)
    {
        return new(string.Empty) { Id = id };
    }
}
