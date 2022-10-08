using AngleSharp;
using AngleSharp.Dom;

namespace TgBotForSearchWork.src.Other;

internal class HtmlDocument
{
    private readonly static IBrowsingContext _browsingContext;

    static HtmlDocument()
    {
        _browsingContext = BrowsingContext.New();
    }

    public static Task<IDocument> Create(Stream stream, CancellationToken cancellationToken = default)
    {
        return _browsingContext.OpenAsync(req => req.Content(stream), cancellationToken);
    }
}
