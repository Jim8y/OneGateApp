namespace NeoOrder.OneGate.Models.AppLinks;

abstract class AppLinkAction
{
    protected abstract string Route { get; }

    protected abstract Page CreatePage(IServiceProvider serviceProvider);

    protected virtual IDictionary<string, object>? CreateQuery() => null;

    public Page GetPage(IServiceProvider serviceProvider)
    {
        Page page = CreatePage(serviceProvider);
        if (page is IQueryAttributable attributable && CreateQuery() is IDictionary<string, object> query)
            attributable.ApplyQueryAttributes(query);
        return new NavigationPage(page);
    }

    public async void GotoRoute(AppShell shell)
    {
        if (CreateQuery() is IDictionary<string, object> query)
            await shell.GoToAsync(Route, query);
        else
            await shell.GoToAsync(Route);
    }
}
