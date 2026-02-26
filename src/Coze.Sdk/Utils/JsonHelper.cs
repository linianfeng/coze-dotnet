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
        MissingMemberHandling = MissingMemberHandling.Ignore
    };

    private static readonly JsonSerializerSettings CamelCaseSettings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        },
        NullValueHandling = NullValueHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore
    };

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
