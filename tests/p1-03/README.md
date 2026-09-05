# P1-03: native DApp bridge provenance

Run the linked production policy tests:

```sh
dotnet run --project tests/p1-03/P1-03.Tests.csproj
node tests/p1-03/dapi-script.test.mjs
```

The tests were added first (RED: policy source missing); lifecycle regressions
were added before the native bootstrap implementation (RED), then pass 26/26. They
exercise main-frame/origin checks, scheme/port/file-origin rejection, navigation
expiration, and the Android main-frame private capability used for synchronous
system calls. Native-issued document tokens reject old same-origin messages even
when delivered after a new document becomes active. Ordinary payloads cannot
replace the native identity. The Node test executes the actual injected RPC script and native
callback template, checking old replies are rejected after a new document loads.
Source-bound assertions also check authentication after its activity-log await,
post-await remote approval guards, no asynchronous signing-to-broadcast gap, and
reserved iOS child-frame sync prompt rejection. These assertions inspect guard
placement; they do not execute wallet signing or platform prompt delegates.
Native integration still requires the checks below; these tests are not simulator
validation. The test project is registered in the solution and declares a
non-building, non-output reference to the app; its build compiles only linked
production policy source, not mobile target frameworks.

## Native QA before committing

Run on both real Android and iOS simulators. Serve `fixtures/` on two local HTTP
ports, e.g. 8765 and 8766. Open `index.html?iframePort=8766` in OneGate's DApp
debug launcher, using a hostname reachable from the simulator (typically
`10.0.2.2` on Android or `127.0.0.1` on iOS). The page embeds one same-origin
iframe and one different-port iframe. All probes use read-only RPC or cancelable
address selection; do not authorize transactions or publish wallet data.

1. Main-frame `getBlockCount` succeeds. Main-frame address picker opens and can
   be canceled normally; normal error/cancel callbacks still resolve.
2. In **both** iframe probes, raw native `pickAddress` messages do not open any
   wallet popup or appear as accepted dAPI requests. No provider or orientation
   shims are installed in iframe globals.
3. Raw Android sync prompt without the private main-frame token is rejected;
   normal main-frame `screen.orientation.lock('landscape')` followed by the
   synchronous `screen.orientation.unlock()` continues to work. On iOS test
   orientation and fullscreen enter/exit as well.
4. Begin a slow read-only main-frame request, navigate/reload before it resolves,
   and verify its callback does not reach the new document. A transaction/message
   confirmation left pending while its document navigates must not sign after
   approval; use an instrumented local authorization fixture, not a real transfer.
   Also keep a main-frame request pending during iframe navigation and a canceled
   cross-origin top navigation; both must survive because the top document stays.
   Hold a same-origin navigation response, send a late request from the still-live
   old document, finish navigation, then verify that request and old-token replay
   are rejected. Do not substitute navigation-start generation checks for this.
5. Main-frame signing confirmations show the native-verified requesting origin.
   Open, cancel, close, and reopen the DApp; verify the bridge remains usable.

Android's prompt callback does not expose a frame flag, so synchronous calls use
a random capability held only in the top-frame injection's closure, together
with the native prompt source URL. Same-origin JavaScript can intentionally call
the parent page's public methods under normal browser same-origin permissions;
this change prevents direct untrusted frame-to-native calls, not cooperation by
the trusted parent page. Windows uses the same protected prompt capability since
its dialog callback also lacks a main-frame flag. A private synchronous native
bootstrap runs only at top-document start and establishes the active identity;
generic MAUI `Navigating` events include iframe/canceled navigations and do not
replace that identity. Handler removal and unload revoke it. No insecure
`AddJavascriptInterface` fallback remains.

## Integration

This branch requires both Android `DOCUMENT_START_SCRIPT` and
`WEB_MESSAGE_LISTENER`. Independent P2-16 adds the native missing-capability UX;
when integrating both, its preflight must include the latter capability too.
No simulator validation, app build, commit, or push was performed by the worker.
