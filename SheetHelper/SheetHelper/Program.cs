using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SheetHelper
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    internal static class DataManager
    {
        private const string SettingFileName = "";

        private static Setting setting;
        public static Setting Setting => setting ?? LoadSetting();

        private static Setting LoadSetting()
        {
            string fileContent = SaverLoader.LoadLocalFile(SettingFileName);
            return setting;
        }
    }

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

    internal class Setting
    {
    }

    public static class CSVParser
    {
        public static List<string[]> GetArrayFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                IEnumerable<string[]> mapped = new List<string[]>();

                using (StreamReader reader = new StreamReader(filePath))
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    mapped = csv.GetRecords<string[]>();
                }

                return (List<string[]>)mapped;
            }

            return null;
        }

        public static void SetArrayToFile(string filePath, List<List<string>> text)
        {
            IEnumerable<string[]> mapped = text.Select(x => x.ToArray());

            using (StreamWriter writer = new StreamWriter(filePath))
            using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                foreach (List<string> item in text)
                {
                    foreach (string it in item)
                    {
                        csv.WriteField(it);
                    }

                    csv.NextRecord();
                }
            }
        }
    }
}
