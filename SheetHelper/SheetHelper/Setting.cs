using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SheetHelper
{
    public enum ConverterType
    {
        defaultText,
        enDate,
        euDate,
    }

    [Serializable]
    internal class Setting
    {
        public static Setting DefaultSetting =>
            new Setting()
            {
                FirstLine = new List<string>()
                {
                    "M", //month
                    "Date",
                    "P/F",
                    "Account",
                    "Description",
                    "Item",
                    "Amount",
                    "Curr", //currency
                },
                Handlers = new List<ValueHandler>()
                {
                    new ValueHandler()
                    {
                        Converter = ConverterType.defaultText,
                        Destination = "M",
                        PossibleColumnPatterns = new List<string>() { "month" }
                    },
                    new ValueHandler()
                    {
                        Converter = ConverterType.defaultText,
                        Destination = "Item",
                        PossibleColumnPatterns = new List<string>() { "item" }
                    },
                    new ValueHandler()
                    {
                        Converter = ConverterType.enDate,
                        Destination = "Date",
                        PossibleColumnPatterns = new List<string>() { "date", "time" }
                    },
                    new ValueHandler()
                    {
                        Converter = ConverterType.euDate,
                        Destination = "Date",
                        PossibleColumnPatterns = new List<string>() { "date", "time" }
                    },
                    new ValueHandler()
                    {
                        Converter = ConverterType.defaultText,
                        Destination = "Description",
                        PossibleColumnPatterns = new List<string>() { "description" }
                    },
                }
            };

        [JsonProperty("firstLine")] public List<string> FirstLine;
        [JsonProperty("handlers")] public List<ValueHandler> Handlers;

        public Setting()
        {
        }

        public bool IsDestinationExist(string destination)
        {
            return FirstLine.Any(s => s == destination);
        }
    }
}
