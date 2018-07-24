using AwesomeLogger.Structs;
using AwesomeLogger.Utils;

namespace AwesomeLogger.LogPrependers
{
	public class TimestampPrepender : ILogPrepender
	{
		public LogLineChunk Prepend(LogLevel level) => new LogLineChunk(LogUtils.LogTime, LogColors.Timestamp);
	}
}