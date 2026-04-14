using Microsoft.EntityFrameworkCore;
using NeoOrder.OneGate.Data;
using NeoOrder.OneGate.Services;
using Contact = NeoOrder.OneGate.Data.Contact;

namespace NeoOrder.OneGate.Pages;

public partial class ContactsPage : ContentPage
{
    readonly ApplicationDbContext dbContext;

    public LoadingService LoadingService { get; set { field = value; OnPropertyChanged(); } }
    public Contact[]? Contacts { get; set { field = value; OnPropertyChanged(); } }

    public ContactsPage(ApplicationDbContext dbContext)
    {
        this.LoadingService = new(LoadContactsAsync);
        this.dbContext = dbContext;
        InitializeComponent();
        LoadingService.BeginLoad();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (this.ShouldRefresh())
            LoadingService.BeginLoad();
    }

    async void OnContactTapped(object sender, TappedEventArgs e)
    {
        Contact contact = (Contact)e.Parameter!;
        await Shell.Current.GoToAsync("edit", new Dictionary<string, object> { ["contact"] = contact });
    }

    async Task LoadContactsAsync()
    {
        Contacts = await dbContext.Contacts.ToArrayAsync();
    }
}
