using System;

namespace ZipExtractor
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            StartupApplication app = new StartupApplication();
            app.Run();
        }
    }
}
