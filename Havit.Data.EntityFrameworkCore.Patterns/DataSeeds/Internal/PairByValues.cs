namespace Havit.Data.EntityFrameworkCore.Patterns.DataSeeds.Internal;

internal class PairByValues : IEquatable<PairByValues>
{
	public object[] Data { get; }

	public PairByValues(object[] data)
	{
		Data = data;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as PairByValues);
	}

	public bool Equals(PairByValues other)
	{
		return (other != null) && (this.Data.SequenceEqual(other.Data));
	}

	public override int GetHashCode()
	{
		return Data.Aggregate(0, (value, item) => value ^ (item?.GetHashCode() ?? 0));
	}
}
