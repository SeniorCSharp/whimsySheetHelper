using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SheetHelper
{
    internal static class SaverLoader
    {
        public static string LocalPath => Directory.GetCurrentDirectory();

        /// <summary>
        /// Saves data to a file with the specified name. Remember that this function saves the file in a standard folder.
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <param name="stringData">Your string data</param>
        public static void LocalSaveTo(string fileName, string stringData)
        {
            try
            {
                fileName = Normalizer(fileName);
                OverGeneratePath(LocalPath + fileName);
                FileStream fileStream = new FileStream(LocalPath + fileName, FileMode.Create);
                StreamWriter myFile = new StreamWriter(fileStream);
                myFile.Write(stringData);
                myFile.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Load string data from file. Remember that this function loads the file from the standard folder.
        /// </summary>
        /// <param name="fileName">Name of file</param>
        /// <returns>String data or "", if file don`t exist</returns>
        public static string LocalLoadFrom(string fileName)
        {
            try
            {
                fileName = Normalizer(fileName);
                if (File.Exists(LocalPath + fileName))
                {
                    StreamReader myFile = new StreamReader(LocalPath + fileName);
                    string dString = myFile.ReadToEnd();
                    myFile.Close();
                    return dString;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return "";
        }

        /// <summary>
        /// Saves data to a file with the specified name. Remember that in this function you must specify the full path to the file.
        /// </summary>
        /// <param name="fullPath">Full path to file</param>
        /// <param name="stringData">Your string data</param>
        public static void GlobalSaveTo(string fullPath, string stringData)
        {
            try
            {
                OverGeneratePath(fullPath);
                FileStream fileStream = new FileStream(fullPath, FileMode.Create);
                StreamWriter myFile = new StreamWriter(fileStream);
                myFile.Write(stringData);
                myFile.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Load string data from file. Remember that in this function you must specify the full path to the file.
        /// </summary>
        /// <param name="fullPath">Full path to file</param>
        /// <returns>String data or "", if file don`t exist</returns>
        public static string GlobalLoadFrom(string fullPath)
        {
            try
            {
                if (File.Exists(fullPath))
                {
                    StreamReader myFile = new StreamReader(fullPath);
                    string dString = myFile.ReadToEnd();
                    myFile.Close();
                    return dString;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return "";
        }

        public static List<string> GetFileNames(string directory)
        {
            List<string> files = Directory.GetFiles(directory)?.ToList();
            return files;
        }

        public static string GetExtention(this string fileName)
        {
            if (fileName.LastIndexOf(".") >= 0)
            {
                return fileName.Substring(fileName.LastIndexOf("."));
            }

            return "";
        }

        #region private functions

        private static string Normalizer(string fileName)
        {
            if (fileName.IndexOf('/') == 0)
            {
                return fileName;
            }
            else
            {
                return "/" + fileName;
            }
        }

        private static string OverGeneratePath(string fileName)
        {
            string[] tempArray = fileName.Split('/');
            List<string> finalListOfPathParts = new List<string>();

            #region delete extra "/"

            foreach (string str in tempArray)
            {
                if (str != "")
                {
                    finalListOfPathParts.Add(str);
                }
            }

            #endregion delete extra "/"

            #region create returned string and overGenerating path

            string returnedPath = "";
            for (int i = 0; i < finalListOfPathParts.Count - 1; i++)
            {
                returnedPath += finalListOfPathParts[i] + "/";
                if (!Directory.Exists(returnedPath))
                {
                    try
                    {
                        Directory.CreateDirectory(returnedPath);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            }

            returnedPath += finalListOfPathParts[finalListOfPathParts.Count - 1];

            #endregion create returned string and overGenerating path

            return returnedPath;
        }

        #endregion private functions
    }
}
