using TgBotForSearchWork.VacancyParsers.Models;

namespace TgBotForSearchWork.VacancyParsers.VacancyParsers;

internal class DouVacancyParser : AllVacancyParser
{
    protected override CssClass VacancyItem => new("l-vacancy");

    protected override CssClass Title => new("title");

    protected override CssClass ShortDescription => new("sh-info");

    protected override CssClass Date => new("date");

    protected override CssClass Url => new("vt");

}
