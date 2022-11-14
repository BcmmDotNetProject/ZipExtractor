using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ZipExtractor.Utils;

namespace ZipExtractor.ViewModels
{
    public class MainWindowViewModel : ICheckValue
    {
        public string? ZipPath { get; set; } = @"D:\Project\ZipExtractor\ZipExtractor\bin\Debug\netcoreapp3.1\ZipFile.zip";

        public string? OutputDirectory { get; set; } = @"D:\Project\ZipExtractor\ZipExtractor\bin\Debug\netcoreapp3.1\ZipFile";

        public ICommand? ExtractCommand { get; set; }

        private readonly RenameManager _renameManager;

        public MainWindowViewModel(RenameManager renameManager)
        {
            ExtractCommand = new CommandBase()
            {
                DoExecute = OnExtract
            };

            _renameManager = renameManager;
        }

        private void OnExtract(object _)
        {
            _renameManager.Clean();

            if (Directory.Exists("ZipFile"))
            {
                Directory.Delete("ZipFile", true);
            }

            if (!CheckValues())
            {
                return;
            }

            var zipArchive = ZipFile.Open(ZipPath, ZipArchiveMode.Read, Encoding.GetEncoding("GBK"));

            foreach (var entry in zipArchive.Entries)
            {
                ExtractArchive(entry); 
            }
        }

        private void ExtractArchive(ZipArchiveEntry zipArchiveEntry)
        {
            var destDirectory = Path.GetDirectoryName(zipArchiveEntry.FullName);

            destDirectory = Path.Combine(OutputDirectory ?? throw new InvalidOperationException("无效的输出目录。"),
              destDirectory ?? String.Empty);

            string destPath;

            if (string.IsNullOrEmpty(zipArchiveEntry.Name))
            {
                Directory.CreateDirectory(destDirectory);
                return;
            }
            else
            if (string.Equals(Path.GetExtension(zipArchiveEntry.Name), ".exe", StringComparison.OrdinalIgnoreCase))
            {
                var randomName = Path.GetRandomFileName();

                destPath = Path.Combine(destDirectory, randomName);
                _renameManager.Append(destPath, zipArchiveEntry.Name);
            }
            else
            {
                destPath = Path.Combine(destDirectory, zipArchiveEntry.Name);
            }

            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            using var zipStream = zipArchiveEntry.Open();
            using var readStream = FileHelper.CreateEmptyFile(destPath);

            zipStream.CopyTo(readStream);
        }

        public bool CheckValues()
        {
            if (string.IsNullOrWhiteSpace(ZipPath))
            {
                MessageBox.Show("请输入Zip文件", "提示");
            }
            else
            if (Path.GetFileName(ZipPath)?.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0
                || Path.GetDirectoryName(ZipPath)?.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                MessageBox.Show($"无效的文件名或文件路径{ZipPath}。", "提示");
            }
            else
            if (!File.Exists(ZipPath))
            {
                MessageBox.Show($"文件{ZipPath}不存在。", "提示");
            }
            else
            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                MessageBox.Show("请输入输出路径", "提示");
            }
            else
            if (OutputDirectory.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                MessageBox.Show($"无效的输出路径{OutputDirectory}。", "提示");
            }
            else
            {
                return true;
            }

            return false;
        }
    }
}
