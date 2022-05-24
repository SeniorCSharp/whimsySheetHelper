using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SheetHelper
{
    internal static class SaverLoader
    {
        public static string LocalPath => Directory.GetCurrentDirectory();

        public static string LoadLocalFile(string fileName)
        {
            if (FileExist(LocalPath + fileName))
            {
                return LoadFile(LocalPath + fileName);
            }

            return "";
        }

        private static string LoadFile(string fullPath)
        {
            return ""; //need to realize
        }

        public static string LoadGlobalFile(string filePath)
        {
            if (FileExist(filePath))
            {
                return LoadFile(filePath);
            }

            return "";
        }

        private static bool FileExist(string fileName)
        {
            throw new NotImplementedException();
        }

        public static List<string> GetFileNames(string directory)
        {
            List<string> files = Directory.GetFiles(directory)?.ToList();
            files.RemoveAll(x => x.GetExtention() != ".csv");
            return files;
        }

        public static string GetExtention(this string fileName)
        {
            if (fileName.LastIndexOf(".") >= 0)
            {
                return fileName.Substring(fileName.LastIndexOf("."));
            }

            return "";
        }
    }
}
