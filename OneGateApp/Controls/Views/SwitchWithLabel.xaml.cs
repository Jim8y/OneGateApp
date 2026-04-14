namespace NeoOrder.OneGate.Controls.Views;

public partial class SwitchWithLabel : ContentView
{
    public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(SwitchWithLabel));
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(SwitchWithLabel));

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public SwitchWithLabel()
    {
        InitializeComponent();
    }

    void OnLabelTapped(object sender, TappedEventArgs e)
    {
        IsToggled = !IsToggled;
    }
}
