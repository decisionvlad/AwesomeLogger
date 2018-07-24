using AwesomeLogger.Structs;

namespace AwesomeLogger.LogPrependers
{
	public class ChunkPrepender : ILogPrepender
	{
		private readonly LogLineChunk chunk;
		public ChunkPrepender(LogLineChunk chunk)
		{
			this.chunk = chunk;
		}

		public LogLineChunk Prepend(LogLevel level) => chunk;
	}
}