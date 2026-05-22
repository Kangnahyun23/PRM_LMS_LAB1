using System.Reflection;

namespace PRN232.LMS.API.Infrastructure;

public static class FieldSelector
{
    public static IEnumerable<object> Shape<T>(IEnumerable<T> items, string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
        {
            return items.Cast<object>();
        }

        var selected = fields
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToArray();

        if (selected.Length == 0)
        {
            return items.Cast<object>();
        }

        var map = typeof(T)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        var props = selected
            .Where(map.ContainsKey)
            .Select(s => map[s])
            .ToArray();

        return items.Select(item =>
        {
            var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in props)
            {
                dict[p.Name] = p.GetValue(item);
            }
            return (object)dict;
        });
    }
}

