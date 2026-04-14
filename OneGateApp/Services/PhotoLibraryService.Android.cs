#if ANDROID

using Android.Content;
using Android.Provider;
using CommunityToolkit.Maui.Alerts;
using NeoOrder.OneGate.Controls;
using NeoOrder.OneGate.Properties;

namespace NeoOrder.OneGate.Services;

partial class PhotoLibraryService
{
    public static partial async Task SaveAsync(IScreenshotResult screenshot, string fileName)
    {
        var context = Platform.AppContext;
        var values = new ContentValues();
        values.Put(MediaStore.IMediaColumns.DisplayName, fileName);
        values.Put(MediaStore.IMediaColumns.MimeType, "image/png");
        values.Put(MediaStore.IMediaColumns.RelativePath, "Pictures/OneGate");
        var uri = context.ContentResolver!.Insert(MediaStore.Images.Media.ExternalContentUri!, values)!;
        using var stream = context.ContentResolver.OpenOutputStream(uri)!;
        await screenshot.CopyToAsync(stream);
        await Toast.Show(Strings.QRCodeSaved);
    }
}
#endif
