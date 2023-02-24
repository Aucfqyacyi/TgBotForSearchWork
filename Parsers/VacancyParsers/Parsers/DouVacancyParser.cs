using Parsers.Models;

namespace Parsers.VacancyParsers.Parsers;

internal class DouVacancyParser : VacancyParser
{
    protected override HtmlElement VacancyItem { get; } = new("l-vacancy");

    protected override HtmlElement Title { get; } = new("vt", "A");

    protected override HtmlElement Description { get; } = new("text b-typo vacancy-section");

    protected override HtmlElement Url { get; } = new("vt", "A");

    protected override uint IdPositionInUrl { get; } = 6;

    protected override string SymbolNearId { get; } = "/";

}
