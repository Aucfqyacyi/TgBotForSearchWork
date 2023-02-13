namespace Parsers.Models;

public class GetParametr
{
    public string Name { get; set; }
    public string Value { get; set; }

    public GetParametr(string getParametrName, string getParametrValue)
    {
        Name = getParametrName;
        Value = getParametrValue;
    }

}
