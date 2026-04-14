using NeoOrder.OneGate.Controls;
using NeoOrder.OneGate.Data;
using System.Net;
using System.Net.Http.Json;

namespace NeoOrder.OneGate.Pages;

public partial class NewsDetailsPage : ContentPage, IQueryAttributable
{
    readonly HttpClient httpClient;

    public NewsDetailsPage(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        InitializeComponent();
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("news", out var value))
        {
            BindingContext = value;
        }
        else
        {
            Uri uri = query["uri"] as Uri ?? new(WebUtility.UrlDecode((string)query["uri"]));
            int id = int.Parse(uri.Segments[2]);
            var response = await httpClient.GetAsync($"/api/news/{id}");
            if (response.IsSuccessStatusCode)
                BindingContext = (await response.Content.ReadFromJsonAsync<News>())!;
            else
                await this.GoBackOrCloseAsync();
        }
    }
}
