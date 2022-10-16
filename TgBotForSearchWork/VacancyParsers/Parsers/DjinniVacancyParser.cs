using TgBotForSearchWork.Models;

namespace TgBotForSearchWork.VacancyParsers.Parsers;

internal class DjinniVacancyParser : VacancyParser
{
    protected override HtmlElement VacancyItem => new("list-jobs__item");

    protected override HtmlElement Title => new("profile");

    protected override HtmlElement ShortDescription => new("list-jobs__description");

    protected override HtmlElement Url => new("profile", "A");
}
