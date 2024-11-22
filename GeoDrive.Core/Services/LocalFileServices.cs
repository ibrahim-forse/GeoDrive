using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoDrive.Core.Services
{
    public class LocalFileServices
    {
        public string[] ReadFileNames(string localStorageDirectory)
        {
            DirectoryInfo d = new DirectoryInfo(localStorageDirectory);

            FileInfo[] Files = d.GetFiles("*.json"); //Getting Text files
            List<string> fileNames = new List<string>();

            foreach (FileInfo file in Files)
            {
                fileNames.Add(Path.Combine(localStorageDirectory, file.Name));
            }

            return fileNames.ToArray();
        }
    }
}
