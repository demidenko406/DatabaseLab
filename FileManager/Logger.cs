using System;
using System.Globalization;
using System.IO;
using System.Threading;
using ConfigurationManager.Parsers;

namespace FileManager
{
    public class Logger
    {
        private AppConfiguration _configuration;

        private readonly object control = new object();

        private bool enabled = true;

        private readonly string path;

        private readonly FileSystemWatcher watcher;

        public Logger()
        {
            path = "C:\\Workspace\\Study\\ISP\\DatabaseLab\\FileManager\\appsettings.json";
            _configuration = FileOperations.GetConfiguration(path);
            watcher = new FileSystemWatcher(_configuration.Source);
            watcher.NotifyFilter = NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName
                                   | NotifyFilters.DirectoryName;
            //Load cfg
            IDeserializer deserializer = new JsonParser();
            //!Load cfg
            watcher.Filter = "*.xml";
            watcher.Created += FileTransfer;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                _configuration = FileOperations.GetConfiguration(path);
                Thread.Sleep(1000);
            }
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }

        private void FileTransfer(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000);
            lock (control)
            {
                var dirInfo = new DirectoryInfo(_configuration.Target);
                var filePath = Path.Combine(_configuration.Source, e.Name);
                var fileName = e.Name;
                var dt = DateTime.Now;
                var subPath = $"{dt.ToString("yyyy", DateTimeFormatInfo.InvariantInfo)}\\" +
                              $"{dt.ToString("MM", DateTimeFormatInfo.InvariantInfo)}\\" +
                              $"{dt.ToString("dd", DateTimeFormatInfo.InvariantInfo)}";
                var newPath = $"{_configuration.Target}" + "\\" +
                              $"{dt.ToString("yyyy", DateTimeFormatInfo.InvariantInfo)}\\" +
                              $"{dt.ToString("MM", DateTimeFormatInfo.InvariantInfo)}\\" +
                              $"{dt.ToString("dd", DateTimeFormatInfo.InvariantInfo)}\\" +
                              $"{Path.GetFileNameWithoutExtension(fileName)}_" +
                              $"{dt.ToString(@"yyyy_MM_dd_HH_mm_ss", DateTimeFormatInfo.InvariantInfo)}" +
                              $"{Path.GetExtension(fileName)}";

                if (!dirInfo.Exists) dirInfo.Create();

                dirInfo.CreateSubdirectory(subPath);
                File.Move(filePath, newPath);
                FileOperations.EncryptFile(newPath, newPath);
                var compressedPath = Path.ChangeExtension(newPath, "gz");
                var newCompressedPath = Path.Combine($"{_configuration.Target}", Path.GetFileName(compressedPath));
                var decompressedPath = Path.ChangeExtension(newCompressedPath, "xml");
                FileOperations.Compress(newPath, compressedPath);
                File.Move(compressedPath, newCompressedPath);
                FileOperations.Decompress(newCompressedPath, decompressedPath);
                FileOperations.DecryptFile(decompressedPath, decompressedPath);
                //FileOperations.AddToArchive(decompressedPath);
                //File.Delete(newPath);
                //File.Delete(newCompressedPath);
                //File.Delete(decompressedPath);
            }
        }
    }
}