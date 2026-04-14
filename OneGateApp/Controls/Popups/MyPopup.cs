using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;

namespace NeoOrder.OneGate.Controls.Popups;

public partial class MyPopup<T> : Popup<T>
{
    public MyPopup()
    {
        CanBeDismissedByTappingOutsideOfPopup = false;
        this.SetAppThemeColor(BackgroundColorProperty, (AppThemeColor)Application.Current!.Resources["Accent"]);
    }
}
