using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml.Serialization;
using System.Linq;
using System.Xml;
using System.IO;
using Havit.Diagnostics.Contracts;
using System.Reflection;

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
		private readonly Dictionary<string, object> _values; 
		#endregion
	
		#region Constructor
		/// <summary>
		/// Konstruktor.
		/// </summary>
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
			// Rozdíl verzí: Viz komentáře metod FromXmlDocument_VersionX.

			XmlDocument result;

			#region Verze 1
			//Type[] extenderTypes = PersisterControlExtenderRepository.Default.GetExtenderValuesTypes();
			
			//XmlSerializer valueSerializer = new XmlSerializer(typeof(object), extenderTypes);
			//using (MemoryStream memoryStream = new MemoryStream())
			//{
			//    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			//    xmlWriterSettings.OmitXmlDeclaration = true; // s xml deklarací se nedaří uložit do databáze

			//    XmlWriter writer = XmlWriter.Create(memoryStream, xmlWriterSettings);

			//    writer.WriteStartElement("ControlsValuesHolder");
			//    writer.WriteAttributeString("version", "1");

			//    foreach (string key in _values.Keys)
			//    {
			//        writer.WriteStartElement("Item");

			//        writer.WriteElementString("Key", key);

			//        writer.WriteStartElement("Value");
			//        object value = GetValue(key);
			//        valueSerializer.Serialize(writer, value);
			//        writer.WriteEndElement();

			//        writer.WriteEndElement();
			//    }
			//    writer.WriteEndElement(); // ControlsValuesHolder
			//    writer.Close();

			//    memoryStream.Seek(0, SeekOrigin.Begin);

			//    result = new XmlDocument();
			//    result.Load(memoryStream);
			//}
			#endregion

			#region Verze 2
			using (MemoryStream memoryStream = new MemoryStream())
			{
			    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			    xmlWriterSettings.OmitXmlDeclaration = true; // s xml deklarací se nedaří uložit do databáze

			    XmlWriter writer = XmlWriter.Create(memoryStream, xmlWriterSettings);

				XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
				ns.Add("", ""); // zajišťuje vynechání výchozích namespaces (při použití v metodě Serialize)

			    writer.WriteStartElement("ControlsValuesHolder");
			    writer.WriteAttributeString("version", "2");

			    foreach (string key in _values.Keys)
			    {
			        writer.WriteStartElement("Item");

			        writer.WriteElementString("Key", key);

			        writer.WriteStartElement("Value");
			        object value = GetValue(key);
					if (value == null)
					{
						writer.WriteAttributeString("isNull", "true");
					}
					else
					{
						Type valueType = value.GetType();
						string assemblyName = valueType.Assembly.GetName().Name;
						if (assemblyName != "mscorlib")
						{
							writer.WriteAttributeString("assembly", value.GetType().Assembly.GetName().Name);
						}
						writer.WriteAttributeString("type", valueType.FullName);

						XmlSerializer valueSerializer = new XmlSerializer(valueType);
						valueSerializer.Serialize(writer, value);
					}

			    	writer.WriteEndElement(); // Value

			        writer.WriteEndElement(); // Item
			    }
			    writer.WriteEndElement(); // ControlsValuesHolder
			    writer.Close();

			    memoryStream.Seek(0, SeekOrigin.Begin);

			    result = new XmlDocument();
			    result.Load(memoryStream);
			}
			#endregion
			return result;
		}
		#endregion

		#region FromXmlDocument
		/// <summary>
		/// Převede reprezentaci hodnot controlů z XmlDocumentu do ControlsValuesHolderu.
		/// </summary>
		public static ControlsValuesHolder FromXmlDocument(XmlDocument xmlDocument)
		{

			using (XmlReader reader = XmlReader.Create(new StringReader(xmlDocument.OuterXml)))
			{
				reader.Read();
				string version = reader.GetAttribute("version");
				reader.Read();

				switch (version)
				{
					case "1":
						return FromXmlDocument_Version1(reader);
					case "2":
						return FromXmlDocument_Version2(reader);
					default:
						throw new InvalidOperationException("Nepodporovaná verze hodnot ControlsValuesHolderu.");
				}

			}
		}
		#endregion

		#region FromXmlDocument_Version1
		/// <summary>
		/// Verze 1
		/// Snažila se o to, aby se nevytvářela znovu a znovu třída XmlSerializer pro každou hodnotu v datech.
		/// To se ale později ukázalo kontraproduktivní - použité přetížení není efektivní, vytváří znovu a znovu temporary assembly 
		/// a pravděpodobně vede k memory leakům - viz http://www-jo.se/f.pfleger/memoryleak
		/// </summary>
		private static ControlsValuesHolder FromXmlDocument_Version1(XmlReader reader)
		{
			ControlsValuesHolder result = new ControlsValuesHolder();

			Type[] extenderTypes = PersisterControlExtenderRepository.Default.GetExtenderValuesTypes();
			XmlSerializer valueSerializer = new XmlSerializer(typeof(object), extenderTypes);

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
			
			return result;
		}
		#endregion

		#region FromXmlDocument_Version2
		/// <summary>
		/// Verze 2
		/// Snaží se vyřešit výkonový problém verze 1 i s pravděpodobným memory leakem (viz tamní komentář).
		/// Namísto použití jednoto XmlSerializeru se pro každou načítanou hodnotu vytváří nová instance, přes to je výkon lepší.
		/// Dochází k reuse případné assembly a nemělo by docházet k memory leakům.
		/// </summary>
		private static ControlsValuesHolder FromXmlDocument_Version2(XmlReader reader)
		{
			ControlsValuesHolder result = new ControlsValuesHolder();

			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				reader.ReadStartElement("Item");

				reader.ReadStartElement("Key");
				string key = reader.ReadString();
				reader.ReadEndElement();

				Contract.Assert(reader.IsStartElement("Value"));
				
				bool valueIsNull = String.Equals("true", reader.GetAttribute("isNull"));
				string valueAssembly = reader.GetAttribute("assembly");
				string valueType = reader.GetAttribute("type");
				reader.Read();

				// null hodnoty mají valueIsNull = true
				// systémové typy (mscorlib) nemají uvedenu hodnotu assembly
				
				object value;
				if (valueIsNull)
				{
					value = null;
				}
				else
				{
					Type type;
					if (valueAssembly != null)
					{
						Assembly assembly;
						try
						{
							assembly = Assembly.Load(valueAssembly);
						}
						catch (FileNotFoundException)
						{
							throw new InvalidOperationException(String.Format("Assembly {0} se nepodařilo načíst.", valueAssembly));
						}
						type = assembly.GetType(valueType, true);
					}
					else
					{
						type = Type.GetType(valueType, true);
					}

					XmlSerializer valueSerializer = new XmlSerializer(type);
					value = (object)valueSerializer.Deserialize(reader);
				}

				if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == "Value")) // null hodnoty nemají uzavírací element
				{
					reader.ReadEndElement(); // Value
				}
				
				result.SetValue(key, value);

				reader.ReadEndElement(); // Item
				reader.MoveToContent();
			}
			reader.ReadEndElement();
			return result;

		}
		#endregion

	}
}
