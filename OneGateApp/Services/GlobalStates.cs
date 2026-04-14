namespace NeoOrder.OneGate.Services;

static class GlobalStates
{
    static readonly Dictionary<Type, Dictionary<string, object>> states = new();

    static void Set(Type type, string key, object value)
    {
        if (!states.TryGetValue(type, out var pageStates))
        {
            pageStates = new Dictionary<string, object>();
            states.Add(type, pageStates);
        }
        pageStates[key] = value;
    }

    static T? GetAndRemove<T>(Type type, string key) where T : notnull
    {
        if (!states.TryGetValue(type, out var pageStates)) return default;
        if (!pageStates.Remove(key, out var value)) return default;
        return (T)value;
    }

    public static void Invalidate<T>() where T : Page
    {
        Set(typeof(T), "invalid", true);
    }

    public static bool ShouldRefresh(this Page page)
    {
        return GetAndRemove<bool>(page.GetType(), "invalid");
    }

    public static void SetReturnPage<T>(string route) where T : Page
    {
        Set(typeof(T), "return", route);
    }

    public static async Task GoBackAsync(this Page page)
    {
        string? route = GetAndRemove<string>(page.GetType(), "return");
        route ??= "..";
        await Shell.Current.GoToAsync(route);
    }
}
