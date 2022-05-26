using System.Collections.Generic;
using System.Linq;

namespace SheetHelper
{
    internal class CSVFile
    {
        public List<List<string>> Content;

        public CSVFile(List<List<string>> content)
        {
            Content = content;
        }

        public CSVFile(List<string[]> content)
        {
            Content = new List<List<string>>();
            foreach (string[] line in content)
            {
                Content.Add(line.ToList());
            }
        }
    }
}
