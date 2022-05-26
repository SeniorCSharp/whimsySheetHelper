using System;
using System.Collections.Generic;

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
        public static Setting defaultSetting =>
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
                }
            };

        public List<string> FirstLine;

        public List<ValueHandler> handlers;
    }
}
