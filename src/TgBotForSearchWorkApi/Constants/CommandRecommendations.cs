using Parsers.Constants;

namespace TgBotForSearchWorkApi.Constants;

public static class CommandRecommendations
{
    private static Dictionary<SiteType, string> _all { get; set; } = new Dictionary<SiteType, string>();
    public static IReadOnlyDictionary<SiteType, string> All { get => _all; }

    static CommandRecommendations()
    {
        string firstRecommendation = "Виберіть потрібну категорію для фільтра, аналогічно якби ви вибирали фільтри на сайті.\n";
        string secondRecommendation = "Якщо ви вибрали категорію роботи, ви можете не вказувати пошуковий запит.\n";
        string thirdRecommendation = "Ви не можете вибрати одразу два фільтри на одну категорію, другий фільтр перезапише перший.\n";
        string workUaRecommendation = "На ворк юа я бы рекомендував використовувати пошук, бо их категорії не дуже корисні.\n";
        string djinniRecommendation = "На джині ви можете вказувати на одну категорію одразу декілка фільтрів.\n";
        _all.Add(SiteType.Dou, firstRecommendation + secondRecommendation + thirdRecommendation);
        _all.Add(SiteType.Djinni, firstRecommendation + secondRecommendation + djinniRecommendation);
        _all.Add(SiteType.WorkUa, firstRecommendation + workUaRecommendation + thirdRecommendation);
    }
}
