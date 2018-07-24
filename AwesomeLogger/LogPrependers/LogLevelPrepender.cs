using AwesomeLogger.Structs;

namespace AwesomeLogger.LogPrependers
{
	public class LogLevelPrepender : ILogPrepender
	{
		public LogLineChunk Prepend(LogLevel level) => new LogLineChunk(LogLevelDisplayAttributes.Strings[(int) level], LogLevelDisplayAttributes.Colors[(int) level]);
	}
}