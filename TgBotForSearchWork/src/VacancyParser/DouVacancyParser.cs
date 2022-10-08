
using TgBotForSearchWork.src.Other;

namespace TgBotForSearchWork.src.VacancyParser;

internal class DouVacancyParser : BaseVacancyParser
{
    protected override CssClass VacancyItem => new("l-vacancy");

    protected override CssClass Title => new("title");

    protected override CssClass Description => new("sh-info");

    protected override CssClass Date => new("date");

    protected override CssClass Url => new("vt");
}
