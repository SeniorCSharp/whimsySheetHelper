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
        accountPayoneer,
        accountUpwork,
        amount,
        amountMinus
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
                    new ValueHandler()
                    {
                        Converter = ConverterType.accountPayoneer,
                        Destination = "Account",
                        PossibleColumnPatterns = new List<string>() { "account", "payoneer" }
                    },
                    new ValueHandler()
                    {
                        Converter = ConverterType.accountUpwork,
                        Destination = "Account",
                        PossibleColumnPatterns = new List<string>() { "account", "upwork" }
                    },
                    new ValueHandler()
                    {
                        Converter = ConverterType.amount,
                        Destination = "Amount",
                        PossibleColumnPatterns = new List<string>() { "amount", "credit", "deposit" }
                    },
                    new ValueHandler()
                    {
                        Converter = ConverterType.amountMinus,
                        Destination = "Amount",
                        PossibleColumnPatterns = new List<string>() { "debit", "withdraw" }
                    },
                    new ValueHandler()
                    {
                        Converter = ConverterType.defaultText,
                        Destination = "Curr",
                        PossibleColumnPatterns = new List<string>() { "currency", "curr" }
                    },
                    new ValueHandler()
                    {
                        Converter = ConverterType.defaultText,
                        Destination = "P/F",
                        PossibleColumnPatterns = new List<string>() { "P/F" }
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
