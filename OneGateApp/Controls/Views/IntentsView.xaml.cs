using NeoOrder.OneGate.Models.Intents;

namespace NeoOrder.OneGate.Controls.Views;

public partial class IntentsView : ContentView
{
    public static readonly BindableProperty IntentsProperty = BindableProperty.Create(nameof(Intents), typeof(IEnumerable<TransactionIntent>), typeof(IntentsView));

    public IEnumerable<TransactionIntent> Intents
    {
        get => (IEnumerable<TransactionIntent>)GetValue(IntentsProperty);
        set => SetValue(IntentsProperty, value);
    }

    public IntentsView()
    {
        InitializeComponent();
    }
}
