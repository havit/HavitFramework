namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model
{
	// Keep this class itself without XML comment (for tests)
	public class LoginAccount
	{
		public int Id { get; set; }

		/// <summary>
		/// LoginAccount's user name
		/// </summary>
		public string Username { get; set; }
	}
}
