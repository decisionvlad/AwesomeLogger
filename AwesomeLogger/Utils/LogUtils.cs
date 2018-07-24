using System;
using System.Collections.Generic;
using System.Text;

namespace AwesomeLogger.Utils
{
    static class LogUtils
    {
		internal static string LogTime => DateTime.Now.ToString("HH:mm:ss.fffffff");
	}
}
