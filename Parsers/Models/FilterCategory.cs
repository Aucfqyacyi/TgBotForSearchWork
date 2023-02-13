using Parsers.Utilities;

namespace Parsers.Models;

public class FilterCategory : IComparable<FilterCategory>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public FilterCategory(string name)
    {
        Id = UniqueIntGenerator.Generate();
        Name = name;
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
