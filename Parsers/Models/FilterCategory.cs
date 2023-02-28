using Parsers.Utilities;

namespace Parsers.Models;

public record FilterCategory : IComparable<FilterCategory>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? GetParameterName { get; set; }

    public FilterCategory(string name, string? getParameterName = null, int? id = null)
    {
        Id = id ?? UniqueIntGenerator.Generate();
        Name = name;
        GetParameterName = getParameterName;
    }

    public FilterCategory(int id, string name, string getParameterName) : this(name, getParameterName, id)
    {
    }

    public int CompareTo(FilterCategory? other)
    {
        return Id - other?.Id ?? 0;
    }

    public static implicit operator FilterCategory(int id)
    {
        return new(string.Empty, string.Empty) { Id = id };
    }
}
