namespace NeoOrder.OneGate.Controls.Views;

class TypeNameTemplateSelector : DataTemplateSelector
{
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (container is not VisualElement element) throw new InvalidOperationException();
        string key = item.GetType().Name;
        while (true)
        {
            if (element.Resources.TryGetValue(key, out var template))
                return (DataTemplate)template;
            element = element.Parent as VisualElement ?? throw new KeyNotFoundException();
        }
    }
}
