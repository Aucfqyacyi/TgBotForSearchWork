using TgBotForSearchWork.VacancyParsers.Models;

namespace TgBotForSearchWork.VacancyParsers.AllVacancyParsers;

internal class DjinniVacancyParser : AllVacancyParser
{
    protected override CssClass VacancyItem => new("list-jobs__item");

    protected override CssClass Title => new("list-jobs__title");

    protected override CssClass ShortDescription => new("list-jobs__description");

    protected override CssClass Date => new("text-date pull-right");

    protected override CssClass Url => new("profile");
}
