namespace NeoOrder.OneGate.Controls.Views;

public readonly record struct BridgeRequestContext(long NavigationId, string Origin, string DocumentToken = "");

/// <summary>Validates native-supplied frame provenance and expires requests on navigation.</summary>
internal sealed class BridgeRequestPolicy
{
    readonly object gate = new();
    long navigationId;
    string documentToken = string.Empty;

    // Called only through the native-protected synchronous document-start handshake.
    public string BeginDocument()
    {
        lock (gate)
        {
            navigationId++;
            return documentToken = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
        }
    }

    public void Invalidate()
    {
        lock (gate) { navigationId++; documentToken = string.Empty; }
    }

    public bool IsCurrent(BridgeRequestContext context)
    {
        lock (gate)
            return !string.IsNullOrEmpty(context.Origin) && !string.IsNullOrEmpty(documentToken)
                && context.NavigationId == navigationId && StringComparer.Ordinal.Equals(context.DocumentToken, documentToken);
    }

    public bool TryAuthorize(string? sourceUrl, string? pageUrl, bool isMainFrame, out BridgeRequestContext context)
    {
        context = default;
        if (!isMainFrame || !TryGetWebOrigin(sourceUrl, out Uri? source) || !TryGetWebOrigin(pageUrl, out Uri? page))
            return false;
        if (!StringComparer.OrdinalIgnoreCase.Equals(source.Scheme, page.Scheme)
            || !StringComparer.OrdinalIgnoreCase.Equals(source.IdnHost, page.IdnHost)
            || source.Port != page.Port)
            return false;
        lock (gate) context = new(navigationId, source.GetLeftPart(UriPartial.Authority), documentToken);
        return true;
    }

    public bool TryAuthorizeSync(string? suppliedToken, string expectedToken, string? sourceUrl, string? pageUrl, out BridgeRequestContext context)
    {
        context = default;
        return !string.IsNullOrEmpty(expectedToken)
            && StringComparer.Ordinal.Equals(suppliedToken, expectedToken)
            && TryAuthorize(sourceUrl, pageUrl, true, out context);
    }

    public bool TryBindDocument(BridgeRequestContext context, string? token, out BridgeRequestContext bound)
    {
        bound = default;
        lock (gate)
        {
            if (!IsCurrent(context) || !StringComparer.Ordinal.Equals(token, documentToken)) return false;
            bound = context;
            return true;
        }
    }

    static bool TryGetWebOrigin(string? value, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out Uri? uri)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out uri) && uri.Scheme is "http" or "https";
    }
}
