using NeoOrder.OneGate.Properties;

namespace NeoOrder.OneGate.Controls.Popups;

public partial class ConfirmationPopup : MyPopup<bool>
{
    public required string Title { get; set { field = value; OnPropertyChanged(); } }
    public required string Message { get; set { field = value; OnPropertyChanged(); } }
    public string AcceptText { get; set { field = value; OnPropertyChanged(); } } = Strings.OK;
    public bool IsDanger { get; set { field = value; OnPropertyChanged(); } }

    public ConfirmationPopup()
    {
        InitializeComponent();
    }

    async void OnAccept(object sender, EventArgs e)
    {
        await CloseAsync(true);
    }

    async void OnCancel(object sender, EventArgs e)
    {
        await CloseAsync(false);
    }
}
