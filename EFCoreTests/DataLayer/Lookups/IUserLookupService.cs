using Havit.EFCoreTests.Model;

namespace Havit.EFCoreTests.DataLayer.Lookups
{
	public interface IUserLookupService
	{
		User GetUserByUsername(string username);
	}
}