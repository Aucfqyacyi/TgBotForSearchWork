using AngleSharp;
using AngleSharp.Dom;

namespace Parsers.Utilities;

internal static class HtmlDocument
{
    private readonly static IBrowsingContext _browsingContext;

    static HtmlDocument()
    {
        _browsingContext = BrowsingContext.New();
    }

    public static Task<IDocument> CreateAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        return _browsingContext.OpenAsync(req => req.Content(stream), cancellationToken);
    }
}
