using System.Globalization;

namespace NeoOrder.OneGate.Data;

static class Extensions
{
    extension(Dictionary<string, string> localizer)
    {
        public string? Localize(string? lang = null)
        {
            if (localizer.Count == 0) return null;
            if (localizer.Count == 1) return localizer.First().Value;
            string? result;
            CultureInfo culture = lang is null ? CultureInfo.CurrentUICulture : new(lang);
            do
            {
                if (localizer.TryGetValue(culture.Name, out result)) return result;
                culture = culture.Parent;
            } while (!culture.Equals(CultureInfo.InvariantCulture));
            if (localizer.TryGetValue("en", out result)) return result;
            return localizer.First().Value;
        }
    }
}
