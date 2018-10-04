using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Text
{
	/// <summary>
	/// Format file size as text, main goal is to show same size as Internet Explorer when downloading file.
	/// </summary>
	public class FileSizeToTextService : IFileSizeToTextService
	{
		/// <summary>
		/// Returns file size as text, main goal is to show same size as Internet Explorer when downloading file.
		/// </summary>
		/// <remarks>
		/// Internet explorer shows:
		/// - Size in bytes up to 1023 bytes (including)
		/// - In all other cases shows always 3 digits maximum (0.97, 1.11, 10.6, 99.9, 100, 120) when posible
		/// - Does not show trailing zeros (never 1.10)
		/// - kilobytes, megabytes, etc. uses radix 2, not 10 (kilobyte is 1024 bytes)
		/// - kilobytes, megabytes, etc. is shown for maximum 999 units, otherwise higher unit is used (1000 kilobytes is 0.97 MB)
		/// - value is never rounded, alwayes trimmed (meaning Math.Floor).
		/// - the highest unit is gigabyte.
		/// </remarks>
		public string GetFileSizeToText(long size)
		{
			long kilobytes = size / 1024;
			long megabytes = kilobytes / 1024;

			if (size < 1024)
			{
				return size.ToString() + " B";
			}

			decimal valueToDisplay;
			string unitToDisplay;

			if (kilobytes <= 999)
			{
				valueToDisplay = size / 1024M;
				unitToDisplay = "kB";
			}
			else if (megabytes <= 999)
			{
				valueToDisplay = kilobytes / 1024M;
				unitToDisplay = "MB";
			}
			else
			{
				valueToDisplay = megabytes / 1024M;
				unitToDisplay = "GB";
			}

			if (valueToDisplay < 10)
			{
				return MathExt.FloorToMultiple(valueToDisplay, 0.01M).ToString("N2") + " " + unitToDisplay;
			}
			else if (valueToDisplay < 100)
			{
				return MathExt.FloorToMultiple(valueToDisplay, 0.1M).ToString("N1") + " " + unitToDisplay;
			}
			else
			{
				return MathExt.FloorToMultiple(valueToDisplay, 1M).ToString("N0") + " " + unitToDisplay;
			}
		}
	}
}
