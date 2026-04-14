using CommunityToolkit.Maui.Alerts;

namespace NeoOrder.OneGate.Controls;

public static class Extensions
{
    extension(Element element)
    {
        public T FindAncestor<T>() where T : Element
        {
            Element ancestor = element.Parent;
            while (true)
            {
                if (ancestor is T result)
                    return result;
                ancestor = ancestor.Parent;
            }
        }
    }

    extension(Page page)
    {
        public async Task GoBackOrCloseAsync()
        {
            Window window = page.GetParentWindow();
            if (window.Page is AppShell shell)
                await shell.GoToAsync("..");
            else
                Application.Current!.CloseWindow(window);
        }
    }

    extension(Toast)
    {
        public static Task Show(string message)
        {
            return Toast.Make(message).Show();
        }
    }
}
