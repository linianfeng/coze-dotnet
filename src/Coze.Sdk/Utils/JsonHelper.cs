using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Coze.Sdk.Utils;

/// <summary>
/// JSON 序列化辅助类。
/// 对应 Java SDK 中的 ObjectMapper 用法。
/// </summary>
internal static class JsonHelper
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        Converters = { new SystemTextJsonElementConverter() }
    };

    private static readonly JsonSerializerSettings CamelCaseSettings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        Converters = { new SystemTextJsonElementConverter() }
    };

    /// <summary>
    /// 将 System.Text.Json.JsonElement 转换为原始值。
    /// 用于处理 ASP.NET Core 使用 System.Text.Json 反序列化后的 Dictionary&lt;string, object?&gt;。
    /// </summary>
    private static object? ConvertJsonElement(object? value)
    {
        return value switch
        {
            JsonElement element => element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                JsonValueKind.Object => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object?>>(element.GetRawText()),
                JsonValueKind.Array => System.Text.Json.JsonSerializer.Deserialize<List<object?>>(element.GetRawText()),
                _ => element.GetRawText()
            },
            IDictionary<string, object?> dict => dict.ToDictionary(kvp => kvp.Key, kvp => ConvertJsonElement(kvp.Value)),
            IList<object?> list => list.Select(ConvertJsonElement).ToList(),
            _ => value
        };
    }

    /// <summary>
    /// 递归转换字典中的所有 JsonElement 为原始值。
    /// </summary>
    public static Dictionary<string, object?>? ConvertParameters(IReadOnlyDictionary<string, object?>? parameters)
    {
        if (parameters == null) return null;

        var result = new Dictionary<string, object?>();
        foreach (var kvp in parameters)
        {
            result[kvp.Key] = ConvertJsonElement(kvp.Value);
        }
        return result;
    }

    /// <summary>
    /// 将单个 JsonElement 转换为原始值（公共方法）。
    /// </summary>
    public static object? ConvertJsonElementToObject(JsonElement element)
    {
        return ConvertJsonElement(element);
    }

    /// <summary>
    /// 将对象序列化为 JSON 字符串。
    /// </summary>
    /// <param name="obj">要序列化的对象。</param>
    /// <returns>JSON 字符串。</returns>
    public static string SerializeObject(object? obj)
    {
        return JsonConvert.SerializeObject(obj, Settings);
    }

    /// <summary>
    /// 将对象序列化为 camelCase 命名的 JSON 字符串。
    /// </summary>
    /// <param name="obj">要序列化的对象。</param>
    /// <returns>JSON 字符串。</returns>
    public static string SerializeObjectCamelCase(object? obj)
    {
        return JsonConvert.SerializeObject(obj, CamelCaseSettings);
    }

    /// <summary>
    /// 将 JSON 字符串反序列化为对象。
    /// </summary>
    /// <typeparam name="T">要反序列化的类型。</typeparam>
    /// <param name="json">JSON 字符串。</param>
    /// <returns>反序列化后的对象。</returns>
    public static T? DeserializeObject<T>(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }
        return JsonConvert.DeserializeObject<T>(json, Settings);
    }

    /// <summary>
    /// 将 camelCase 命名的 JSON 字符串反序列化为对象。
    /// </summary>
    /// <typeparam name="T">要反序列化的类型。</typeparam>
    /// <param name="json">JSON 字符串。</param>
    /// <returns>反序列化后的对象。</returns>
    public static T? DeserializeObjectCamelCase<T>(string? json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return default;
        }
        return JsonConvert.DeserializeObject<T>(json, CamelCaseSettings);
    }
}

/// <summary>
/// Newtonsoft.Json 转换器，用于处理 System.Text.Json.JsonElement。
/// 解决 ASP.NET Core 使用 System.Text.Json 反序列化后与 Newtonsoft.Json 序列化的兼容性问题。
/// </summary>
internal class SystemTextJsonElementConverter : Newtonsoft.Json.JsonConverter<JsonElement>
{
    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, JsonElement value, Newtonsoft.Json.JsonSerializer serializer)
    {
        // 将 JsonElement 的原始 JSON 写入
        writer.WriteRawValue(value.GetRawText());
    }

    public override JsonElement ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, JsonElement existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        // 读取 JSON 并解析为 JsonElement
        var json = reader.Value?.ToString() ?? string.Empty;
        using var document = System.Text.Json.JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
