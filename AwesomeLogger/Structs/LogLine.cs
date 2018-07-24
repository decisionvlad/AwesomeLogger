using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AwesomeLogger.Extensions;
using AwesomeLogger.Utils;

namespace AwesomeLogger.Structs
{
	public class LogLineChunk
	{
		public string Text { get; }
		public string Color { get; }
		public Color WinColor => ColorUtils.Web2Color(Color);
		public LogLineChunk(string text, Color color)
		{
			Text = text;
			Color = ColorUtils.Color2Web(color);
		}
		public override string ToString() => $"{{Color}}{Text}";
	}

	public class LogLine
    {
	    public LogLevel Level { get; }
	    public List<LogLineChunk> Chunks { get; } = new List<LogLineChunk>();

	    public LogLine(LogLevel level)
	    {
		    Level = level;
	    }
	    public LogLine(LogLevel level, LogLineChunk firstChunk)
	    {
		    Level = level;
		    Chunks.Add(firstChunk);
	    }
	    public override string ToString() => Chunks.Select(c => c.ToString()).AggregateText(" | ");

	    public void AddChunk(LogLineChunk chunk)
	    {
		    Chunks.Add(chunk);
	    }
    }
}
