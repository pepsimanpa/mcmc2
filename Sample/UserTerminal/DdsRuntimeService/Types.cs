using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DdsServiceRuntime
{
    internal class TypeLoader
    {
        public static List<Struct> LoadFromFile(string xmlPath)
        {
            if (string.IsNullOrWhiteSpace(xmlPath))
                throw new ArgumentException("xmlPath is required.", nameof(xmlPath));

            if (File.Exists(xmlPath) is false)
                throw new FileNotFoundException("XML is not found.", xmlPath);

            using var xmlFile = File.OpenRead(xmlPath);
            var serializer = new XmlSerializer(typeof(QosProfile));
            var dds = serializer.Deserialize(xmlFile) as QosProfile;
            
            string dir = Path.GetDirectoryName(xmlPath)!;

            List<Struct> result = new List<Struct>();
            foreach (var include in dds!.types.includes)
            {
                string fullPath = $"{dir}\\{include.file}";
                result.AddRange(LoadFromFile(fullPath));
            }

            foreach (var module in dds!.types.modules)
            {
                result.AddRange(LoadFromModule(module));
            }

            return result;
        }

        private static List<Struct> LoadFromModule(Module module, string parentModuleName = "")
        {
            List<Struct> result = new();

            string moduleName = module.name;
            if (!string.IsNullOrEmpty(parentModuleName))
                moduleName = $"{parentModuleName}::{module.name}";

            foreach(var childModule in module.modules)
            {
                result.AddRange(LoadFromModule(childModule, moduleName));
            }

            foreach (var @struct in module.structs)
            {
                @struct.moduleName = moduleName;
                result.Add(@struct);
            }

            return result;
        }
    }

    [XmlRoot("dds")]
    public record QosProfile
    {
        [XmlElement("types")]
        public Types types;
    }

    public record Types
    {
        [XmlElement("include", typeof(Include))]
        public List<Include> includes;

        [XmlElement("module", typeof(Module))]
        public List<Module> modules;
    }

    public record Include
    {
        [XmlAttribute("file")]
        public string file;
    }

    public record Module
    {
        [XmlAttribute("name")]
        public string name;

        [XmlElement("module", typeof(Module))]
        public List<Module> modules;

        [XmlElement("struct", typeof(Struct))]
        public List<Struct> structs;
    }

    public record Struct
    {
        public string moduleName;

        [XmlAttribute("name")]
        public string name;

        [XmlElement("member", typeof(Member))]
        public List<Member> members;
    }

    public record Member
    {
        [XmlAttribute("name")]
        public string name;

        [XmlAttribute("type")]
        public string type;
    }
}
