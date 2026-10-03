using System.Buffers;
using System.Buffers.Text;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Havit.Business.Benchmarks;

// Výsledky: .NET 10.0.12, Linux x64 (Intel Xeon 2.10GHz), ShortRun job. Mean v ns, v závorce Allocated.
// Pro net48 (kde Havit.Business běží) zatím nezměřeno - viz run.ps1.
//
// | IdDigits | ItemCount | Délka | Split_FastIntParse | Utf8Parser_EncodingGetBytes | Utf8Parser_ArrayPool | CharParse |
// |---------:|----------:|------:|-------------------:|----------------------------:|---------------------:|----------:|
// | 1 | 1 | 2 | 26.4 (64 B) | 18.1 (32 B) | 13.0 (0 B) | 2.1 (0 B) |
// | 1 | 2 | 4 | 38.1 (96 B) | 17.6 (32 B) | 14.5 (0 B) | 2.9 (0 B) |
// | 1 | 3 | 6 | 45.1 (128 B) | 20.6 (32 B) | 21.1 (0 B) | 4.8 (0 B) |
// | 1 | 4 | 8 | 70.0 (160 B) | 21.8 (32 B) | 19.7 (0 B) | 4.1 (0 B) |
// | 1 | 6 | 12 | 102.6 (224 B) | 29.3 (40 B) | 21.3 (0 B) | 5.8 (0 B) |
// | 1 | 8 | 16 | 123.0 (288 B) | 30.3 (40 B) | 31.9 (0 B) | 8.8 (0 B) |
// | 1 | 12 | 24 | 181.5 (416 B) | 41.7 (48 B) | 37.0 (0 B) | 12.8 (0 B) |
// | 1 | 16 | 32 | 216.9 (544 B) | 51.2 (56 B) | 46.7 (0 B) | 18.9 (0 B) |
// | 1 | 32 | 64 | 414.6 (1056 B) | 90.4 (88 B) | 93.0 (0 B) | 38.6 (0 B) |
// | 1 | 128 | 256 | 1,663.2 (4128 B) | 333.5 (280 B) | 306.5 (0 B) | 131.9 (0 B) |
// | 1 | 1024 | 2048 | 14,857.1 (32800 B) | 2,616.2 (2072 B) | 2,192.5 (0 B) | 922.3 (0 B) |
// | 3 | 1 | 4 | 29.0 (72 B) | 16.7 (32 B) | 13.8 (0 B) | 2.7 (0 B) |
// | 3 | 2 | 8 | 39.8 (112 B) | 20.9 (32 B) | 15.5 (0 B) | 5.4 (0 B) |
// | 3 | 3 | 12 | 68.1 (152 B) | 25.4 (40 B) | 22.2 (0 B) | 9.7 (0 B) |
// | 3 | 4 | 16 | 72.3 (192 B) | 24.4 (40 B) | 26.0 (0 B) | 11.2 (0 B) |
// | 3 | 6 | 24 | 103.8 (272 B) | 37.9 (48 B) | 25.3 (0 B) | 13.0 (0 B) |
// | 3 | 8 | 32 | 148.5 (352 B) | 43.3 (56 B) | 39.2 (0 B) | 16.0 (0 B) |
// | 3 | 12 | 48 | 209.0 (512 B) | 51.0 (72 B) | 54.8 (0 B) | 23.5 (0 B) |
// | 3 | 16 | 64 | 268.1 (672 B) | 77.5 (88 B) | 68.0 (0 B) | 42.0 (0 B) |
// | 3 | 32 | 128 | 457.6 (1312 B) | 131.6 (152 B) | 118.4 (0 B) | 66.9 (0 B) |
// | 3 | 128 | 512 | 1,805.0 (5152 B) | 459.4 (536 B) | 395.9 (0 B) | 257.5 (0 B) |
// | 3 | 1024 | 4096 | 17,280.1 (40992 B) | 3,756.8 (4120 B) | 2,748.0 (0 B) | 1,945.5 (0 B) |
// | 6 | 1 | 7 | 34.9 (80 B) | 16.3 (32 B) | 17.3 (0 B) | 3.7 (0 B) |
// | 6 | 2 | 14 | 54.2 (128 B) | 26.6 (40 B) | 18.2 (0 B) | 7.7 (0 B) |
// | 6 | 3 | 21 | 72.1 (176 B) | 32.0 (48 B) | 27.0 (0 B) | 9.8 (0 B) |
// | 6 | 4 | 28 | 74.8 (224 B) | 33.8 (56 B) | 34.0 (0 B) | 13.6 (0 B) |
// | 6 | 6 | 42 | 133.0 (320 B) | 51.7 (72 B) | 32.9 (0 B) | 19.0 (0 B) |
// | 6 | 8 | 56 | 180.1 (416 B) | 63.4 (80 B) | 46.9 (0 B) | 26.6 (0 B) |
// | 6 | 12 | 84 | 258.4 (608 B) | 86.2 (112 B) | 70.8 (0 B) | 49.2 (0 B) |
// | 6 | 16 | 112 | 341.2 (800 B) | 108.7 (136 B) | 89.2 (0 B) | 67.2 (0 B) |
// | 6 | 32 | 224 | 586.7 (1568 B) | 155.8 (248 B) | 154.7 (0 B) | 128.5 (0 B) |
// | 6 | 128 | 896 | 2,499.4 (6176 B) | 673.9 (920 B) | 564.9 (0 B) | 446.8 (0 B) |
// | 6 | 1024 | 7168 | 20,357.7 (49184 B) | 5,683.5 (7192 B) | 4,700.8 (0 B) | 3,081.3 (0 B) |

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
