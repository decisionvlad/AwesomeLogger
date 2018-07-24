using System;
using System.Drawing;
using System.Linq;
using AwesomeLogger.Loggers;
using AwesomeLogger.Structs;
using AwesomeLogger.Utils;

namespace LoggerTest
{
    class Program
    {
        static void Main(string[] args)
        {
	        /*var logBaseOptions = new LogOptions
	        {
				LogTimestamp = true,
				LogLogLevel = true
	        };
	        var logConsoleOptions = new LogConsoleOptions(-1400, 0, 1400, 500);
			var logRollingFileOptions = new LogRollingFileOptions
	        {
		        Path = "Logs",
		        ArchivePath = @"Logs\Archive",
		        NewLogOnRestart = true,
		        MaxSizeInKb = 100,
		        MaxArchiveCount = 5
	        };

	        var log = new LogRollingFile("test", logRollingFileOptions, logBaseOptions);
			var logConsole = new LogConsole(logConsoleOptions, logBaseOptions);
			log.AddPipe(logConsole);*/

	        var log = LogBuilder.DefaultLog("Test");

	        try
	        {
		        log.Info("Test", Color.DodgerBlue);
		        log.Info(" => Ouep", Color.Lime);
		        log.InfoL(" :)", Color.HotPink);

		        log.Info("Second", Color.DodgerBlue);
		        log.Info(" => OK", Color.Lime);
		        log.InfoL(" :))", Color.HotPink);

		        log.InfoL("Whole line", Color.PaleTurquoise);
		        log.Newline();
		        log.InfoL("Again!", Color.PaleGreen);
		        log.Newline();
		        log.Newline();
		        log.InfoL("Again2!", Color.PaleGreen);
		        Inside();
	        }
	        catch (Exception ex)
	        {
				log.Error(ex, true);
		        log.Info("ABC", Color.PaleGoldenrod);
	        }

	        Console.ReadKey();
        }

	    private static void Inside()
	    {
		    Inside2();
	    }

	    private static void Inside2()
	    {
		    throw new ApplicationException("TESTING :)");
	    }
	}
}

