using Parsers.Utilities;

namespace Parsers.Models;

public record FilterCategory : IComparable<FilterCategory>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public HashSet<string> GetParameterNames { get; set; }

    public FilterCategory(string name, string? getParameter = null, int? id = null)
    {
        Id = id ?? UniqueIntGenerator.Generate();
        Name = name;
        GetParameterNames = new();
        if(getParameter is not null)
            GetParameterNames.Add(getParameter);
    }

    public FilterCategory(int id, string name, string? getParameter = null) : this(name, getParameter, id)
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
