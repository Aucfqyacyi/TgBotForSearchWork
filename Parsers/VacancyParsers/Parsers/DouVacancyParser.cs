using Parsers.Models;

namespace Parsers.VacancyParsers.Parsers;

internal class DouVacancyParser : VacancyParser
{
    protected override HtmlElement VacancyItem { get; } = new("l-vacancy");

    protected override HtmlElement Title { get; } = new("title");

    protected override HtmlElement ShortDescription { get; } = new("sh-info");

    protected override HtmlElement Url { get; } = new("vt", "A");

    protected override uint IdPositionInUrl { get; } = 6;

    protected override char SymbolAfterId { get; } = '/';

}
