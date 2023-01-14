using Parsers.Models;

namespace Parsers.VacancyParsers.Parsers;

internal class WorkUaVacancyParser : VacancyParser
{
    protected override HtmlElement VacancyItem => new("card card-hover card-visited wordwrap job-link");

    protected override HtmlElement Title => new(string.Empty, "H2");

    protected override HtmlElement ShortDescription => new("overflow text-muted add-top-sm cut-bottom");

    protected override HtmlElement Url => new(string.Empty, "A");

    protected override uint IdPositionInUrl { get => 5; }

    protected override char SymbolAfterId { get => '/'; }
}
