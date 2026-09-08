using System;

namespace MinecraftClient.Scripting
{
    /// <summary>
    /// Metadata for a C# chat bot plugin loaded from the 'plugins' folder.
    /// Declare it on the plugin's main bot class, e.g.:
    /// [MCCPlugin("MyBot", "1.0.0", "AuthorName")]
    /// class MyBot : ChatBot { ... }
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MCCPluginAttribute : Attribute
    {
        /// <summary>Plugin display name, also used for the config file name (plugins/&lt;Name&gt;.yml)</summary>
        public string Name { get; }

        /// <summary>Plugin version</summary>
        public string Version { get; }

        /// <summary>Optional plugin author</summary>
        public string? Author { get; }

        public MCCPluginAttribute(string name, string version, string? author = null)
        {
            Name = name;
            Version = version;
            Author = author;
        }
    }
}
