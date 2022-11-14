using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ZipExtractor.Utils
{
    public class RenameManager
    {
        private readonly FileInfo _batFile;

        private const string BatFileName = "RenameExecutableFiles.bat";

        public RenameManager()
        {
            var directory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)
                ?? throw new InvalidOperationException("主模块目录不存在。");

            _batFile = new FileInfo(Path.Combine(directory, BatFileName));
        }

        public void Append(string sourceFile, string newFileName)
        {
            StreamWriter stream;

            if (!_batFile.Exists)
            {
                stream = new StreamWriter(_batFile.Create(), Encoding.GetEncoding("GBK"));
            }
            else
            {
                stream = new StreamWriter(_batFile.Open(FileMode.Append), Encoding.GetEncoding("GBK"));
            }

            var stringBuilder = new StringBuilder("ren");

            stringBuilder.Append(" \"");
            stringBuilder.Append(sourceFile);
            stringBuilder.Append("\" \"");
            stringBuilder.Append(newFileName);
            stringBuilder.Append("\"");

            stream.WriteLine(stringBuilder) ;
            stream.Close();
        }

        public void Clean()
        {
            if (!_batFile.Exists)
            {
                _batFile.Create();
            }
            else
            {
                using var stream = _batFile.Open(FileMode.Truncate);
            }
        }
    }
}
