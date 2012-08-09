using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml.Serialization;
using System.Linq;
using System.Xml;
using System.IO;

namespace Havit.Web.UI.WebControls.ControlsValues
{
	/// <summary>
	/// Úložiště hodnot controlů.
	/// Klíč do úložiště si získává sám control ControlsValuesPersister.
	/// Hodnoty zpracovávají třídy implementující IPersisterCotrolExtender.
	/// </summary>
	[Serializable]
	public class ControlsValuesHolder
	{
		#region Private fields
		private Dictionary<string, object> _values; 
		#endregion
	
		#region Constructor
		public ControlsValuesHolder()
		{
			_values = new Dictionary<string, object>();			
		}
		#endregion

		#region ContainsKey
		/// <summary>
		/// Vrací true, pokud v kolekci hodnot existuje záznam s daným klíčem. Jinak false.
		/// </summary>
		public bool ContainsKey(string key)
		{
			return _values.ContainsKey(key);
		} 
		#endregion

		#region GetValue
		/// <summary>
		/// Vrací hodnotu pro daný klíč.
		/// Pokud v kolekci hodnot neexistuje záznam s daným klíčem, vyhazuje výjimku.
		/// </summary>
		public object GetValue(string key)
		{
			return _values[key];
		} 
		#endregion

		#region SetValue
		/// <summary>
		/// Přidá do kolekce záznam s daným klíčem a hodnotou.
		/// Pokud již záznam s daným klíčem existuje, je jeho hodnota přepsána.
		/// </summary>
		public void SetValue(string key, object value)
		{
			_values[key] = value;			
		} 
		#endregion

		#region ToXmlDocument
		/// <summary>
		/// Převede reprezentaci hodnot controlů z ControlsValuesHolderu do XmlDocumentu.
		/// </summary>
		public XmlDocument ToXmlDocument()
		{
			XmlDocument result;
			Type[] extenderTypes = PersisterControlExtenderRepository.Default.GetExtenderValuesTypes();
			
			XmlSerializer valueSerializer = new XmlSerializer(typeof(object), extenderTypes);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.OmitXmlDeclaration = true; // s xml deklarací se nedaří uložit do databáze

				XmlWriter writer = XmlWriter.Create(memoryStream, xmlWriterSettings);

				writer.WriteStartElement("ControlsValuesHolder");
				writer.WriteAttributeString("version", "1");

				foreach (string key in _values.Keys)
				{
					writer.WriteStartElement("Item");

					writer.WriteElementString("Key", key);

					writer.WriteStartElement("Value");
					object value = GetValue(key);
					valueSerializer.Serialize(writer, value);
					writer.WriteEndElement();

					writer.WriteEndElement();
				}
				writer.WriteEndElement(); // ControlsValuesHolder
				writer.Close();

				memoryStream.Seek(0, SeekOrigin.Begin);

				result = new XmlDocument();
				result.Load(memoryStream);
			}

			return result;
		}
		#endregion

		#region FromXmlDocument
		/// <summary>
		/// Převede reprezentaci hodnot controlů z XmlDocumentu do ControlsValuesHolderu.
		/// </summary>
		public static ControlsValuesHolder FromXmlDocument(XmlDocument xmlDocument)
		{
			ControlsValuesHolder result = new ControlsValuesHolder();

			Type[] extenderTypes = PersisterControlExtenderRepository.Default.GetExtenderValuesTypes();
			XmlSerializer valueSerializer = new XmlSerializer(typeof(object), extenderTypes);

			using (XmlReader reader = XmlReader.Create(new StringReader(xmlDocument.OuterXml))) // TODO: zjednodusit, lze-li
			{
				reader.ReadStartElement();

				reader.MoveToContent();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					reader.ReadStartElement("Item");

					reader.ReadStartElement("Key");
					string key = reader.ReadString();
					reader.ReadEndElement();

					reader.ReadStartElement("Value");
					object value = (object)valueSerializer.Deserialize(reader);
					reader.ReadEndElement();

					result.SetValue(key, value);

					reader.ReadEndElement();
					reader.MoveToContent();
				}
				reader.ReadEndElement();
			}

			return result;
		}
		#endregion
		
	}
}
