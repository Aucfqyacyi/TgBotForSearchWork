using Parsers.Models;

namespace Parsers.VacancyParsers.Parsers;

internal class DjinniVacancyParser : HtmlPageVacancyParser
{
    protected override HtmlElement VacancyItem { get; } = new("list-jobs__item");

    protected override HtmlElement Title { get; } = new("profile", "A");

    protected override HtmlElement Description { get; } = new("profile-page-section");

    protected override HtmlElement Url { get; } = new("profile", "A");

    protected override uint IdPositionInUrl { get; } = 4;

    protected override string SymbolNearId { get; } = "-";

    public DjinniVacancyParser(HttpClient httpClient) : base(httpClient)
    {
    }
}
