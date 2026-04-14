namespace NeoOrder.OneGate.Pages;

public partial class FullScreenImagePage : ContentPage, IQueryAttributable
{
    const double MinimumScale = 1.0;
    const double MaximumScale = 4.0;

    Point startScaleOrigin;
    double startScale = 1.0;
    double startTranslationX = 0.0;
    double startTranslationY = 0.0;

    public ImageSource? Source { get; set { field = value; OnPropertyChanged(); } }

    public FullScreenImagePage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Source = (ImageSource)query["source"];
    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("..", false);
        return true;
    }

    void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Started:
                startScaleOrigin = e.ScaleOrigin;
                startScale = image.Scale;
                startTranslationX = image.TranslationX;
                startTranslationY = image.TranslationY;
                image.AnchorX = 0;
                image.AnchorY = 0;
                break;
            case GestureStatus.Running:
                image.Scale = Math.Clamp(image.Scale * e.Scale, MinimumScale, MaximumScale);
                var renderedX = image.X + startTranslationX;
                var deltaX = renderedX / Width;
                var deltaWidth = Width / (image.Width * startScale);
                var originX = (startScaleOrigin.X - deltaX) * deltaWidth;
                var renderedY = image.Y + startTranslationY;
                var deltaY = renderedY / Height;
                var deltaHeight = Height / (image.Height * startScale);
                var originY = (startScaleOrigin.Y - deltaY) * deltaHeight;
                var targetX = startTranslationX - (originX * image.Width) * (image.Scale - startScale);
                var targetY = startTranslationY - (originY * image.Height) * (image.Scale - startScale);
                image.TranslationX = targetX;
                image.TranslationY = targetY;
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                if (image.Scale <= MinimumScale)
                    ResetImage();
                else
                    ClampTranslation();
                break;
        }
    }

    void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                startTranslationX = image.TranslationX;
                startTranslationY = image.TranslationY;
                break;
            case GestureStatus.Running:
                image.TranslationX = startTranslationX + e.TotalX;
                image.TranslationY = startTranslationY + e.TotalY;
                ClampTranslation();
                break;
        }
    }

    void ResetImage()
    {
        image.Scale = 1.0;
        image.TranslationX = 0;
        image.TranslationY = 0;
    }

    void ClampTranslation()
    {
        var minX = Math.Min(0, Width - image.Width * image.Scale);
        var minY = Math.Min(0, Height - image.Height * image.Scale);
        image.TranslationX = Math.Clamp(image.TranslationX, minX, 0);
        image.TranslationY = Math.Clamp(image.TranslationY, minY, 0);
    }
}
