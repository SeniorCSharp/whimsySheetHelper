using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Windows.Forms;
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

        public static void Test()
        {
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
                            if (!possibleConvertions.Exists(x => x.Converter == ConverterType.defaultText))
                            {
                                ValueHandler handler = DataManager.Setting.handlers.FirstOrDefault(x => x.Converter == ConverterType.defaultText);
                                if (handler != null)
                                {
                                    possibleConvertions.Add(handler);
                                }
                            }

                            delegates[i] = ShowDialog(possibleConvertions, fileName, headerField);
                            continue;
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

        public static ValueHandler ShowDialog(List<ValueHandler> options, string filename, string headerName)
        {
            Form form = new Form();
            form.Width = 500;
            form.Height = 500;
            form.StartPosition = FormStartPosition.Manual;
            form.BackColor = Color.FromArgb(32, 32, 32);
            form.Location = Cursor.Position;
            form.FormBorderStyle = FormBorderStyle.None;

            Panel panel = new Panel();
            panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panel.Location = new System.Drawing.Point(0, 0);
            panel.Size = new System.Drawing.Size(500, 500);
            panel.Name = "panel3";


            Label textLabel = new Label();
            textLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            textLabel.Location = new System.Drawing.Point(10, 10);
            textLabel.Size = new System.Drawing.Size(365, 80);
            textLabel.ForeColor = System.Drawing.Color.LightGreen;
            textLabel.Text = "Unable to accurately determine cell format. Please select your preferred option from the list below.\r\nFIle :" +
                             filename + "\r\nHeader : " + headerName;

            Button confirmation = new Button();
            confirmation.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            confirmation.Cursor = System.Windows.Forms.Cursors.Default;
            confirmation.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            confirmation.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            confirmation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            confirmation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            confirmation.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            confirmation.ForeColor = System.Drawing.Color.LightGreen;
            confirmation.Location = new System.Drawing.Point(375, 10);
            confirmation.Size = new System.Drawing.Size(80, 30);
            confirmation.Text = "Ok";
            confirmation.UseVisualStyleBackColor = true;
            confirmation.Click += (sender, e) =>
            {
                form.DialogResult = DialogResult.OK;
                form.Close();
            };
            confirmation.Enabled = false;

            CheckedListBox box = new CheckedListBox();
            box.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            box.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            box.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            box.ForeColor = System.Drawing.Color.LightGreen;
            box.FormattingEnabled = true;
            box.Items.AddRange(options.Select(x => x.Converter.ToString()).ToArray());

            box.Location = new System.Drawing.Point(10, 85);
            box.Size = new System.Drawing.Size(200, 150);
            box.CheckOnClick = true;
            box.ItemCheck += (sender, args) =>
            {
                bool enabled = false;
                for (int i = 0; i < box.Items.Count; ++i)
                {
                    if (i != args.Index)
                    {
                        box.SetItemChecked(i, false);
                    }

                    if (box.GetItemChecked(i))
                    {
                        enabled = true;
                    }
                }

                confirmation.Enabled = !enabled;
            };

            form.Controls.Add(confirmation);
            form.Controls.Add(textLabel);
            form.Controls.Add(box);
            form.Controls.Add(panel);
            if (form.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < box.Items.Count; ++i)
                {
                    if (box.GetItemChecked(i))
                    {
                        return options[i];
                    }
                }
            }

            return null;
        }
    }
}
