using Havit.Data.Patterns.DataSeeds;
using Havit.Data.Patterns.DataSeeds.Profiles;

namespace Havit.Data.Patterns.Tests.DataSeeds.Infrastructure;

/// <summary>
/// Testovací data seed, který při seedování zaznamená svůj label do sdíleného seznamu (pro ověření pořadí seedování).
/// Záznam probíhá v <see cref="SeedData"/>, kterou volá synchronní i asynchronní runner, takže třídu lze použít v obou variantách testů.
/// Použití uzavřených generických typů (např. <c>RecordingDataSeed&lt;DefaultProfile&gt;</c> vs <c>RecordingDataSeed&lt;ProfileWithPrerequisite&gt;</c>)
/// zajišťuje odlišný <see cref="object.GetType"/>, takže nedochází ke kolizi s kontrolou duplicitních typů v <see cref="DataSeedRunner"/>.
/// </summary>
internal class RecordingDataSeed<TProfile> : DataSeed<TProfile>
	where TProfile : IDataSeedProfile, new()
{
	private readonly List<string> _seedOrder;
	private readonly string _label;

	public RecordingDataSeed(List<string> seedOrder, string label)
	{
		_seedOrder = seedOrder;
		_label = label;
	}

	public override void SeedData()
	{
		_seedOrder.Add(_label);
	}
}
