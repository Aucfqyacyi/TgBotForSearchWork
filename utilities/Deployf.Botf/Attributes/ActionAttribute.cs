namespace Deployf.Botf;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class ActionAttribute : Attribute
{
    protected string? _template;
    protected string? _description;

    public string? Template { get => _template; }
    public string? Description { get => _description; }

    public ActionAttribute(string? template = null, string? description = null)
    {
        _template = template;
        _description = description;
    }
}
