using Parsers.Extensions;
using Parsers.Models;
using Parsers.Utilities;

namespace Parsers.VacancyParsers.Parsers;

internal class WorkUaVacancyParser : VacancyParser
{
    protected override HtmlElement VacancyItem { get; } = new("card card-hover card-visited wordwrap job-link");

    protected override HtmlElement Title { get; } = new(string.Empty, "H2");

    protected override HtmlElement Description { get; } = new("job-description");

    protected override HtmlElement Url { get; } = new(string.Empty, "A");

    protected override uint IdPositionInUrl { get; } = 4;

    protected override string SymbolNearId { get; } = "/";


    protected override async ValueTask<string?> GetDescriptionAsync(string url, int descrtiptionLenght, CancellationToken cancellationToken)
    {
        var elements = await HtmlParser.GetElementsAsync(new(url), document => document.GetElementById(Description.CssClassName), cancellationToken);
        return elements?.GetTextContent(descrtiptionLenght);
    }

}
