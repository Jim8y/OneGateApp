namespace NeoOrder.OneGate.Controls.Behaviors;

partial class FullScreenBehavior : Behavior<Image>
{
    readonly TapGestureRecognizer tap = new();

    public FullScreenBehavior()
    {
        tap.Tapped += OnTapped;
    }

    protected override void OnAttachedTo(Image image)
    {
        image.GestureRecognizers.Add(tap);
    }

    protected override void OnDetachingFrom(Image image)
    {
        image.GestureRecognizers.Remove(tap);
    }

    async void OnTapped(object? sender, TappedEventArgs e)
    {
        Image image = (Image)sender!;
        await Shell.Current.GoToAsync("full", false, new Dictionary<string, object>
        {
            ["source"] = image.Source switch
            {
                FileImageSource source => new FileImageSource
                {
                    File = source.File
                },
                FontImageSource source => new FontImageSource
                {
                    Color = source.Color,
                    FontAutoScalingEnabled = source.FontAutoScalingEnabled,
                    FontFamily = source.FontFamily,
                    Glyph = source.Glyph,
                    Size = source.Size
                },
                UriImageSource source => new UriImageSource
                {
                    CacheValidity = source.CacheValidity,
                    CachingEnabled = source.CachingEnabled,
                    Uri = source.Uri
                },
                _ => throw new NotSupportedException("Unsupported image source type.")
            }
        });
    }
}
