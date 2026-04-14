#if IOS || MACCATALYST

using CommunityToolkit.Maui.Alerts;
using Foundation;
using NeoOrder.OneGate.Controls;
using NeoOrder.OneGate.Properties;
using Photos;

namespace NeoOrder.OneGate.Services;

partial class PhotoLibraryService
{
    public static partial async Task SaveAsync(IScreenshotResult screenshot, string fileName)
    {
        using var stream = await screenshot.OpenReadAsync();
        NSData nsData = NSData.FromStream(stream)!;
        var status = PHPhotoLibrary.GetAuthorizationStatus(PHAccessLevel.AddOnly);
        if (status == PHAuthorizationStatus.NotDetermined)
            status = await PHPhotoLibrary.RequestAuthorizationAsync(PHAccessLevel.AddOnly);
        if (status != PHAuthorizationStatus.Authorized && status != PHAuthorizationStatus.Limited)
        {
            await Toast.Show(Strings.PermissionDenied);
            return;
        }
        PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
        {
            var creationRequest = PHAssetCreationRequest.CreationRequestForAsset();
            var options = new PHAssetResourceCreationOptions
            {
                OriginalFilename = fileName
            };
            creationRequest.AddResource(PHAssetResourceType.Photo, nsData, options);
        }, null);
        await Toast.Show(Strings.QRCodeSaved);
    }
}
#endif
