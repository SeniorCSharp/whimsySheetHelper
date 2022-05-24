namespace SheetHelper
{
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
}
