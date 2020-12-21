using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using ConfigurationManager.Parsers;

namespace FileManager
{
    public static class FileOperations
    {
        public static void EncryptFile(string inputFile, string outputFile)
        {
            var password = @"myKey123";
            var UE = new UnicodeEncoding();
            var key = UE.GetBytes(password);
            var arr = File.ReadAllBytes(inputFile);

            var cryptFile = outputFile;
            using (var fsCrypt = new FileStream(cryptFile, FileMode.Create))
            {
                var RMCrypto = new RijndaelManaged();

                using (var cs = new CryptoStream(fsCrypt, RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write))
                {
                    foreach (var bt in arr) cs.WriteByte(bt);
                }
            }
        }

        public static void DecryptFile(string inputFile, string outputFile)
        {
            {
                var password = @"myKey123";

                var UE = new UnicodeEncoding();
                var key = UE.GetBytes(password);
                var bt = new List<byte>();

                using (var fsCrypt = new FileStream(inputFile, FileMode.Open))
                {
                    var RMCrypto = new RijndaelManaged();

                    using (var cs = new CryptoStream(fsCrypt, RMCrypto.CreateDecryptor(key, key),
                        CryptoStreamMode.Read))
                    {
                        int data;
                        while ((data = cs.ReadByte()) != -1) bt.Add((byte) data);
                    }
                }

                using (var fsOut = new FileStream(outputFile, FileMode.Create))
                {
                    foreach (var b in bt) fsOut.WriteByte(b);
                }
            }
        }

        public static void Compress(string sourceFile, string compressedFile)
        {
            using (var sourceStream = new FileStream(sourceFile, FileMode.OpenOrCreate))
            {
                using (var targetStream = File.Create(compressedFile))
                {
                    using (var compressionStream = new GZipStream(targetStream, CompressionMode.Compress))
                    {
                        sourceStream.CopyTo(compressionStream);
                    }
                }
            }
        }

        public static void Decompress(string compressedFile, string targetFile)
        {
            using (var sourceStream = new FileStream(compressedFile, FileMode.OpenOrCreate))
            {
                using (var targetStream = File.Create(targetFile))
                {
                    using (var decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(targetStream);
                        Console.WriteLine("Восстановлен файл: {0}", targetFile);
                    }
                }
            }
        }

        public static void AddToArchive(string filePath)
        {
            var archivePath = "C:\\TargetDirectory\\archive.zip";

            using (var zipArchive = ZipFile.Open(archivePath, ZipArchiveMode.Update))
            {
                zipArchive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
            }
        }

        public static AppConfiguration GetConfiguration(string src)
        {
            using (var file = File.Open(src, FileMode.Open))
            {
                var reader = new StreamReader(file);
                var xml = reader.ReadToEnd();
                reader.Close();
                IDeserializer deserializer = new JsonParser();
                var configuration = deserializer.Deserialize<AppConfiguration>(xml);
                return configuration;
            }
        }

        public static void SetConfiguration(string src, AppConfiguration configuration)
        {
            using (var file = File.Open(src, FileMode.Create))
            {
                var writer = new StreamWriter(file);
                ISerrializer serrializer = new JsonParser();
                var xml = serrializer.Serialize(configuration);
                writer.Write(xml);
                writer.Close();
            }
        }
    }
}