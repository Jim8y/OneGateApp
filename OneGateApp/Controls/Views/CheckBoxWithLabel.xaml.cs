namespace NeoOrder.OneGate.Controls.Views;

public partial class CheckBoxWithLabel : ContentView
{
    public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CheckBoxWithLabel));
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(CheckBoxWithLabel));

    public event EventHandler<CheckedChangedEventArgs>? CheckedChanged;

    public bool IsChecked
    {
        get => (bool)GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public CheckBoxWithLabel()
    {
        InitializeComponent();
    }

    void OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckedChanged?.Invoke(this, e);
    }

    void OnLabelTapped(object sender, TappedEventArgs e)
    {
        IsChecked = !IsChecked;
    }
}
