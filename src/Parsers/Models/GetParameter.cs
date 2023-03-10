namespace Parsers.Models;

public class GetParameter
{
    public string Name { get; set; }
    public string Value { get; set; }
    public bool CanBeDuplicated { get; set; }

    public GetParameter(string getParameterName, string getParameterValue, bool canBeDuplicated = false)
    {
        Name = getParameterName;
        Value = getParameterValue;
        CanBeDuplicated = canBeDuplicated;
    }
}
