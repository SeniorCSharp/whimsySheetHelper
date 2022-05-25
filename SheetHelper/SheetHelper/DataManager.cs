using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace SheetHelper
{
    internal static class DataManager
    {
        private const string SettingFileName = "setting.json";

        public static Setting Setting => setting ?? LoadSetting();
        public static string CurrentFolder { get; private set; }

        private static Setting setting = null;

        private static List<string> allFiles = new List<string>();

        public static Setting LoadSetting()
        {
            if (setting == null)
            {
                string fileContent = SaverLoader.LocalLoadFrom(SettingFileName);
                if (string.IsNullOrEmpty(fileContent))
                {
                    Debug.Log("Can`t find setting file. Make new...");
                }
                else
                {
                    Debug.Log("Try load settings...");
                    setting = JsonConvert.DeserializeObject<Setting>(fileContent);
                }
            }

            return setting;
        }

        public static void OpenFolderPicker()
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Debug.Log("You selected: " + dialog.FileName);
            }

            CurrentFolder = dialog.FileName;

            AddFiles(SaverLoader.GetFileNames(dialog.FileName));
        }

        public static void CleanFiles()
        {
            allFiles = new List<string>();
        }

        public static void AddFiles(List<string> files)
        {
            files.RemoveAll(x => x.GetExtention() != ".csv");
            foreach (string path in files)
            {
                Debug.Log(path);
                allFiles.Add(path);
            }
        }
    }
}
