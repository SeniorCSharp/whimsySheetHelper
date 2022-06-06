using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SheetHelper
{
    public delegate string ConvertionDelegate(string fieldValue);

    public delegate bool SuitableDelegate(ValueHandler handler, string fileName, List<List<string>> table, int columnIndex);

    public class ValueHandler
    {
        private enum Cultures
        {
            eu,
            en,
            fr,
            de,
        }

        private const string ResultDateFormat = "dd.MM.yyyy";

        private static Dictionary<ConverterType, ConvertionDelegate> handlersDictionary = new Dictionary<ConverterType, ConvertionDelegate>()
        {
            { ConverterType.defaultText, DefaultTextHandler },
            { ConverterType.euDate, DateHandlerEU },
            { ConverterType.enDate, DateHandlerEN },
            { ConverterType.accountPayoneer, AccountHandlerPayoneer },
            { ConverterType.accountUpwork, AccountHandlerUpwork },
            { ConverterType.amount, AmountHandler },
            { ConverterType.amountMinus, AmountMinusHandler },
        };

        private static Dictionary<ConverterType, SuitableDelegate> suitableDictionary = new Dictionary<ConverterType, SuitableDelegate>()
        {
            { ConverterType.defaultText, DefaultTextSuitable },
            { ConverterType.euDate, DateSuitableEU },
            { ConverterType.enDate, DateSuitableEN },
            { ConverterType.accountPayoneer, AccountSuitablePayoneer },
            { ConverterType.accountUpwork, AccountSuitableUpwork },
            { ConverterType.amount, AmountSuitable },
            { ConverterType.amountMinus, AmountSuitable },
        };

        private static Dictionary<Cultures, CultureInfo> cultures = new Dictionary<Cultures, CultureInfo>()
        {
            { Cultures.eu, new CultureInfo("ru-RU") },
            { Cultures.en, new CultureInfo("en-US") },
        };

        [JsonIgnore] public SuitableDelegate SuitableFunction => suitableDictionary[Converter];
        [JsonIgnore] public ConvertionDelegate ConvertionFunction => handlersDictionary[Converter];

        #region serialized data

        [JsonProperty("posibleColumnPatterns")]
        public List<string> PossibleColumnPatterns;

        [JsonProperty("destination")] public string Destination;
        [JsonProperty("converter")] public ConverterType Converter;

        #endregion serialized data

        public ValueHandler()
        {
        }

        public ValueHandler GetCopy()
        {
            ValueHandler ret = new ValueHandler
            {
                Destination = Destination,
                Converter = Converter
            };
            ret.PossibleColumnPatterns = new List<string>(PossibleColumnPatterns.Count);
            foreach (string pattern in PossibleColumnPatterns)
            {
                ret.PossibleColumnPatterns.Add(pattern);
            }

            return ret;
        }

        public bool IsBasiclyFit(string columnName)
        {
            columnName = columnName.ToLower();
            foreach (string pattern in PossibleColumnPatterns)
            {
                if (columnName == pattern.ToLower())
                {
                    Debug.Log("find column mapping [" + columnName + "] -> [" + pattern + "]");
                    return true;
                }
                else
                {
                    //Debug.Log("don`t fit column mapping [" + columnName + "] -> [" + pattern.ToLower() + "]");
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
                        DateTime dateValue = DateTime.Parse(dateString, cultures[Cultures.eu], DateTimeStyles.None);
                    }
                    catch (Exception)
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
                        DateTime dateValue = DateTime.Parse(dateString, cultures[Cultures.en], DateTimeStyles.None);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool AccountSuitablePayoneer(ValueHandler handler, string fileName, List<List<string>> table, int columnIndex)
        {
            for (int i = 1; i < table.Count; i++)
            {
                string cell = table[i][columnIndex];
                if (!string.IsNullOrEmpty(cell))
                {
                    if (cell.ToLower().Contains("payoneer"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool AccountSuitableUpwork(ValueHandler handler, string fileName, List<List<string>> table, int columnIndex)
        {
            for (int i = 1; i < table.Count; i++)
            {
                string cell = table[i][columnIndex];
                if (!string.IsNullOrEmpty(cell))
                {
                    if (cell.ToLower().Contains("upwork"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool AmountSuitable(ValueHandler handler, string fileName, List<List<string>> table, int columnIndex)
        {
            return true;
        }

        #endregion suitable functions

        #region handle functions

        private static string DefaultTextHandler(string fieldValue)
        {
            return fieldValue;
        }

        private static string DateHandlerEU(string fieldValue)
        {
            return DateTime.Parse(fieldValue, cultures[Cultures.eu], DateTimeStyles.None).ToString(ResultDateFormat);
        }

        private static string DateHandlerEN(string fieldValue)
        {
            return DateTime.Parse(fieldValue, cultures[Cultures.en], DateTimeStyles.None).ToString(ResultDateFormat);
        }

        private static string AccountHandlerPayoneer(string fieldValue)
        {
            return "Payoneer Account";
        }

        private static string AccountHandlerUpwork(string fieldValue)
        {
            return "Upwork Account";
        }

        private static string AmountHandler(string fieldValue)
        {
            //find fractional
            int lastIndex1 = fieldValue.Length - 2;
            int lastIndex2 = fieldValue.Length - 3;

            fieldValue = fieldValue.Replace(" ", "");

            if (fieldValue.LastIndexOf(".") == lastIndex1 || fieldValue.LastIndexOf(",") == lastIndex1)
            {
                try
                {
                    int fract = int.Parse(fieldValue.Substring(lastIndex1 + 1));
                    fieldValue = fieldValue.Substring(0, lastIndex1);
                    fieldValue = fieldValue.Replace(".", "");
                    fieldValue = fieldValue.Replace(",", "");
                    int integerPart = int.Parse(fieldValue);
                    return integerPart.ToString() + "," + fract.ToString();
                }
                catch (Exception)
                {
                    Debug.Log("can not parse this money value -> " + fieldValue.Substring(lastIndex1 + 1));
                }
            }
            if (fieldValue.LastIndexOf(".") == lastIndex2 || fieldValue.LastIndexOf(",") == lastIndex2)
            {
                try
                {
                    int fract = int.Parse(fieldValue.Substring(lastIndex2 + 1));
                    fieldValue = fieldValue.Substring(0, lastIndex2);
                    fieldValue = fieldValue.Replace(".", "");
                    fieldValue = fieldValue.Replace(",", "");
                    int integerPart = int.Parse(fieldValue);
                    return integerPart.ToString() + "," + fract.ToString();
                }
                catch (Exception)
                {
                    Debug.Log("can not parse this money value -> " + fieldValue.Substring(lastIndex2 + 1));
                }
            }
            try
            {
                fieldValue = fieldValue.Replace(".", "");
                fieldValue = fieldValue.Replace(",", "");
                int integerPart = int.Parse(fieldValue);
                return integerPart.ToString() + ",00";
            }
            catch (Exception)
            {
                Debug.Log("can not parse this money value -> " + fieldValue);
            }

            return "";
        }

        private static string AmountMinusHandler(string fieldValue)
        {
            if (!string.IsNullOrEmpty(fieldValue))
            {
                string amount = AmountHandler(fieldValue);
                if (!string.IsNullOrEmpty(amount))
                {
                    return "-" + amount;
                }
            }

            return "";
        }

        #endregion handle functions
    }
}