using System;
using System.Collections.Generic;
using System.Text;

namespace AwesomeLogger.Extensions
{
	public static class StringExtensions
	{
		public static string AggregateText<T>(this IEnumerable<T> list, string separator) => string.Join(separator, list);
	}
}
