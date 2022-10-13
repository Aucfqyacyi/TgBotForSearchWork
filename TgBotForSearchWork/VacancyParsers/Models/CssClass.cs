namespace TgBotForSearchWork.VacancyParsers.Models;

internal class HtmlElement
{
    public string CssClassName { get; }
    public string TagName { get; }

    public HtmlElement(string name, string tagName = "")
    {
        CssClassName = name;
        TagName = tagName;
    }
}
