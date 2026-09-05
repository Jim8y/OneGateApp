#if ANDROID

using Android.Webkit;
using Android.Widget;
using AndroidX.Core.View;
using AndroidX.WebKit;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace NeoOrder.OneGate.Controls.Handlers;

partial class BridgeWebViewHandler
{
    const string NativeBridgeName = "__OneGateNativeBridge";
    const string SyncPrompt = "__OneGateBridgeSync";
    readonly string syncToken = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
    ScriptHandler? scriptHandler;

    class ScriptHandler(BridgeWebViewHandler handler) : Java.Lang.Object, WebViewCompat.IWebMessageListener
    {
        public void OnPostMessage(Android.Webkit.WebView? view, WebMessageCompat? message, Android.Net.Uri? sourceOrigin, bool isMainFrame, JavaScriptReplyProxy? replyProxy)
        {
            if (!isMainFrame || message?.Type != WebMessageCompat.TypeString) return;
            if (message?.Data is string payload)
                handler.BridgeWebView.OnMessage(payload, sourceOrigin?.ToString(), view?.Url, isMainFrame);
        }
    }

    class BridgeWebChromeClient(BridgeWebViewHandler handler) : MauiWebChromeClient(handler)
    {
        static readonly string[] VideoCaptureResources = [PermissionRequest.ResourceVideoCapture];
        Android.Views.View? fullscreenView;
        ICustomViewCallback? fullscreenCallback;
        WindowInsetsControllerCompat? fullscreenInsetsController;
        bool wereSystemBarsVisible;

        public override bool OnJsPrompt(Android.Webkit.WebView? view, string? url, string? message, string? defaultValue, JsPromptResult? result)
        {
            if (message?.StartsWith(SyncPrompt, StringComparison.Ordinal) != true)
                return base.OnJsPrompt(view, url, message, defaultValue, result);
            // Android prompts do not expose isMainFrame. Only the main-frame injection
            // receives this private capability; the raw prompt remains origin-checked.
            string? token = message.StartsWith(SyncPrompt + ":", StringComparison.Ordinal) ? message[(SyncPrompt.Length + 1)..] : null;
            if (!handler.BridgeWebView.IsAuthorizedSyncSource(token, handler.syncToken, url, view?.Url))
            {
                result?.Cancel();
                return true;
            }
            result?.Confirm(handler.BridgeWebView.OnSyncMessage(defaultValue ?? string.Empty, url, view?.Url, true));
            return true;
        }

        public override void OnShowCustomView(Android.Views.View? view, ICustomViewCallback? callback)
        {
            if (fullscreenView is not null)
            {
                OnHideCustomView();
                return;
            }

            if (view is null || callback is null)
                return;

            var activity = handler.MauiContext?.Context?.GetActivity() ?? Platform.CurrentActivity;
            if (activity is null)
                return;

            fullscreenCallback = callback;
            fullscreenView = view;
            wereSystemBarsVisible = false;
            fullscreenView.SetBackgroundColor(Android.Graphics.Color.White);

            if (OperatingSystem.IsAndroidVersionAtLeast(30))
            {
                WindowCompat.SetDecorFitsSystemWindows(activity.Window!, false);
                var controller = activity.Window!.InsetsController;
                if (controller is not null)
                {
                    var windowInsets = activity.Window.DecorView.RootWindowInsets;
                    wereSystemBarsVisible = windowInsets is null
                        || windowInsets.IsVisible(WindowInsetsCompat.Type.NavigationBars())
                        || windowInsets.IsVisible(WindowInsetsCompat.Type.StatusBars());
                    if (wereSystemBarsVisible)
                    {
                        controller.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
                        controller.Hide(WindowInsetsCompat.Type.SystemBars());
                    }
                }
            }
            else if (OperatingSystem.IsAndroidVersionAtLeast(19))
            {
                var decorView = activity.Window!.DecorView;
                fullscreenInsetsController = WindowCompat.GetInsetsController(activity.Window, decorView);
                wereSystemBarsVisible = true;
                if (fullscreenInsetsController is not null)
                {
                    fullscreenInsetsController.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
                    fullscreenInsetsController.Hide(WindowInsetsCompat.Type.SystemBars());
                }
                WindowCompat.SetDecorFitsSystemWindows(activity.Window, false);
            }

            if (activity.Window!.DecorView is FrameLayout layout)
            {
                layout.AddView(fullscreenView, new FrameLayout.LayoutParams(
                    Android.Views.ViewGroup.LayoutParams.MatchParent,
                    Android.Views.ViewGroup.LayoutParams.MatchParent));
            }
        }

        public override void OnHideCustomView()
        {
            if (fullscreenView is null)
                return;

            var activity = handler.MauiContext?.Context?.GetActivity() ?? Platform.CurrentActivity;
            if (activity is not null)
            {
                if (activity.Window!.DecorView is FrameLayout layout)
                    layout.RemoveView(fullscreenView);

                if (OperatingSystem.IsAndroidVersionAtLeast(30))
                {
                    WindowCompat.SetDecorFitsSystemWindows(activity.Window, true);
                    var controller = activity.Window.InsetsController;
                    if (controller is not null && wereSystemBarsVisible)
                        controller.Show(WindowInsetsCompat.Type.SystemBars());
                }
                else if (OperatingSystem.IsAndroidVersionAtLeast(19))
                {
                    if (fullscreenInsetsController is not null && wereSystemBarsVisible)
                        fullscreenInsetsController.Show(WindowInsetsCompat.Type.SystemBars());
                    WindowCompat.SetDecorFitsSystemWindows(activity.Window, true);
                }
            }

            fullscreenCallback?.OnCustomViewHidden();
            fullscreenView = null;
            fullscreenCallback = null;
            fullscreenInsetsController = null;
            wereSystemBarsVisible = false;
        }

        public override void OnPermissionRequest(PermissionRequest? request)
        {
            if (request is null)
                return;

            if (!IsCameraPermissionRequest(request) || !IsSameWebOrigin(request.Origin?.ToString(), handler.PlatformView.Url))
            {
                request.Deny();
                return;
            }

            _ = MainThread.InvokeOnMainThreadAsync(() => HandlePermissionRequestAsync(request));
        }

        static bool IsCameraPermissionRequest(PermissionRequest request)
        {
            string[]? resources = request.GetResources();
            return resources is { Length: > 0 }
                && resources.Contains(PermissionRequest.ResourceVideoCapture)
                && resources.All(resource => resource == PermissionRequest.ResourceVideoCapture);
        }

        static async Task HandlePermissionRequestAsync(PermissionRequest request)
        {
            try
            {
                if (!await ConfirmCameraPermissionAsync(request.Origin?.ToString()))
                {
                    request.Deny();
                    return;
                }

                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.Camera>();

                if (status == PermissionStatus.Granted)
                    request.Grant(VideoCaptureResources);
                else
                    request.Deny();
            }
            catch
            {
                request.Deny();
            }
        }
    }

    static partial void ConfigureMapper(PropertyMapper<IWebView, IWebViewHandler> mapper)
    {
        mapper[nameof(WebChromeClient)] = MapBridgeWebChromeClient;
    }

    static void MapBridgeWebChromeClient(IWebViewHandler handler, IWebView webView)
    {
        if (handler is BridgeWebViewHandler bridgeHandler)
            bridgeHandler.PlatformView.SetWebChromeClient(new BridgeWebChromeClient(bridgeHandler));
    }

    protected override void ConnectHandler(Android.Webkit.WebView platformView)
    {
        base.ConnectHandler(platformView);
        platformView.Settings.DomStorageEnabled = true;
        platformView.Settings.JavaScriptEnabled = true;
        platformView.Settings.MediaPlaybackRequiresUserGesture = false;
        if (WebViewFeature.IsFeatureSupported(WebViewFeature.DocumentStartScript)
            && WebViewFeature.IsFeatureSupported(WebViewFeature.WebMessageListener))
        {
            scriptHandler = new ScriptHandler(this);
            // Messages include sourceOrigin and isMainFrame, verified before dispatch.
            WebViewCompat.AddWebMessageListener(platformView, NativeBridgeName, ["*"], scriptHandler);
            string script = $$"""
                (function () {
                    if (window.top !== window) return;
                    const token = '{{syncToken}}';
                    window.__OneGateBridge = {
                        invoke: function(payload) { window.{{NativeBridgeName}}.postMessage(payload); },
                        invokeSync: function(payload) { return window.prompt('{{SyncPrompt}}:' + token, payload); }
                    };
                })();
                """ + Views.BridgeWebView.CreateRpcScript();
            if (!string.IsNullOrWhiteSpace(BridgeWebView.DocumentStartScript))
                script += BridgeWebView.DocumentStartScript;
            WebViewCompat.AddDocumentStartJavaScript(platformView, script, ["*"]);
        }
    }

    protected override void DisconnectHandler(Android.Webkit.WebView platformView)
    {
        if (scriptHandler is not null)
        {
            WebViewCompat.RemoveWebMessageListener(platformView, NativeBridgeName);
            scriptHandler.Dispose();
            scriptHandler = null;
        }
        base.DisconnectHandler(platformView);
    }
}
#endif
