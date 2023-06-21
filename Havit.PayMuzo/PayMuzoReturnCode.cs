using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Havit.PayMuzo;

/// <summary>
/// Návratový kód PayMUZO, hlášení výsledku operace.
/// </summary>
public abstract class PayMuzoReturnCode
{
	private static readonly Hashtable codeTypes = new Hashtable();

	/// <summary>
	/// Český význam.
	/// </summary>
	public string CsText
	{
		get { return _csText; }
		set { _csText = value; }
	}
	private string _csText;

	/// <summary>
	/// Anglický význam.
	/// </summary>
	public string EnText
	{
		get { return _enText; }
		set { _enText = value; }
	}
	private string _enText;

	/// <summary>
	/// Číselná hodnota návratového kódu.
	/// </summary>
	public int Value
	{
		get { return _value; }
		set { _value = value; }
	}
	private int _value;

	/// <summary>
	/// Vytvoří instanci return-code a nastaví počáteční hodnoty.
	/// </summary>
	/// <param name="value">numeric code</param>
	/// <param name="csText">význam česky</param>
	/// <param name="enText">význam anglicky</param>
	protected PayMuzoReturnCode(int value, string csText, string enText)
	{
		this._value = value;
		this._csText = csText;
		this._enText = enText;
	}

	/// <summary>
	/// Najde kód a vrátí ho. Pokud není nalezen, vrací <c>null</c>.
	/// </summary>
	/// <param name="value">numerická hodnota kódu</param>
	/// <typeparam name="T">typ kódu</typeparam>
	protected internal static T FindByValueInternal<T>(int value)
		where T : class
	{
		Hashtable codes = (Hashtable)codeTypes[typeof(T)];
		if (codes == null)
		{
			return null;
		}
		return (T)codes[value];
	}

	/// <summary>
	/// Zaregistruje return-code do interní Hashtable.
	/// </summary>
	public static void RegisterCode(PayMuzoReturnCode code)
	{
		Hashtable codes = (Hashtable)codeTypes[code.GetType()];
		if (codes == null)
		{
			codes = new Hashtable();
			codeTypes.Add(code.GetType(), codes);
		}
		if (!codes.ContainsKey(code.Value))
		{
			codes.Add(code.Value, code);
		}
	}
}
