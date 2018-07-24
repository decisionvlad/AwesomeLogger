using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using AwesomeLogger.Structs;
using Polly;

using Console = Colorful.Console;

namespace AwesomeLogger.Loggers
{
	public class LogRollingFileOptions
	{
		public string Path { get; set; } = "Logs";
		public string ArchivePath { get; set; } = @"Logs\Archive";
		public bool NewLogOnRestart { get; set; } = true;
		public int MaxSizeInKb { get; set; } = 10 * 1024;
		public int MaxArchiveCount { get; set; } = 128;
	}

	public class LogRollingFile : LogBase
	{
		private readonly RollingFileWriter writer;

		public LogRollingFile(string name, LogRollingFileOptions options = null, LogOptions baseOptions = null) : base(baseOptions)
	    {
			writer = new RollingFileWriter(name, options ?? new LogRollingFileOptions());
	    }

	    internal override void WriteSpecific(LogLevel level, LogLineChunk chunk)
	    {
		    writer.Write(chunk);
	    }

	    internal override void NewlineSpecific(LogLine currentLine)
	    {
		    writer.Newline();
	    }
    }

	class RollingFileWriter
	{
		private readonly string name;
		private readonly LogRollingFileOptions options;

		private readonly TimeSpan[] retryDelays = {TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)};
		private readonly Policy fileDeletePolicy;
		private readonly Policy fileMovePolicy;

		private string ActiveLogFilename => Path.Combine(options.Path, $"{name}.slog");
		private string ArchivedLogFilename(int index) => Path.Combine(options.ArchivePath, $"{name}_{index}.slog");

		private FileStream fs;
		private StreamWriter sw;

		private long activeLogSize = 0;

		public RollingFileWriter(string name, LogRollingFileOptions options)
		{
			this.name = name;
			this.options = options;
			fileDeletePolicy = Policy
				.Handle<Exception>()
				.WaitAndRetry(retryDelays, (ex, timespan, cnt, ctx) =>
					{
						Console.WriteLine($"Retrying LogFile Delete {cnt}/{retryDelays.Length}", Color.PaleVioletRed);
					});
			fileMovePolicy = Policy
				.Handle<Exception>()
				.WaitAndRetry(retryDelays, (ex, timespan, cnt, ctx) =>
					{
						Console.WriteLine($"Retrying LogFile Move {cnt}/{retryDelays.Length}", Color.PaleVioletRed);
					});
			Directory.CreateDirectory(options.Path);
			Directory.CreateDirectory(options.ArchivePath);

			if (File.Exists(ActiveLogFilename) && options.NewLogOnRestart)
				ArchiveActiveLog();

			AcquireStream(!options.NewLogOnRestart);
		}

		public void Write(LogLineChunk chunk)
		{
			var str = $"{{{chunk.Color}}}{chunk.Text}";
			sw.Write(str);
			activeLogSize += str.Length;
		}

		public void Newline()
		{
			sw.WriteLine();
			activeLogSize += Environment.NewLine.Length;
			ArchiveActiveLogIfTooBig();
		}

		private void ArchiveActiveLogIfTooBig()
		{
			if (activeLogSize >= options.MaxSizeInKb)
			{
				ArchiveActiveLog();
				AcquireStream(false);
			}
		}

		private void ArchiveActiveLog()
		{
			if (!File.Exists(ActiveLogFilename))
				throw new ApplicationException($"Active log doesn't exist: {ActiveLogFilename}");
			ReleaseStream();

			List<FileInfo> GetArchiveFilenames()
			{
				var list = new List<FileInfo>();
				for (var i = 0; i < options.MaxArchiveCount; i++)
					if (File.Exists(ArchivedLogFilename(i)))
						list.Add(new FileInfo(ArchivedLogFilename(i)));
				return list;
			}

			bool IsArchiveFull(List<FileInfo> archivedFilenames) => archivedFilenames.Count == options.MaxArchiveCount;

			string DeleteOldestArchiveAndReturnItsName(List<FileInfo> archivedFilenames)
			{
				archivedFilenames.Sort((a, b) => a.LastAccessTime.CompareTo(b.LastAccessTime));
				var first = archivedFilenames[0];

				fileDeletePolicy.Execute(() =>
				{
					File.Delete(first.FullName);
					if (File.Exists(first.FullName))
						throw new IOException($"File {first.FullName} still exists");
				});

				return first.FullName;
			}

			int GetFirstFreeIndex(List<FileInfo> archivedFilenames)
			{
				for (var index = 0; index < options.MaxArchiveCount; index++)
					if (!File.Exists(ArchivedLogFilename(index)))
						return index;
				throw new ApplicationException("Log archives shouldn't be full");
			}

			var archived = GetArchiveFilenames();
			string destFilename;
			if (IsArchiveFull(archived))
				destFilename = DeleteOldestArchiveAndReturnItsName(archived);
			else
				destFilename = ArchivedLogFilename(GetFirstFreeIndex(archived));

			fileMovePolicy.Execute(() =>
			{
				File.Move(ActiveLogFilename, destFilename);
			});
		}

		private void AcquireStream(bool append)
		{
			if (fs != null || sw != null)
				throw new ApplicationException("Acquiring log stream precondition failed: (fs != null || sw != null)");
			fs = new FileStream(ActiveLogFilename, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
			sw = new StreamWriter(fs)
			{
				AutoFlush = true
			};
			activeLogSize = new FileInfo(ActiveLogFilename).Length;
		}


		private void ReleaseStream()
		{
			if (fs != null)
			{
				sw.Dispose();
				fs.Dispose();
				sw = null;
				fs = null;
			}
		}
	}
}
