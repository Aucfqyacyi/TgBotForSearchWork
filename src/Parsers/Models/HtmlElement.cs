namespace Parsers.Models;

internal class HtmlElement
{
    public string CssClassName { get; }
    public string TagName { get; }

    public HtmlElement(string cssClassName, string tagName = "")
    {
        CssClassName = cssClassName;
        TagName = tagName;
    }
}
