using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace mem0.NET;

[ExcludeFromCodeCoverage]
public  static class JsonOptionsCache
{
    /// <summary>Singleton for <see cref="ReadOnlyMemoryConverter"/>.</summary>
    public static ReadOnlyMemoryConverter ReadOnlyMemoryConverter { get; } = new();

    /// <summary>
    /// Cached <see cref="JsonSerializerOptions"/> instance for reading and writing JSON using the default settings.
    /// </summary>
    public static JsonSerializerOptions Default { get; } = new()
    {
        Converters = { ReadOnlyMemoryConverter },
    };

    /// <summary>
    /// Cached <see cref="JsonSerializerOptions"/> instance for writing JSON with indentation.
    /// </summary>
    public static JsonSerializerOptions WriteIndented { get; } = new()
    {
        WriteIndented = true,
        Converters = { ReadOnlyMemoryConverter },
    };

    /// <summary>
    /// Cached <see cref="JsonSerializerOptions"/> instance for reading JSON in a permissive way,
    /// including support for trailing commas, case-insensitive property names, and comments.
    /// </summary>
    public static JsonSerializerOptions ReadPermissive { get; } = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { ReadOnlyMemoryConverter },
    };
}
