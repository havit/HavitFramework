namespace Havit.Data.Patterns.Tests.DataEntries.Infrastructure
{
	public class SystemCodebookEntry
	{
		public int Id { get; set; }

		public string Nazev { get; set; }

		public enum Entry
		{
			First = 1,
			Second = 2
		}
	}
}
