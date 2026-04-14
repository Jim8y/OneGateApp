using NeoOrder.OneGate.Pages;
using NeoOrder.OneGate.Services;

namespace NeoOrder.OneGate.Models.AppLinks;

class ViewNewsAction(Uri uri) : AppLinkAction
{
    protected override string Route => "//home/news/details";
    public int NewsId { get; } = int.Parse(uri.Segments[2]);

    protected override Page CreatePage(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetServiceOrCreateInstance<NewsDetailsPage>();
    }

    protected override IDictionary<string, object> CreateQuery()
    {
        return new Dictionary<string, object>
        {
            ["uri"] = uri
        };
    }
}
