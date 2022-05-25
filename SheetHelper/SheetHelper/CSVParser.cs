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
                List<string[]> rows = new List<string[]>();

                using (StreamReader reader = new StreamReader(filePath))
                using (CsvParser parser = new CsvParser(reader, CultureInfo.InvariantCulture))
                {
                    while (parser.Read())
                    {
                        string[] row = parser.Record;
                        rows.Add(row);
                    }
                }

                return rows;
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
