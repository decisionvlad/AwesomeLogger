using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace AwesomeLogger.Utils
{
	public static class ColorUtils
	{
		internal static string Color2Web(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

		public static Color Web2Color(string s)
		{
			try
			{
				var rStr = s.Substring(1, 2);
				var gStr = s.Substring(3, 2);
				var bStr = s.Substring(5, 2);
				var r = int.Parse(rStr, NumberStyles.HexNumber, null);
				var g = int.Parse(gStr, NumberStyles.HexNumber, null);
				var b = int.Parse(bStr, NumberStyles.HexNumber, null);
				var color = Color.FromArgb(r, g, b);
				return color;
			}
			catch (Exception)
			{
				return Color.White;
			}
		}

		public static Color Interpolate(Color x, Color y, double l)
		{
			var r = Interpolate(x.R, y.R, l);
			var g = Interpolate(x.G, y.G, l);
			var b = Interpolate(x.B, y.B, l);
			var res = Color.FromArgb(255, r, g, b);
			return res;
		}

		private static byte Interpolate(int a, int b, double l) => (byte)(a + (b - a) * l);
	}
}
