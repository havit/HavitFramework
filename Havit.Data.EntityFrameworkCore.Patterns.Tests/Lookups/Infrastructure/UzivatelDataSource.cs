using Havit.Data.EntityFrameworkCore.Patterns.DataSources.Fakes;

namespace Havit.Data.EntityFrameworkCore.Patterns.Tests.Lookups.Infrastructure;

public class UzivatelDataSource : FakeDataSource<Uzivatel>
{
	public UzivatelDataSource(Uzivatel[] uzivatele) : base(uzivatele)
	{

	}
}
