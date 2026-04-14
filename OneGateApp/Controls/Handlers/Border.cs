using Microsoft.Maui.Handlers;

namespace NeoOrder.OneGate.Controls.Handlers;

public static class Border
{
    public static readonly BindableProperty IsVisibleProperty = BindableProperty.CreateAttached("IsVisible", typeof(bool), typeof(Border), true);

    public static bool GetIsVisible(InputView view)
    {
        return (bool)view.GetValue(IsVisibleProperty);
    }

    public static void SetIsVisible(InputView view, bool value)
    {
        view.SetValue(IsVisibleProperty, value);
    }

    public static void ConfigureHandlers()
    {
        EditorHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
        {
            if (view is not Editor editor) return;
            if (GetIsVisible(editor)) return;
#if ANDROID
            handler.PlatformView.Background = null;
#elif IOS || MACCATALYST
            handler.PlatformView.BorderStyle = UIKit.UITextViewBorderStyle.None;
#elif WINDOWS
            handler.PlatformView.BorderThickness = new(0);
#endif
        });
        EntryHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
        {
            if (view is not Entry entry) return;
            if (GetIsVisible(entry)) return;
#if ANDROID
            handler.PlatformView.Background = null;
#elif IOS || MACCATALYST
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif WINDOWS
            handler.PlatformView.BorderThickness = new(0);
#endif
        });
    }
}
