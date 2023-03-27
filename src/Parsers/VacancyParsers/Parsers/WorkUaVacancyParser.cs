using Parsers.Extensions;
using Parsers.Models;

namespace Parsers.VacancyParsers.Parsers;

internal class WorkUaVacancyParser : HtmlPageVacancyParser
{

    protected override HtmlElement VacancyItem { get; } = new("card card-hover card-visited wordwrap job-link");

    protected override HtmlElement Title { get; } = new(string.Empty, "H2");

    protected override HtmlElement Description { get; } = new("job-description");

    protected override HtmlElement Url { get; } = new(string.Empty, "A");

    protected override uint IdPositionInUrl { get; } = 4;

    protected override string SymbolNearId { get; } = "/";

    public WorkUaVacancyParser(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override async ValueTask<string?> GetDescriptionAsync(string url, uint descrtiptionLenght, CancellationToken cancellationToken)
    {
        var elements = await _httpClient.GetElementsAsync(new(url), document => document.GetElementById(Description.CssClassName), cancellationToken);
        return elements?.GetTextContent(descrtiptionLenght);
    }

}
