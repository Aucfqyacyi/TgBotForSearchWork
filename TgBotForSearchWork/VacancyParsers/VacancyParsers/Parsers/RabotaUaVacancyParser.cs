using TgBotForSearchWork.VacancyParsers.Models;

namespace TgBotForSearchWork.VacancyParsers.VacancyParsers.Parsers;

internal class RabotaUaVacancyParser : VacancyParser
{
    protected override HtmlElement VacancyItem => new("ng-tns-c216-28 ng-star-inserted");

    protected override HtmlElement Title => new("santa-typo-h3 santa-mb-10 ng-tns-c216-28 ng-trigger ng-trigger-changeColor");

    protected override HtmlElement ShortDescription => new(string.Empty);

    protected override HtmlElement Url => new("card ng-tns-c216-28 santa-mb-20 ng-star-inserted");
    public override Task<List<Vacancy>> ParseAsync(Stream stream, string host, CancellationToken cancellationToken = default)
    {
        return base.ParseAsync(stream, host, cancellationToken);
    }
}
