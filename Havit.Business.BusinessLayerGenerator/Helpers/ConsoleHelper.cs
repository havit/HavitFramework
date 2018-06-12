using System;
using System.IO;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	public static class ConsoleHelper
	{
		#region WarningColor (const) 
		public const ConsoleColor WarningColor = ConsoleColor.Yellow;
		#endregion

		#region Properties
		public static int WarningCount { get; private set; }
		public static int ErrorCount { get; private set; }
		#endregion

		#region WriteLineInfo
		/// <summary>
		/// Zapíše informaci na standardní výstup.
		/// </summary>
		public static void WriteLineInfo(string format, params object[] args)
		{
			WriteLineWithColor(Console.Out, ConsoleColor.Gray, format, args);
		} 
		#endregion

		#region WriteLineError
		/// <summary>
		/// Zapíše chybovou zprávu na chybový výstup.
		/// </summary>
		public static void WriteLineError(string format, params object[] args)
		{
			WriteLineWithColor(Console.Error, ConsoleColor.Red, format, args);
			ErrorCount += 1;			
		} 
		#endregion

		#region WriteLineWarning
		/// <summary>
		/// Zapíše upozornění na chybový výstup.
		/// </summary>
		public static void WriteLineWarning(string format, params object[] args)
		{
			WriteLineWithColor(Console.Error, WarningColor, format, args);
			WarningCount += 1;
		}
		#endregion

		#region WriteLineWithColor
		/// <summary>
		/// Nastaví barvu konzole a provede zápis zprávy do writeru.
		/// </summary>
		public static void WriteLineWithColor(TextWriter writer, ConsoleColor color, string format, params object[] args)
		{
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			writer.WriteLine(format, args);
			Console.ForegroundColor = originalColor;
		} 
		#endregion
	}
}
