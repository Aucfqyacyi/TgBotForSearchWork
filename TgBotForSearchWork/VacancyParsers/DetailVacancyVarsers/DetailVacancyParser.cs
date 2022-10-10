using AngleSharp.Dom;
using System.Text;
using TgBotForSearchWork.VacancyParsers.Extensions;
using TgBotForSearchWork.VacancyParsers.Models;
using TgBotForSearchWork.VacancyParsers.Others;

namespace TgBotForSearchWork.VacancyParsers.DetailVacancyVarsers;

internal class DetailVacancyParser : IDetailVacancyParser
{
    protected CssClass Description { get; } = new("text b-typo vacancy-section");

    public async Task<string> ParseAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        IDocument doc = await HtmlDocument.CreateAsync(stream, cancellationToken);
        IHtmlCollection<IElement> descriptionElements = doc.GetElementsByClassName(Description.Name);
        StringBuilder stringBuilder = new StringBuilder();
        foreach (IElement descriptionElement in descriptionElements)
        {
            IElement? element = descriptionElement.FirstElementChild;
            if (element is not null)
            {
                stringBuilder.Append(element.GetAllText());
            }
        }
        return "aboba";
    }
}
