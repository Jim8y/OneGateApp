using System.Collections;

namespace NeoOrder.OneGate.Controls.Views;

public partial class SettingsView : ContentView
{
    public static readonly BindableProperty SettingsProperty = BindableProperty.Create(nameof(Settings), typeof(IEnumerable), typeof(SettingsView));

    public IEnumerable? Settings
    {
        get => (IEnumerable)GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    public bool IsGrouped { get; set { field = value; OnPropertyChanged(); } }

    public SettingsView()
    {
        InitializeComponent();
    }
}
