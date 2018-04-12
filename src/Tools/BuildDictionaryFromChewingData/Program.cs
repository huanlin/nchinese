using System;
using System.IO;
using System.Threading.Tasks;
using Serilog;

namespace BuildDictionaryFromChewingData
{
    class Program
    {
        async static Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: ");
                Console.WriteLine("");
                Console.WriteLine("BuildDictionaryFromChewingData <input-file> <output-file>");
                return;
            }
            string inputFile = args[0];
            string outputFile = args[1];


            var logFileName = "log.txt";
            if (File.Exists(logFileName))
            {
                File.Delete(logFileName);
            }

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logFileName)
                .MinimumLevel.Debug()
                .CreateLogger();

            var dictManager = new WordDictionaryManager(Log.Logger);
            await dictManager.LoadFromTextFileAsync(inputFile);
            await dictManager.SaveToTextFileAsync(outputFile);
        }
    }
}
