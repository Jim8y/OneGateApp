import assert from 'node:assert/strict';
import { readFileSync } from 'node:fs';
import vm from 'node:vm';

const source = readFileSync(new URL('../../OneGateApp/Controls/Views/BridgeWebView.cs', import.meta.url), 'utf8');
const constants = Object.fromEntries([...source.matchAll(/const string (\w+) = "([^"]+)";/g)].map(match => [match[1], match[2]]));
const script = source.match(/return \$\$"""([\s\S]*?)"""\.ReplaceLineEndings/)[1]
  .replace(/\{\{(\w+)\}\}/g, (_, name) => constants[name])
  .replace(/\r\n|\r|\n/g, ''); // Match the real C# ReplaceLineEndings("") result.
let nextNativeToken = 1;
function document(isMainFrame = true) {
  const messages = [];
  const nativeToken = 'native-issued-' + nextNativeToken++;
  const window = { __OneGateBridge: {
    invoke: value => messages.push(JSON.parse(value)),
    invokeSync: value => {
      const request = JSON.parse(value);
      assert.equal(request.method, '__onegate_document');
      return JSON.stringify({ jsonrpc: '2.0', id: request.id, result: nativeToken });
    }
  } };
  window.top = isMainFrame ? window : {};
  const context = vm.createContext({ window, navigator: { platform: 'QA' } });
  vm.runInContext(script, context);
  return { window, context, messages, nativeToken };
}
const first = document();
const pending = first.window.__OneGateInvoke('getBlockCount', []);
const request = first.messages[0];
assert.equal(request.onegateDocument, first.window.__OneGateDocumentToken);
assert.ok(request.onegateDocument);
assert.equal(request.onegateDocument, first.nativeToken, 'request identity must be issued by native bootstrap, not supplied by arbitrary JS');
const response = { jsonrpc: '2.0', id: request.id, result: 123 };
const callback = source.match(/return EvaluateJavaScriptAsync\(\$"(if \(window\.__OneGateDocumentToken.+)"\);/)[1]
  .replace('{JsonSerializer.Serialize(context.DocumentToken)}', JSON.stringify(request.onegateDocument))
  .replace('{BridgeCallbackName}', constants.BridgeCallbackName)
  .replace('{response.ToJsonString()}', JSON.stringify(response));
vm.runInContext(callback, first.context);
assert.equal(await pending, 123);
const next = document();
let staleReplies = 0;
next.window.__OneGateBridgeCallback = () => staleReplies++;
vm.runInContext(callback, next.context);
assert.equal(staleReplies, 0, 'queued response must not reach a new document');
assert.equal(document(false).window.__OneGateInvoke, undefined, 'iframe has no RPC shim');
assert.equal(Object.getOwnPropertyDescriptor(first.window, '__OneGateDocumentToken').writable, false);
assert.doesNotMatch(source, /Navigating \+=/,
  'generic navigation events include iframe and canceled navigations and must not replace top-document identity');
console.log('PASS document-scoped payload and callback, navigation race, iframe shim and immutable token');

// Source-bound guard placement checks complement the policy execution tests.
// They do not claim to execute wallet authentication or platform delegates.
const dapi = readFileSync(new URL('../../OneGateApp/Pages/LaunchDAppPage.xaml.dAPI.cs', import.meta.url), 'utf8');
assert.match(dapi, /await activityLogService\.RecordWalletAuthorizationAsync\(DApp\);\s*EnsureCurrentBridgeRequest\(\);\s*WalletAccount/,
  'navigation during the authorization activity-log await must be rejected before authentication signing');
const remoteApproval = dapi.slice(dapi.indexOf('async Task<RemoteDebugApprovalResult> RequestRemoteApprovalAsync'), dapi.indexOf('async Task<UInt256> SignAndSendAsync'));
assert.match(remoteApproval, /if \(!approval\.Approved\) throw new OperationCanceledException\(\);\s*EnsureCurrentBridgeRequest\(\);\s*return approval;/,
  'remote approval results must be revalidated after their await');
const send = dapi.slice(dapi.indexOf('async Task<UInt256> SignAndSendAsync'));
assert.match(send, /EnsureCurrentBridgeRequest\(\);\s*var context/);
assert.doesNotMatch(send.slice(send.indexOf('var context'), send.indexOf('await rpcClient.SendRawTransaction')), /\bawait\b/,
  'no asynchronous gap is allowed between the final request guard and starting broadcast');
const ios = readFileSync(new URL('../../OneGateApp/Controls/Handlers/BridgeWebViewHandler.iOS.cs', import.meta.url), 'utf8');
assert.match(ios, /if \(prompt == SyncPrompt && handler.VirtualView is Views.BridgeWebView bridgeWebView\)/,
  'reserved sync prompts from child frames must be rejected by bridge policy instead of falling back to a native dialog');
console.log('PASS source-bound authentication, approval and broadcast guard placement, reserved iOS prompt routing');
