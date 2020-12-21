using System.IO;
using ConfigurationManager.Parsers;
using DataAccess.Providers;

namespace DataManager.Config
{
    public class DataConfig
    {
        public ProviderType Type { get; set; }
        public string ConnectionString { get; set; }
        public string TargetPath { get; set; }

        public static DataConfig GetConfiguration(string src)
        {
            using (var file = File.Open(src, FileMode.Open))
            {
                var reader = new StreamReader(file);
                var json = reader.ReadToEnd();
                reader.Close();
                IDeserializer deserializer = new JsonParser();
                var configuration = deserializer.Deserialize<DataConfig>(json);
                return configuration;
            }
        }

        public void SetConfiguration(string src)
        {
            using (var file = File.Open(src, FileMode.Create))
            {
                var writer = new StreamWriter(file);
                ISerrializer serrializer = new JsonParser();
                var xml = serrializer.Serialize(this);
                writer.Write(xml);
                writer.Close();
            }
        }
    }
}