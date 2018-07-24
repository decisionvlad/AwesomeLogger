using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using AwesomeLogger.Structs;

using Console = Colorful.Console;

namespace AwesomeLogger.Loggers
{
	public class LogConsoleOptions
	{
		public Rectangle DebugRect { get; }
		public LogConsoleOptions() => DebugRect = Rectangle.Empty;
		public LogConsoleOptions(int x, int y, int width, int height) => DebugRect = new Rectangle(x, y, width, height);
	}

	public class LogConsole : LogBase
	{
		public LogConsole(LogConsoleOptions options = null, LogOptions baseOptions = null) : base(baseOptions)
		{
			options = options ?? new LogConsoleOptions();
			if (options.DebugRect != Rectangle.Empty)
				ConsoleUtils.AdjustConsole(options.DebugRect);
		}

		internal override void WriteSpecific(LogLevel level, LogLineChunk chunk)
		{
			Console.Write(chunk.Text, chunk.WinColor);
		}

		internal override void NewlineSpecific(LogLine currentLine)
		{
			Console.WriteLine();
		}

		private static class ConsoleUtils
		{
			private const int SwpNozorder = 0x4;
			private const int SwpNoactivate = 0x10;
			[DllImport("kernel32")]
			private static extern IntPtr GetConsoleWindow();
			[DllImport("user32")]
			private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);
			private static IntPtr Handle => GetConsoleWindow();
			private static void SetWindowPosition(int x, int y, int width, int height)
			{
				SetWindowPos(Handle, IntPtr.Zero, x, y, width, height, SwpNozorder | SwpNoactivate);
			}
			public static void AdjustConsole(Rectangle rect)
			{
				try
				{
					SetWindowPosition(rect.X, rect.Y, rect.Width, rect.Height);
				}
				catch (Exception)
				{
					// ignored
				}
			}
		}
	}
}
