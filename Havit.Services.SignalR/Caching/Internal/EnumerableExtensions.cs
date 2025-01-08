using Havit.Diagnostics.Contracts;

namespace Havit.Services.SignalR.Caching.Internal;

internal static class EnumerableExtensions
{
	public static IEnumerable<string[]> ChunkifyStringsToMaxLength(this IEnumerable<string> source, int maxLength)
	{
		Contract.Requires<ArgumentNullException>(source != null, nameof(source));
		Contract.Requires<ArgumentOutOfRangeException>(maxLength > 0, nameof(maxLength));

		int aggregatedLength = 0;
		var chunk = new List<string>();

		using (var iter = source.GetEnumerator())
		{
			while (iter.MoveNext())
			{
				// Situace:
				// a) po přidání položky se stále vejdeme do limitu maxLength
				// b) po přidání položky se nevejdeme do limitu maxLength
				// c) ani se samotnou položkou se nevejdeme do limitu maxLength

				int currentItemLength = iter.Current.Length;

				// a) po přidání položky se stále vejdeme do limitu maxLength
				if (aggregatedLength + currentItemLength <= maxLength)
				{
					// přidáme položku do seznamu, započítáme délku a pokračujeme
					chunk.Add(iter.Current);
					aggregatedLength += iter.Current.Length;
				}
				else
				{
					if (currentItemLength > maxLength)
					{
						// c) ani se samotnou položkou se nevejdeme do limitu maxLength

						// z hlediska účelu jde o nepravděpodobný scénář (takže neuvažujeme o vyhození výjimky, atp.)
						// položku, která přepračuje maxlength vrátíme položku samotnou (položka jde mimo pořadí, což vzhledem k účelu nevadí)
						// neovlivníme tak obsah seznamu chunk, ani aggregatedLength						
						yield return [iter.Current];
					}
					else
					{
						// b) po přidání položky se nevejdeme do limitu maxLength

						// vrátíme aktuální chunk
						yield return chunk.ToArray();

						// vyvoříme nový chunk, resetujeme aggregatedLength od 0 (resp. započteme délku aktuální položky)
						chunk = new List<string> { iter.Current };
						aggregatedLength = currentItemLength;
					}
				}
			}

			// když dojdeme na konec vstupních dat, musíme ještě vrátit dosud napočítaný chunk (pokud obsahuje data)
			if (chunk.Count > 0)
			{
				yield return chunk.ToArray();
			}
		}
	}
}
