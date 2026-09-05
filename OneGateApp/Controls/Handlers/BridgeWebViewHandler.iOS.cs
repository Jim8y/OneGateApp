#if IOS || MACCATALYST

using CoreGraphics;
using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using WebKit;

namespace NeoOrder.OneGate.Controls.Handlers;

partial class BridgeWebViewHandler
{
    const string SyncPrompt = "__OneGateBridgeSync";

    class ScriptHandler(BridgeWebViewHandler handler) : NSObject, IWKScriptMessageHandler
    {
        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            handler.BridgeWebView.OnMessage(message.Body?.ToString() ?? string.Empty,
                GetFrameOrigin(message.FrameInfo), handler.PlatformView.Url?.AbsoluteString, message.FrameInfo.MainFrame);
        }
    }

    BridgeWebViewUIDelegate? uiDelegate;

    class BridgeWebViewUIDelegate : MauiWebViewUIDelegate
    {
        readonly IWebViewHandler handler;

        public BridgeWebViewUIDelegate(IWebViewHandler handler) : base(handler)
        {
            this.handler = handler;
        }

        public override void RequestDeviceOrientationAndMotionPermission(
            WKWebView webView,
            WKSecurityOrigin origin,
            WKFrameInfo frame,
            Action<WKPermissionDecision> decisionHandler)
        {
            decisionHandler(frame.MainFrame ? WKPermissionDecision.Grant : WKPermissionDecision.Deny);
        }

        public override void RequestMediaCapturePermission(
            WKWebView webView,
            WKSecurityOrigin origin,
            WKFrameInfo frame,
            WKMediaCaptureType type,
            Action<WKPermissionDecision> decisionHandler)
        {
            decisionHandler(frame.MainFrame && type == WKMediaCaptureType.Camera ? WKPermissionDecision.Prompt : WKPermissionDecision.Deny);
        }

#pragma warning disable CS0672
        public override void RunJavaScriptTextInputPanel(
            WKWebView webView,
            string prompt,
            string? defaultText,
            WKFrameInfo frame,
            Action<string> completionHandler)
        {
            if (prompt == SyncPrompt && handler.VirtualView is Views.BridgeWebView bridgeWebView)
            {
                completionHandler(bridgeWebView.OnSyncMessage(defaultText ?? string.Empty,
                    GetFrameOrigin(frame), webView.Url?.AbsoluteString, frame.MainFrame));
                return;
            }

#pragma warning disable CS0618
            base.RunJavaScriptTextInputPanel(webView, prompt, defaultText, frame, completionHandler);
#pragma warning restore CS0618
        }
#pragma warning restore CS0672
    }

    static partial void ConfigureMapper(PropertyMapper<IWebView, IWebViewHandler> mapper)
    {
        mapper[nameof(WKUIDelegate)] = MapBridgeWKUIDelegate;
    }

    static void MapBridgeWKUIDelegate(IWebViewHandler handler, IWebView webView)
    {
        if (handler is not BridgeWebViewHandler bridgeHandler)
            return;

        bridgeHandler.PlatformView.UIDelegate = bridgeHandler.uiDelegate ??= new BridgeWebViewUIDelegate(bridgeHandler);
    }

    protected override WKWebView CreatePlatformView()
    {
        var config = MauiWKWebView.CreateConfiguration();
        config.AllowsInlineMediaPlayback = true;
        config.MediaTypesRequiringUserActionForPlayback = WKAudiovisualMediaTypes.None;
        var controller = new WKUserContentController();
        string shim = """
            window.__OneGateBridge = {
                invoke: function(payload) {
                    window.webkit.messageHandlers.__OneGateBridge.postMessage(payload);
                },
                invokeSync: function(payload) {
                    return window.prompt("__OneGateBridgeSync", payload);
                }
            };
            """;
        controller.AddUserScript(CreateDocumentStartScript(shim + Views.BridgeWebView.CreateRpcScript()));
        if (!string.IsNullOrWhiteSpace(BridgeWebView.DocumentStartScript))
            controller.AddUserScript(CreateDocumentStartScript(BridgeWebView.DocumentStartScript));
        controller.AddScriptMessageHandler(new ScriptHandler(this), "__OneGateBridge");
#if MACCATALYST
        config.Preferences.ElementFullscreenEnabled = true;
#endif
        config.UserContentController = controller;
        return new MauiWKWebView(CGRect.Empty, this, config);
    }

    static WKUserScript CreateDocumentStartScript(string script)
    {
        return new WKUserScript(new NSString(script), WKUserScriptInjectionTime.AtDocumentStart, true);
    }

    static string? GetFrameOrigin(WKFrameInfo frame)
    {
        WKSecurityOrigin origin = frame.SecurityOrigin;
        if (origin.Protocol is not ("http" or "https") || string.IsNullOrEmpty(origin.Host)) return null;
        return new UriBuilder(origin.Protocol, origin.Host, origin.Port > 0 ? (int)origin.Port : -1)
            .Uri.GetLeftPart(UriPartial.Authority);
    }
}
#endif
