using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Extensions;
using Microsoft.EntityFrameworkCore;
using NeoOrder.OneGate.Controls;
using NeoOrder.OneGate.Controls.Popups;
using NeoOrder.OneGate.Data;
using NeoOrder.OneGate.Properties;
using NeoOrder.OneGate.Services;
using Contact = NeoOrder.OneGate.Data.Contact;

namespace NeoOrder.OneGate.Pages;

public partial class EditContactPage : ContentPage, IQueryAttributable
{
    readonly IServiceProvider serviceProvider;
    readonly ApplicationDbContext dbContext;

    public required Contact Contact { get; set { field = value; OnPropertyChanged(); } }

    public EditContactPage(IServiceProvider serviceProvider, ApplicationDbContext dbContext)
    {
        this.serviceProvider = serviceProvider;
        this.dbContext = dbContext;
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Contact = (Contact)query["contact"];
    }

    async void OnSubmitted(object sender, EventArgs e)
    {
        string label = Contact.Label;
        await dbContext.Contacts
            .Where(p => p.Address == Contact.Address)
            .ExecuteUpdateAsync(builder => builder.SetProperty(p => p.Label, _ => label));
        await Toast.Show(Strings.ContactUpdatedSuccessfully);
        GlobalStates.Invalidate<ContactsPage>();
        GlobalStates.Invalidate<SettingsPage>();
        await Shell.Current.GoToAsync("..");
    }

    async void OnDelete(object sender, EventArgs e)
    {
        var popup = serviceProvider.GetServiceOrCreateInstance<ConfirmationPopup>();
        popup.Title = Strings.DeleteConfirmation;
        popup.Message = Strings.DeleteContactText;
        popup.AcceptText = Strings.Delete;
        popup.IsDanger = true;
        var result = await this.ShowPopupAsync<bool>(popup);
        if (!result.Result) return;
        await dbContext.Contacts.Where(p => p.Address == Contact.Address).ExecuteDeleteAsync();
        await Toast.Show(Strings.ContactDeletedSuccessfully);
        GlobalStates.Invalidate<ContactsPage>();
        GlobalStates.Invalidate<SettingsPage>();
        await Shell.Current.GoToAsync("..");
    }
}
