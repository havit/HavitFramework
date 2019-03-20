using System.Xml.Serialization;

namespace Havit.Services.Xml.Rss
{
	/// <summary>
	/// Třída reprezentující RSS prvek "item".
	/// </summary>
	[XmlRoot("item")]
	public class RssItem
	{
		private string title = "";
		private string link = "";
		private string description = "";
		private string author = "";
		private string category = "";
		private string comments = "";
		private string guid = "";
		private string pubDate = "";

		/// <summary>
		/// The title of the item.
		/// REQUIRED.
		/// </summary>
		[XmlElement("title")]
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
			}
		}

		/// <summary>
		/// The URL of the item.
		/// REQUIRED.
		/// </summary>
		[XmlElement("link")]
		public string Link
		{
			get
			{
				return link;
			}
			set
			{
				link = value;
			}
		}

		/// <summary>
		/// The item synopsis.
		/// REQUIRED.
		/// </summary>
		[XmlElement("description")]
		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		/// <summary>
		/// Email address of the author of the item.
		/// </summary>
		/// <example>
		/// lawyer@boyer.net (Lawyer Boyer)
		/// </example>
		[XmlElement("author")]
		public string Author
		{
			get
			{
				return author;
			}
			set
			{
				author = value;
			}
		}

		/// <summary>
		/// Includes the item in one or more categories.
		/// Současná implementace umožňuje zadání pouze jedné kategorie.
		/// </summary>
		[XmlElement("category")]
		public string Category
		{
			get
			{
				return category;
			}
			set
			{
				category = value;
			}
		}

		/// <summary>
		/// URL of a page for comments relating to the item.
		/// </summary>
		[XmlElement("comments")]
		public string Comments
		{
			get
			{
				return comments;
			}
			set
			{
				comments = value;
			}
		}

		/// <summary>
		/// A string that uniquely identifies the item.
		/// When present, an aggregator may choose to use this string to determine if an item is new.
		/// </summary>
		[XmlElement("guid")]
		public string Guid
		{
			get
			{
				return guid;
			}
			set
			{
				guid = value;
			}
		}

		/// <summary>
		/// Indicates when the item was published.
		/// </summary>
		/// <remarks>
		/// If it's a date in the future, aggregators may choose to not display the item until that date. 
		/// </remarks>
		[XmlElement("pubDate")]
		public string PubDate
		{
			get
			{
				return pubDate;
			}
			set
			{
				pubDate = value;
			}
		}

		/// <summary>
		/// Default constructor.
		/// Nutno nastavit Title, Link a Description !!!
		/// </summary>
		/// <remarks>
		/// Nutno zachovat kvůli XmlSerializeru, který vyžaduje přítomnost default constructoru.
		/// </remarks>
		public RssItem()
		{
		}

		/// <summary>
		/// Vytvoří RSS item s povinnými položkami
		/// </summary>
		public RssItem(string title, string link, string description)
		{
			this.Title = title;
			this.Link = link;
			this.Description = description;
		}
	}
}
