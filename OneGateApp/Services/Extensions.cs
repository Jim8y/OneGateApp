namespace NeoOrder.OneGate.Services;

public static class Extensions
{
    extension(IServiceProvider serviceProvider)
    {
        public T GetServiceOrCreateInstance<T>()
        {
            return ActivatorUtilities.GetServiceOrCreateInstance<T>(serviceProvider);
        }
    }
}
