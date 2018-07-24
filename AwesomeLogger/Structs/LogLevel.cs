using System.Drawing;

namespace AwesomeLogger.Structs
{
	public enum LogLevel
	{
		Info,
		Warn,
		Error
	}

	static class LogLevelDisplayAttributes
	{
		public static string[] Strings =
		{
			"NFO",
			"WRN",
			"ERR"
		};

		public static Color[] Colors =
		{
			LogColors.Info,
			LogColors.Warn,
			LogColors.Error
		};
	}
}
