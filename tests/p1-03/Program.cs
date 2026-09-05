using NeoOrder.OneGate.Controls.Views;

int failures = 0;
void Check(bool condition, string name)
{
    Console.WriteLine($"{(condition ? "PASS" : "FAIL")} {name}");
    if (!condition) failures++;
}

var policy = new BridgeRequestPolicy();
string firstToken = policy.BeginDocument();
Check(policy.TryAuthorize("https://game.example", "https://game.example/play", true, out var request), "current main frame accepted");
Check(!policy.TryAuthorize("https://game.example", "https://game.example/play", false, out _), "same-origin iframe rejected");
Check(!policy.TryAuthorize("https://ads.example", "https://game.example/play", false, out _), "cross-origin iframe rejected");
Check(!policy.TryAuthorize("https://ads.example", "https://game.example/play", true, out _), "forged source origin rejected");
Check(!policy.TryAuthorize("http://game.example", "https://game.example/play", true, out _), "scheme mismatch rejected");
Check(!policy.TryAuthorize("https://game.example:8443", "https://game.example/play", true, out _), "port mismatch rejected");
Check(!policy.TryAuthorize("file:///tmp/a.html", "file:///tmp/a.html", true, out _), "opaque and file origins rejected");
Check(!policy.TryAuthorize("null", "https://game.example", true, out _), "null origin rejected");
Check(policy.TryAuthorize("https://GAME.EXAMPLE:443", "https://game.example/play", true, out _), "canonical origin accepted");
Check(policy.TryAuthorize("http://localhost:8080", "http://localhost:8080/demo", true, out _), "local developer web origin preserved");
Check(policy.IsCurrent(request), "current request can reply");
string secondToken = policy.BeginDocument();
Check(firstToken != secondToken, "native issues a fresh token for each actual document");
Check(!policy.IsCurrent(request), "new document invalidates old response and signing permission");
Check(policy.TryAuthorize("https://game.example", "https://game.example/next", true, out var next) && policy.IsCurrent(next), "new document request accepted");
Check(!policy.TryAuthorizeSync("wrong", "expected", "https://game.example", "https://game.example", out _), "sync prompt without main-frame token rejected");
Check(policy.TryAuthorizeSync("expected", "expected", "https://game.example", "https://game.example/play", out _), "sync main-frame capability preserved");
Check(!policy.TryAuthorizeSync("expected", "expected", "https://ads.example", "https://game.example", out _), "sync token alone cannot bypass origin");
Check(policy.TryBindDocument(next, secondToken, out var bound) && bound.DocumentToken == secondToken, "native context binds to its issued document token");
Check(!policy.TryBindDocument(next, "attacker-selected", out _), "ordinary payload cannot replace native-issued document identity");
Check(!policy.TryBindDocument(request, firstToken, out _), "old document request cannot bind a new response");
Check(!policy.TryBindDocument(next, firstToken, out _), "late old-document message cannot acquire a new navigation generation");
Check(!policy.IsCurrent(next with { DocumentToken = firstToken }), "current generation with stale token cannot authorize signing or reply");
Check(!policy.TryBindDocument(next, null, out _), "missing document token rejected");
Check(!policy.TryBindDocument(next, new string('x', 129), out _), "oversized document token rejected");
policy.Invalidate();
Check(!policy.IsCurrent(next), "handler disconnect and unload revoke the active document");
Check(policy.TryAuthorize("https://game.example", "https://game.example/next", true, out var unloaded)
    && !policy.TryBindDocument(unloaded, secondToken, out _), "an unloaded document cannot revive itself with its previous token");
Environment.ExitCode = failures == 0 ? 0 : 1;
