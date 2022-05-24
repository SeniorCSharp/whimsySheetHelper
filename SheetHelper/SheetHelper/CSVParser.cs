using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace SheetHelper
{
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
