using TgBotForSearchWork.VacancyParsers.Models;

namespace TgBotForSearchWork.VacancyParsers.VacancyParsers.Parsers;

internal class WorkUaVacancyParser : VacancyParser
{
    protected override HtmlElement VacancyItem => new("card card-hover card-visited wordwrap job-link");

    protected override HtmlElement Title => new(string.Empty, "H2");

    protected override HtmlElement ShortDescription => new("overflow text-muted add-top-sm cut-bottom");

    protected override HtmlElement Url => new(string.Empty, "A");
    public override Task<List<Vacancy>> ParseAsync(Stream stream, string host, CancellationToken cancellationToken = default)
    {
        return base.ParseAsync(stream, host, cancellationToken);
    }
}
