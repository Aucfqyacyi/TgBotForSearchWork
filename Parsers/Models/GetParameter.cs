namespace Parsers.Models;

public class GetParameter
{
    public string Name { get; set; }
    public string Value { get; set; }

    public GetParameter(string getParametrName, string getParametrValue)
    {
        Name = getParametrName;
        Value = getParametrValue;
    }

}
