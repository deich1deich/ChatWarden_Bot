using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = System.IO.File;

namespace PeaceDaBoll.InitCheck
{
    internal class InitializationCheck
    {
        private static List<string> filesName = 
            [
                "Banwords.txt",
                "Profiles.xyi"
            ];

        /// <summary>
        /// Метод для проверки на наличие директории Data и файлов Banwords.txt и Profiles.xyi
        /// </summary>
        /// <param name="path">Путь до директории с файлами</param>
        public static void CheckFileAndDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (string file in filesName)
            {
                string verifiableFile = path + @"\" + file;

                if (!File.Exists(verifiableFile))
                {
                    File.AppendAllText(verifiableFile, "");
                }
            }

            Form1.WriteLog("Проверка директории и файлов находящихся в ней прошла успешна!");
        }
    }
}
