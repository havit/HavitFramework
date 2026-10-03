using System.Buffers;
using System.Buffers.Text;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Havit.Business.Benchmarks;

/// <summary>
/// Porovnání způsobů parsování identifikátorů v CollectionPropertyHolder.EnsureLazyValueInitialization
/// (formát "1|2|3|" - oddělovač je i za poslední položkou).
/// Cílem je najít délku vstupu, od které se vyplatí "dlouhá" větev (Utf8Parser) oproti "krátké" (Split + FastIntParse).
/// CollectionPropertyHolder aktuálně přepíná při délce vstupu > 25 znaků.
/// Délka vstupu = ItemCount * (IdDigits + 1).
/// </summary>
/// <remarks>
/// Práce s výsledným ID (getObjectFunc + Add do kolekce) je pro všechny varianty stejná, proto ji nahrazujeme součtem ID.
/// </remarks>
[MemoryDiagnoser]
public class CollectionPropertyHolderParseBenchmark
{
	[Params(1, 2, 3, 4, 6, 8, 12, 16, 32, 128, 1024)]
	public int ItemCount { get; set; }

	[Params(1, 3, 6)]
	public int IdDigits { get; set; }

	private string _itemIDsWithDelimiter;

	[GlobalSetup]
	public void Setup()
	{
		int firstId = (int)Math.Pow(10, IdDigits - 1); // 1, 100, 100000
		_itemIDsWithDelimiter = String.Concat(Enumerable.Range(0, ItemCount).Select(i => (firstId + (i % (9 * firstId))).ToString() + "|"));
	}

	/// <summary>
	/// Krátká větev CollectionPropertyHolderu (beze změny).
	/// </summary>
	[Benchmark(Baseline = true)]
	public int Split_FastIntParse()
	{
		int result = 0;
		string[] itemIDs = _itemIDsWithDelimiter.Split('|');
		int itemIDsLength = itemIDs.Length - 1; // za každou (i za poslední) položkou je oddělovač
		for (int i = 0; i < itemIDsLength; i++)
		{
			result += FastIntParse(itemIDs[i]);
		}
		return result;
	}

	/// <summary>
	/// Dlouhá větev CollectionPropertyHolderu před změnou (alokace pole pomocí Encoding.UTF8.GetBytes).
	/// </summary>
	[Benchmark]
	public int Utf8Parser_EncodingGetBytes()
	{
		int result = 0;
		Span<byte> itemIDsSpan = Encoding.UTF8.GetBytes(_itemIDsWithDelimiter);
		while (itemIDsSpan.Length > 0)
		{
			Utf8Parser.TryParse(itemIDsSpan, out int id, out int bytesConsumed);
			result += id;
			itemIDsSpan = itemIDsSpan.Slice(bytesConsumed + 1);
		}
		return result;
	}

	/// <summary>
	/// Dlouhá větev CollectionPropertyHolderu po změně (buffer z ArrayPool).
	/// </summary>
	[Benchmark]
	public int Utf8Parser_ArrayPool()
	{
		int result = 0;
		byte[] itemIDsBuffer = ArrayPool<byte>.Shared.Rent(_itemIDsWithDelimiter.Length);
		try
		{
			int itemIDsBytesCount = Encoding.UTF8.GetBytes(_itemIDsWithDelimiter, 0, _itemIDsWithDelimiter.Length, itemIDsBuffer, 0);
			Span<byte> itemIDsSpan = itemIDsBuffer.AsSpan(0, itemIDsBytesCount);
			while (itemIDsSpan.Length > 0)
			{
				Utf8Parser.TryParse(itemIDsSpan, out int id, out int bytesConsumed);
				result += id;
				itemIDsSpan = itemIDsSpan.Slice(bytesConsumed + 1);
			}
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(itemIDsBuffer);
		}
		return result;
	}

	/// <summary>
	/// Kandidát: parsování přímo nad znaky řetězce - bez alokace, bez převodu do UTF-8, jediný průchod.
	/// </summary>
	[Benchmark]
	public int CharParse()
	{
		int result = 0;
		string itemIDsWithDelimiter = _itemIDsWithDelimiter;
		int value = 0;
		bool negative = false;
		for (int i = 0; i < itemIDsWithDelimiter.Length; i++)
		{
			char c = itemIDsWithDelimiter[i];
			if (c == '|')
			{
				result += negative ? -value : value;
				value = 0;
				negative = false;
			}
			else if (c == '-')
			{
				negative = true;
			}
			else
			{
				value = (10 * value) + (c - '0');
			}
		}
		return result;
	}

	/// <summary>
	/// Kopie BusinessObjectBase.FastIntParse.
	/// </summary>
	private static int FastIntParse(string value)
	{
		unchecked
		{
			int result = 0;
			byte negative = (byte)(((value.Length > 0) && (value[0] == '-')) ? 1 : 0);

			byte l = (byte)value.Length;
			for (byte i = negative; i < l; i++)
			{
				result = (10 * result) + (value[i] - 48);
			}

			return negative == 0 ? result : -1 * result;
		}
	}
}
