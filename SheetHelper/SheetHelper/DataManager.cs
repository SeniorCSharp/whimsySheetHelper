using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

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
                    setting = Setting.DefaultSetting;
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
            Debug.Log(1);
            ShowDialog(Setting.Handlers, "some folder", "some header");
            Debug.Log(2);
        }

        public static void SaveSettings()
        {
            string fileContent = JsonConvert.SerializeObject(Setting);
            SaverLoader.LocalSaveTo(SettingFileName, fileContent);
        }

        public static void SaveResult(string path)
        {
            CSVParser.SetArrayToFile(path, result.Content);
            MessageBox.Show("Файл сохранен");
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
            InitResultFile();
            SetInResultFileFirstLine();
            Dictionary<string, int> mappingDictionary = new Dictionary<string, int>();
            for (int i = 0; i < result.Content[0].Count; i++)
            {
                mappingDictionary.Add(result.Content[0][i], i);
            }

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

                    foreach (ValueHandler handler in DataManager.Setting.Handlers)
                    {
                        if (Setting.IsDestinationExist(handler.Destination))
                        {
                            if (handler.IsBasiclyFit(headerField))
                            {
                                if (handler.SuitableFunction(handler, fileName, table, i))
                                {
                                    possibleConvertions.Add(handler);
                                }
                            }
                        }
                    }

                    if (possibleConvertions.Count > 1)
                    {
                        // if (!possibleConvertions.Exists(x => x.Converter == ConverterType.defaultText))
                        // {
                        //     ValueHandler handler = DataManager.Setting.Handlers.FirstOrDefault(x => x.Converter == ConverterType.defaultText);
                        //     if (handler != null)
                        //     {
                        //         possibleConvertions.Add(handler);
                        //     }
                        // }

                        delegates[i] = ShowDialog(possibleConvertions, fileName, headerField);
                        continue;
                    }

                    if (possibleConvertions.Count == 1)
                    {
                        delegates[i] = possibleConvertions[0];
                    }
                }

                for (int i = 0; i < table.Count; i++)
                {
                    string[] row = new string[result.Content[0].Count];
                    for (int j = 0; j < header.Count; j++)
                    {
                        if (delegates[j] != null)
                        {
                            row[mappingDictionary[delegates[j].Destination]] = delegates[j].ConvertionFunction(table[i][j]);
                        }
                    }

                    result.Content.Add(row.ToList());
                }
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
            Form form = new Form
            {
                Width = 500,
                Height = 500,
                StartPosition = FormStartPosition.Manual,
                BackColor = Color.FromArgb(32, 32, 32),
                Location = Cursor.Position,
                FormBorderStyle = FormBorderStyle.None
            };

            Panel panel = new Panel
            {
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(500, 500),
                Name = "panel3"
            };

            Label textLabel = new Label
            {
                Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204),
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(365, 85),
                ForeColor = System.Drawing.Color.LightGreen,
                Text = "Unable to accurately determine column format. Please select your preferred option from the list below.\r\nFile : " +
                       filename + "\r\nColumn : " + headerName
            };

            Button confirmation = new Button
            {
                BackgroundImageLayout = System.Windows.Forms.ImageLayout.None,
                Cursor = System.Windows.Forms.Cursors.Default
            };
            confirmation.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            confirmation.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(32, 32, 32);
            confirmation.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(32, 32, 32);
            confirmation.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            confirmation.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
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

            CheckedListBox box = new CheckedListBox
            {
                BackColor = System.Drawing.Color.FromArgb(32, 32, 32),
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204),
                ForeColor = System.Drawing.Color.LightGreen,
                FormattingEnabled = true
            };
            List<string> opt = options.Select(x => x.Converter.ToString() + "; target -> " + x.Destination).ToList();
            opt.Add("do not process this column");
            box.Items.AddRange(opt.ToArray());

            box.Location = new System.Drawing.Point(10, 95);
            box.Size = new System.Drawing.Size(400, 150);
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
                        if (i != box.Items.Count - 1)
                        {
                            Debug.Log(i);
                            return options[i];
                        }
                    }
                }
            }

            Debug.Log("null");
            return null;
        }
    }
}
