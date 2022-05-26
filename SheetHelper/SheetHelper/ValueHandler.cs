using System;
using System.Collections.Generic;
using System.Globalization;

namespace SheetHelper
{
    [Serializable]
    internal class ValueHandler
    {
        private const string ResultDateFormat = "dd.MM.yyyy";

        public delegate string ConvertionDelegate(string fieldValue);

        public delegate bool SuitableDelegate(ValueHandler handler, string fileName, List<List<string>> table, int columnIndex);

        public static Dictionary<ConverterType, ConvertionDelegate> HandlersDictionary = new Dictionary<ConverterType, ConvertionDelegate>()
        {
            { ConverterType.defaultText, DefaultTextHandler },
            { ConverterType.euDate, DateHandlerEU },
            { ConverterType.enDate, DateHandlerEN },
        };

        public static Dictionary<ConverterType, SuitableDelegate> SuitableDictionary = new Dictionary<ConverterType, SuitableDelegate>()
        {
            { ConverterType.defaultText, DefaultTextSuitable },
            { ConverterType.euDate, DateSuitableEU },
            { ConverterType.enDate, DateSuitableEN },
        };

        public List<string> PossibleColumnPatterns;
        public string OutputColumnKey;
        public ConverterType Converter;

        public SuitableDelegate SuitableFunction => SuitableDictionary[Converter];
        public ConvertionDelegate ConvertionFunction => HandlersDictionary[Converter];

        public List<string> AdditionInfo;

        public bool IsBasiclyFit(string columnName)
        {
            columnName = columnName.ToLower();
            foreach (string pattern in PossibleColumnPatterns)
            {
                if (columnName.Contains(pattern))
                {
                    return true;
                }
            }

            return false;
        }

        #region suitable functions

        private static bool DefaultTextSuitable(ValueHandler handler, string fileName, List<List<string>> table, int columnIndex)
        {
            return true;
        }

        private static bool DateSuitableEU(ValueHandler handler, string fileName, List<List<string>> table, int columnIndex)
        {
            for (int i = 1; i < table.Count; i++)
            {
                string dateString = table[i][columnIndex];
                if (!string.IsNullOrEmpty(dateString))
                {
                    try
                    {
                        DateTime dateValue = DateTime.Parse(dateString, new CultureInfo("ru-RU"), DateTimeStyles.None);
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool DateSuitableEN(ValueHandler handler, string fileName, List<List<string>> table, int columnIndex)
        {
            for (int i = 1; i < table.Count; i++)
            {
                string dateString = table[i][columnIndex];
                if (!string.IsNullOrEmpty(dateString))
                {
                    try
                    {
                        DateTime dateValue = DateTime.Parse(dateString, new CultureInfo("en-US"), DateTimeStyles.None);
                    }
                    catch (FormatException)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region handle functions

        private static string DefaultTextHandler(string fieldValue)
        {
            return fieldValue;
        }

        private static string DateHandlerEU(string fieldValue)
        {
            return DateTime.Parse(fieldValue, new CultureInfo("ru-RU"), DateTimeStyles.None).ToString(ResultDateFormat);
        }

        private static string DateHandlerEN(string fieldValue)
        {
            return DateTime.Parse(fieldValue, new CultureInfo("en-US"), DateTimeStyles.None).ToString(ResultDateFormat);
        }

        #endregion
    }
}
