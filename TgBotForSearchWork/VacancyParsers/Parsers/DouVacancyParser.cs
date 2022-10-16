using TgBotForSearchWork.Models;

namespace TgBotForSearchWork.VacancyParsers.Parsers;

internal class DouVacancyParser : VacancyParser
{
    protected override HtmlElement VacancyItem => new("l-vacancy");

    protected override HtmlElement Title => new("title");

    protected override HtmlElement ShortDescription => new("sh-info");

    protected override HtmlElement Url => new("vt", "A");

}
