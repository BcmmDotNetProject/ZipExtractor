using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ZipExtractor.Utils
{
    public class FileHelper
    {
        public const string FSUtil = "C:\\Windows\\System32\\fsutil.exe";

        public static FileStream CreateEmptyFile(string path, bool overwrite = false)
        {
            if (File.Exists(path))
            {
                if (!overwrite)
                {
                    throw new InvalidOperationException($"文件{path}已经存在。");
                }
                else
                {
                    File.Delete(path);
                }
            }

            var process = new Process();

            process.StartInfo.FileName = FSUtil;
            process.StartInfo.Arguments = $"file createNew \"{path}\" 0";

            try
            {
                if (!process.Start())
                {
                    throw new InvalidOperationException("创建文件进程启动失败。");
                }

                process.WaitForExit();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("创建文件进程出现错误。", exception);
            }

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"创建文件进程出现错误，退出码{process.ExitCode}。");
            }

            return File.OpenWrite(path); 
        }
    }
}
