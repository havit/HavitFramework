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
		// Pořadí-citlivé skládání hashe (konzistentní s Equals přes SequenceEqual); XOR by se u symetrických hodnot rušil a zvyšoval kolize.
		HashCode hashCode = new HashCode();
		foreach (object item in Data)
		{
			hashCode.Add(item);
		}
		return hashCode.ToHashCode();
	}
}
