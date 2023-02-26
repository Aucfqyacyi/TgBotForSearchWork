namespace TgBotForSearchWorkApi.Constants;

public static class CommandDescription
{
    public const string Start = "Для початку спілкування с ботом.";
    public const string Stop = "Якщо ви бажаєте більше не отримувати вакансії.";
    public const string AddUrl = "Додати вже готове посилання на вакансії.";
    public const string GetUrl = "Отримати повне посилання на вакансії.";
    public const string DeleteUrl = "Видалити посилання.";
    public const string CreateUrl = "Створити посилання за допомогую фільтров сайту.";
    public const string AddFilter = "Додати фільтр до готового посилання.";
    public const string RemoveFilter = "Видалити фільтр с посилання.";
    public const string ChangeDescriptionLength = "Змінити розмір опису вакансії, яку присилає бот, мінімальне значення 0, тобто бот буде присилати лише назву вакансії, максимальне 6000.";
    public const string Empty = "|";
}
