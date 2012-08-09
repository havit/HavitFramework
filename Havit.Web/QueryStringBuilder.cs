using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Web;

namespace Havit.Web
{
	/// <summary>
	/// Pomocník pro sestavení QueryStringu.
	/// </summary>
	public class QueryStringBuilder : NameValueCollection
	{
		#region ctor
		/// <summary>
		/// Vytvoøí instanci.
		/// </summary>
		/// <remarks>
		/// Používá se StringComparer.OrdinalIgnoreCase po vzoru System.Web.HttpValueCollection.
		/// </remarks>
		public QueryStringBuilder()
			: base(StringComparer.OrdinalIgnoreCase)
		{
		}
		#endregion

		#region Add
		/// <summary>
		/// Pøidá hodnotu do QueryStringu. Pokud již hodnota existuje, potom pøidá další a QueryString bude obsahovat hodnot více.
		/// Pokud chcete nastavit hodnoty bez možnosti duplicit, použijte metodu Set().
		/// </summary>
		/// <exception cref="ArgumentException">pokud je argument name null nebo String.Empty</exception>
		/// <param name="name">název hodnoty</param>
		/// <param name="value">hodnota</param>
		public override void Add(string name, string value)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Argument nesmí být null ani String.Empty.", "name");
			}

			base.Add(name, value);
		}
		#endregion

		#region Set
		/// <summary>
		/// Nastaví hodnotu do QueryStringu. Pokud již hodnota existuje, potom ji pøenastaví na novou hodnotu.
		/// Pokud hodnota neexistuje, založí ji. Pokud chcete pøidávat hodnoty s možnosti duplicit, použijte metodu Add().
		/// </summary>
		/// <exception cref="ArgumentException">pokud je argument name null nebo String.Empty</exception>
		/// <param name="name">název hodnoty</param>
		/// <param name="value">hodnota</param>
		public override void Set(string name, string value)
		{
			if (String.IsNullOrEmpty(name))
			{
				throw new ArgumentException("Argument nesmí být null ani String.Empty.", "name");
			}

			base.Set(name, value);
		}
		#endregion

		#region ToString
		/// <summary>
		/// Pøevede na QueryString, neobsahuje úvodní ? (otazník).
		/// </summary>
		/// <param name="urlEncoded">indikuje, zda-li má být výstup (názvy i hodnoty) UrlEncoded</param>
		/// <returns>QueryString bez úvodního ? (otazníku)</returns>
		public virtual string ToString(bool urlEncoded)
		{
			int count = this.Count;
			if (count == 0)
			{
				return string.Empty;
			}
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < count; i++)
			{
				string key = this.GetKey(i);
				string value;
				if (urlEncoded)
				{
					key = HttpUtility.UrlEncode(key);
				}
				string keyEquals = key + "=";
				ArrayList values = (ArrayList)base.BaseGet(i);
				int valuesCount = (values != null) ? values.Count : 0;
				if (sb.Length > 0)
				{
					sb.Append('&');
				}
				if (valuesCount == 1)
				{
					sb.Append(keyEquals);
					value = (string)values[0];
					if (urlEncoded)
					{
						value = HttpUtility.UrlEncode(value);
					}
					sb.Append(value);
				}
				else if (valuesCount == 0)
				{
					sb.Append(keyEquals);
				}
				else
				{
					for (int j = 0; j < valuesCount; j++)
					{
						if (j > 0)
						{
							sb.Append('&');
						}
						sb.Append(keyEquals);
						value = (string)values[j];
						if (urlEncoded)
						{
							value = HttpUtility.UrlEncode(value);
						}
						sb.Append(value);
					}
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Pøevede na url-encoded QueryString bez úvodního ? (otazníku).
		/// </summary>
		/// <returns>url-encoded QueryString bez úvodního ? (otazníku)</returns>
		public override string ToString()
		{
			return this.ToString(true);
		}
		#endregion

		#region GetUrlWithQueryString
		/// <summary>
		/// Sestaví URL s QueryStringem na základì zadaného URL (které již mùže nìjaký QueryString obsahovat).
		/// Pokud chcete získat samotný QueryString, použijte metodu ToString().
		/// </summary>
		/// <param name="url">vstupní URL (které již mùže nìjaký QueryString obsahovat)</param>
		/// <returns>URL s QueryStringem</returns>
		public string GetUrlWithQueryString(string url)
		{
			if (url == null)
			{
				url = String.Empty;
			}

			if (url.Contains("?"))
			{
				char last = url[url.Length - 1];
				if ((last != '&') && (last != '?'))
				{
					url = url + "&";
				}
			}
			else
			{
				url = url + "?";
			}
			return url + this.ToString();
		}
		#endregion
	}
}
