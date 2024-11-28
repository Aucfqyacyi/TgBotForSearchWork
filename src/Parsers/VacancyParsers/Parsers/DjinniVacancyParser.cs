using Parsers.Models;

namespace Parsers.VacancyParsers.Parsers;

internal class DjinniVacancyParser : HtmlPageVacancyParser
{
    protected override HtmlElement VacancyItem { get; } = new("list-unstyled list-jobs");

    protected override HtmlElement Title { get; } = new("job-item__title-link", "A");

    protected override HtmlElement Description { get; } = new("job-post__description");

    protected override HtmlElement Url { get; } = new("job-item__title-link", "A");

    protected override uint IdPositionInUrl { get; } = 4;

    protected override string SymbolNearId { get; } = "-";

    public DjinniVacancyParser(HttpClient httpClient) : base(httpClient)
    {
    }
}
