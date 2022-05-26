using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Instrumentation;
using CsvHelper;
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

        private static Dictionary<string, CSVFile> allFiles = new Dictionary<string, CSVFile>();
        private static CSVFile result;

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
                Debug.Log(dialog.FileName);
                CurrentFolder = dialog.FileName;
                AddFiles(SaverLoader.GetFileNames(dialog.FileName));
            }
        }

        public static void CleanFiles()
        {
            allFiles = new Dictionary<string, CSVFile>();
        }

        public static void AddFiles(List<string> files)
        {
            files.RemoveAll(x => x.GetExtention() != ".csv");
            foreach (string path in files)
            {
                if (!allFiles.ContainsKey(path))
                {
                    Debug.Log(path);
                    allFiles.Add(path, new CSVFile(CSVParser.GetArrayFromFile(path)));
                }
            }
        }

        public static void MapAllFiles()
        {
            if (allFiles != null && allFiles.Count > 0)
            {
                InitResultFile();
                SetInResultFileFirstLine();
                foreach (KeyValuePair<string, CSVFile> file in allFiles)
                {
                    string fileName = file.Key;
                    List<List<string>> table = file.Value.Content;
                    List<string> header = table[0];
                    List<ValueHandler> delegates = new List<ValueHandler>(header.Count);
                    for (int i = 0; i < header.Count; i++)
                    {
                        delegates.Add(null);
                    }

                    for (int i = 0; i < header.Count; i++)
                    {
                        string headerField = header[i];
                        List<ValueHandler> possibleConvertions = new List<ValueHandler>();

                        foreach (ValueHandler handler in DataManager.Setting.handlers)
                        {
                            if (handler.IsBasiclyFit(headerField))
                            {
                                if (handler.SuitableFunction(handler, fileName, table, i))
                                {
                                    possibleConvertions.Add(handler);
                                }
                            }
                        }

                        if (possibleConvertions.Count > 1)
                        {
                            //предлагаем выбрать через диалоговое окно варианты и выбираем его в качестве преобразования
                        }

                        if (possibleConvertions.Count == 1)
                        {
                            delegates[i] = possibleConvertions[0];
                        }
                    }
                }
                //на этом этапе у нас готовы функции преобразования, так что проходимся по файлику и преобразуем с помощью функций
                //филды в строках, и добавляем по месту эти филды в результирующий список
                //а потом сохраняем по кнопке сейв - выводим окошко - куда сохранить
            }
        }

        private static void SetInResultFileFirstLine()
        {
            result.Content.Add(Setting.FirstLine);
        }

        private static void InitResultFile()
        {
            result = new CSVFile(new List<string[]>());
        }
    }
}
