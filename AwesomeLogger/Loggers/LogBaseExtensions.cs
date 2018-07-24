using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AwesomeLogger.Structs;
using AwesomeLogger.Utils;

namespace AwesomeLogger.Loggers
{
    public static class LogBaseExtensions
    {
		public static void Info(this LogBase log, string str, Color color) => log.Write(LogLevel.Info, new LogLineChunk(str, color));
		public static void InfoL(this LogBase log, string str, Color color) => log.WriteL(LogLevel.Info, new LogLineChunk(str, color));

	    public static void Warn(this LogBase log, string str, Color color) => log.Write(LogLevel.Warn, new LogLineChunk(str, color));
	    public static void WarnL(this LogBase log, string str, Color color) => log.WriteL(LogLevel.Warn, new LogLineChunk(str, color));

	    public static void Error(this LogBase log, string str, Color color) => log.Write(LogLevel.Error, new LogLineChunk(str, color));
	    public static void ErrorL(this LogBase log, string str, Color color) => log.WriteL(LogLevel.Error, new LogLineChunk(str, color));

	    public static void Error(this LogBase log, Exception ex, bool fullStackTrace = false)
	    {
		    log.Newline();
		    var title = "Exception";
		    var titlePad = new string('*', title.Length + 4);
		    var titleSpaces = new string(' ', 10);
		    log.ErrorL($"{titleSpaces}{titlePad}", Color.Red);
		    log.ErrorL($"{titleSpaces}* Exception *", Color.Red);
		    log.ErrorL($"{titleSpaces}{titlePad}", Color.Red);

		    void LogOne(Exception x, int indent)
		    {
			    var indentStr = new string(' ', indent * 2);
			    log.Error($"{indentStr}Type    : ", Color.Green);
			    log.ErrorL(x.GetType().Name, Color.LimeGreen);
			    log.Error($"{indentStr}Message : ", Color.Green);
			    log.ErrorL(x.Message, Color.LimeGreen);

			    void NoStack()
			    {
				    log.Error($"{indentStr}Stack   : ", Color.Green);
				    log.ErrorL("N/A (null)", Color.LimeGreen);
			    }

			    if (x.StackTrace == null)
				    NoStack();
			    else
			    {
				    var stackLines = x.StackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
				    if (stackLines.Count == 0)
					    NoStack();
				    else
				    {
					    var colorStart = Color.Red;
					    var colorEnd = Color.Yellow;

					    var first = true;
					    for (var i = 0; i < stackLines.Count; i++)
					    {
						    var stackLine = stackLines[i];
						    var isOwnCode = stackLine.Contains(".cs:line ");

						    if (!fullStackTrace)
						    {
							    if (!isOwnCode)
								    continue;
						    }

						    if (first)
						    {
							    log.Error($"{indentStr}Stack   : ", Color.Green);
							    first = false;
						    }
						    else
							    log.Error($"{indentStr}          ", Color.Green);

						    var l = (double)i / (stackLines.Count - 1);
						    var color = ColorUtils.Interpolate(colorStart, colorEnd, l);
						    if (isOwnCode)
							    color = Color.DodgerBlue;

						    log.ErrorL(stackLine.TrimStart(), color);

						    if (!fullStackTrace)
							    break;
					    }
					}
			    }
		    }

		    var index = 0;
		    while (ex != null)
		    {
			    LogOne(ex, index++);
			    ex = ex.InnerException;
		    }
		    log.Newline();
		}
	}
}
