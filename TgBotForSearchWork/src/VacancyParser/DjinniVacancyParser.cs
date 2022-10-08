using TgBotForSearchWork.src.Other;

namespace TgBotForSearchWork.src.VacancyParser;

internal class DjinniVacancyParser : VacancyParser
{
    protected override CssClass VacancyItem => new("list-jobs__item");

    protected override CssClass Title => new("list-jobs__title");

    protected override CssClass Description => new("list-jobs__description");

    protected override CssClass Date => new("text-date pull-right");

    protected override CssClass Url => new("profile");

    protected override string Host => @"https://djinni.co";
}
