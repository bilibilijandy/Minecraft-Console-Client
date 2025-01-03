using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MinecraftClient;

public static class YamlHelper
{
    private static ISerializer _serializer;
    private static IDeserializer _deserializer;

    static YamlHelper()
    {
        _serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        _deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
    }

    public static string Serialize(object target)
    {
        return _serializer.Serialize(target);
    }

    public static void SerializeToFile(object target, string filePath)
    {
        var content = Serialize(target);
        File.WriteAllText(filePath, content, Encoding.UTF8);
    }

    public static T Deserialize<T>(string yaml)
    {
        return _deserializer.Deserialize<T>(yaml);
    }

    public static T DeserializeFromFile<T>(string filePath)
    {
        var yaml = File.ReadAllText(filePath, Encoding.UTF8);
        return Deserialize<T>(yaml);
    }
}