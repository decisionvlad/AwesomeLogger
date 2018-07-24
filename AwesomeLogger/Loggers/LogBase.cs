using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using AwesomeLogger.LogPrependers;
using AwesomeLogger.Structs;

namespace AwesomeLogger.Loggers
{
	public class LogOptions
	{
		public bool LogTimestamp { get; set; } = true;
		public bool LogLogLevel { get; set; } = true;
	}



	public abstract class LogBase
    {
	    private readonly LogOptions options;
		private readonly object lockObject = new object();
		private readonly List<LogBase> pipes = new List<LogBase>();
		private readonly List<ILogPrepender> prependers = new List<ILogPrepender>();

	    private LogLine currentLine = null;
	    private bool IsAtStartOfLine => currentLine == null;

		protected LogBase(LogOptions options = null)
	    {
		    this.options = options ?? new LogOptions();
			SetupPrependers();
	    }

		// ****************************
		// * Implementation dependent *
		// ****************************
		internal abstract void WriteSpecific(LogLevel level, LogLineChunk chunk);
	    internal abstract void NewlineSpecific(LogLine currentLine);

		// **********************
		// * Base logging logic *
		// **********************
	    public void WriteL(LogLevel level, LogLineChunk chunk)
	    {
		    Write(level, chunk);
		    Newline();
	    }

		public void Write(LogLevel level, LogLineChunk chunk)
	    {
		    lock (lockObject)
		    {
			    if (IsAtStartOfLine)
			    {
				    currentLine = new LogLine(level);
				    foreach (var prepender in prependers)
				    {
					    var chunkToPrepend = prepender.Prepend(level);
					    currentLine.AddChunk(chunkToPrepend);
					    WriteSpecific(level, chunkToPrepend);
				    }
			    }

			    WriteSpecific(level, chunk);

			    foreach (var pipe in pipes)
				    pipe.Write(level, chunk);
		    }
	    }

	    public void Newline()
	    {
		    lock (lockObject)
		    {
			    NewlineSpecific(currentLine);

			    foreach (var pipe in pipes)
				    pipe.Newline();

			    currentLine = null;
		    }
	    }



		// ********
		// * Misc *
		// ********
		public void AddPipe(LogBase log) => pipes.Add(log);
		public void RemovePipe(LogBase log) => pipes.Remove(log);

		private void SetupPrependers()
	    {
		    Debug.Assert(options != null, nameof(this.options) + " != null");
		    if (options.LogTimestamp)
			    prependers.Add(new TimestampPrepender());
		    if (options.LogTimestamp && options.LogLogLevel)
			    prependers.Add(new ChunkPrepender(new LogLineChunk(" - ", LogColors.Timestamp)));
		    if (options.LogLogLevel)
			    prependers.Add(new LogLevelPrepender());
		    if (options.LogTimestamp || options.LogLogLevel)
			    prependers.Add(new ChunkPrepender(new LogLineChunk(": ", LogColors.Timestamp)));
	    }
	}
}
