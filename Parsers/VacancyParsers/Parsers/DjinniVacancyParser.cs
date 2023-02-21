using Parsers.Models;

namespace Parsers.VacancyParsers.Parsers;

internal class DjinniVacancyParser : VacancyParser
{
    protected override HtmlElement VacancyItem { get; } = new("list-jobs__item");

    protected override HtmlElement Title { get; } = new("profile");

    protected override HtmlElement ShortDescription { get; } = new("list-jobs__description");

    protected override HtmlElement Url { get; } = new("profile", "A");

    protected override uint IdPositionInUrl { get; } = 4;

    protected override string SymbolNearId { get; } = "-";
}
