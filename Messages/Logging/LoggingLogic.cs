using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using File = System.IO.File;

namespace PeaceDaBoll.Messages.Logging
{
    internal class LoggingLogic
    {
        public static async Task LoggingWriter(string message)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath) + @$"\log-{DateTime.Now:dd.MM.yyy}.txt";
            if (File.Exists(path))
            {
                await File.AppendAllTextAsync(path, $"{DateTime.Now:G}: {message}\n");
            }
            else
            {
                File.Create(path);
                await File.AppendAllTextAsync(path, $"{DateTime.Now:G}: {message}\n");
            }
        }
    }
}
