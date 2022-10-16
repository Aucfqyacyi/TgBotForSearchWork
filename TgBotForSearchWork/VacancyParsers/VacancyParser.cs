using AngleSharp.Dom;
using TgBotForSearchWork.Extensions;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Others;

namespace TgBotForSearchWork.VacancyParsers;

internal abstract class VacancyParser : IAllVacancyParser
{
    protected abstract HtmlElement VacancyItem { get; }
    protected abstract HtmlElement Title { get; }
    protected abstract HtmlElement ShortDescription { get; }
    protected abstract HtmlElement Url { get; }

    public virtual async Task<List<Vacancy>> ParseAsync(Stream stream, string host, CancellationToken cancellationToken = default)
    {
        IDocument doc = await HtmlDocument.CreateAsync(stream, cancellationToken);
        IHtmlCollection<IElement> vacancyElements = doc.GetElementsByClassName(VacancyItem.CssClassName);
        List<Task<Vacancy>> tasks = new();
        foreach (IElement vacancyElement in vacancyElements)
        {
            tasks.Add(CreateVacancyAsync(vacancyElement, host));
        }          
        await Task.WhenAll(tasks);
        return tasks.Aggregate(new List<Vacancy>(), (vacancies, task) =>
        {
            vacancies.Add(task.Result);
            return vacancies;
        });
    }

    private Vacancy CreateVacancy(IElement element, string host)
    {
        string url = element.GetIElement(Url)?.GetHrefAttribute(host) ?? string.Empty;
        string title = element.GetTextContent(Title);
        string shortDescription = element.GetTextContent(ShortDescription);
        return new(title, url, shortDescription);
    }

    private Task<Vacancy> CreateVacancyAsync(IElement element, string host)
    {
        return Task.Run(() => CreateVacancy(element, host));
    }
}
