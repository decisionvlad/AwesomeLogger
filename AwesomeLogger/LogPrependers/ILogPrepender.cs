using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using AwesomeLogger.Structs;

namespace AwesomeLogger.LogPrependers
{
	public interface ILogPrepender
	{
		LogLineChunk Prepend(LogLevel level);
	}
}
