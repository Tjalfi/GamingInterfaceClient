using System;
using System.IO;
using System.IO.Compression;

namespace GamingInterfaceClient.Utils
{
    public static class ZipHelper
    {
        public static void Zip(string[] filesToZip, string zipFileName)
        {
            if (File.Exists(zipFileName))
            {
                File.Delete(zipFileName);
            }
            ZipArchive zipFile = ZipFile.Open(zipFileName, ZipArchiveMode.Create);
            foreach (string fileToZip in filesToZip)
            {
                zipFile.CreateEntryFromFile(fileToZip, fileToZip.Substring(fileToZip.LastIndexOfAny(new char[2] { '/', '\\' }) + 1));
            }
        }

        public static bool UnZip(string zipFile, string destinationDir)
        {
            using (ZipArchive archive = ZipFile.OpenRead(zipFile))
            {
                bool validZip = false;
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName == "data.json" || entry.FullName.EndsWith(Path.PathSeparator + "data.json"))
                    {
                        validZip = true;
                    }
                }
                if (!validZip)
                {
                    return false;
                }

                if (!Directory.Exists(destinationDir))
                {
                    if (File.Exists(destinationDir))
                    {
                        throw new IOException("Destination path exists but is no directory");
                    }
                    Directory.CreateDirectory(destinationDir);
                }

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string destinationPath = Path.GetFullPath(Path.Combine(destinationDir, entry.FullName));

                    if (destinationPath.StartsWith(destinationDir, StringComparison.Ordinal))
                    {
                        entry.ExtractToFile(destinationPath);
                    }
                }
            }

            return true;
        }
    }
}
