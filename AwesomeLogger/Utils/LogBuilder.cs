using System;
using System.Collections.Generic;
using System.Text;
using AwesomeLogger.Loggers;

namespace AwesomeLogger.Utils
{
    public static class LogBuilder
    {
	    public static LogBase DefaultLog(string name)
	    {
			var rollingFileLog = new LogRollingFile(name);
			var consoleLog = new LogConsole(new LogConsoleOptions(-1400, 0, 1400, 500));
			rollingFileLog.AddPipe(consoleLog);
		    return rollingFileLog;
	    }
    }
}
