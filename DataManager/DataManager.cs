using System;
using System.IO;
using ConfigurationManager.Parsers;
using DataManager.Config;
using ServiceLayer.API;

namespace DataManager
{
    public class DataManager
    {
        private readonly DataConfig config;

        public DataManager()
        {
            var info = new DirectoryInfo(Environment.CurrentDirectory);
            var path = "C:\\Workspace\\Study\\ISP\\DatabaseLab\\Explorer\\appsettings.json";
            config = DataConfig.GetConfiguration(path);
            Repository = new Database(config.ConnectionString, config.Type);
            Repository.CalculateSummary();
        }

        public DataManager(DataConfig cfg)
        {
            var info = new DirectoryInfo(Environment.CurrentDirectory);
            var path = "C:\\Workspace\\Study\\ISP\\DatabaseLab\\Explorer\\appsettings.json";
            config = cfg;
            cfg.SetConfiguration(path);
            Repository = new Database(config.ConnectionString, config.Type);
            Repository.CalculateSummary();
        }

        public IRepository Repository { get; }

        public void MakeLog()
        {
            ISerrializer serrializer = new XmlParser();
            var s = serrializer.Serialize(Repository.DetailedSummaries);
            var pth = $"{config.TargetPath}\\file_{DateTime.Now.Date.ToString("MM.dd.yyyy").Replace(':', '-')}.xml";
            using (var sw = new StreamWriter(pth))
            {
                sw.Write(s);
            }
        }
    }
}