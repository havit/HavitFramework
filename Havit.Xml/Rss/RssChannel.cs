using System.Xml.Serialization;

namespace Havit.Xml.Rss
{
	/// <summary>
	/// Třída reprezentující RSS channel
	/// </summary>
	[XmlRoot("channel")]
	public class RssChannel
	{
		#region Fields

		private string title = "";
		private string link = "";
		private string description = "";
		private string language = "";		
		private string copyright = "";
		private string managingEditor = "";
		private string webMaster = "";
		private string pubDate = "";
		private string lastBuildDate = "";
		private string category = "";
		private string generator = "";
		private string ttl = "";
		private string rating = "";
		private RssImage image;
		private RssItemCollection items;

		/// <summary>
		/// The name of the channel.
		/// REQUIRED Element.
		/// </summary>
		/// <remarks>
		/// It's how people refer to your service. If you have an HTML website that contains the same
		/// information as your RSS file, the title of your channel should be the same as the title of your website.
		/// </remarks>
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
		/// The URL to the HTML website corresponding to the channel.
		/// REQUIRED Element.
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
		/// Phrase or sentence describing the channel.
		/// REQUIRED Element.
		/// </summary>
		[XmlElement("description", IsNullable=false)]
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
		/// The language the channel is written in.
		/// Pro češtinu je "cs", popř. "cs-CZ".
		/// </summary>
		/// <remarks>
		/// This allows aggregators to group all Italian language sites, for example, on a single page.
		/// A list of allowable values for this element, as provided by Netscape - http://blogs.law.harvard.edu/tech/stories/storyReader$15
		/// You may also use values defined by the W3C - http://www.w3.org/TR/REC-html40/struct/dirlang.html#langcodes
		/// </remarks>
		[XmlElement("language")]
		public string Language
		{
			get
			{
				return language;
			}
			set
			{
				language = value;
			}
		}

		/// <summary>
		/// Copyright notice for content in the channel.
		/// </summary>
		[XmlElement("copyright")]
		public string Copyright
		{
			get
			{
				return copyright;
			}
			set
			{
				copyright = value;
			}
		}

		/// <summary>
		/// Email address for person responsible for editorial content.
		/// </summary>
		[XmlElement("managingEditor")]
		public string ManagingEditor
		{
			get
			{
				return managingEditor;
			}
			set
			{
				managingEditor = value;
			}
		}

		/// <summary>
		/// Email address for person responsible for technical issues relating to channel.
		/// </summary>
		[XmlElement("webMaster")]
		public string WebMaster
		{
			get
			{
				return webMaster;
			}
			set
			{
				webMaster = value;
			}
		}

		/// <summary>
		/// The publication date for the content in the channel.
		/// </summary>
		/// <remarks>
		/// For example, the New York Times publishes on a daily basis, the publication date flips once every 24 hours.
		/// That's when the pubDate of the channel changes. All date-times in RSS conform to the Date and Time Specification
		/// of RFC 822, with the exception that the year may be expressed with two characters or four characters (four preferred).
		/// http://asg.web.cmu.edu/rfc/rfc822.html
		/// </remarks>
		/// <example>
		/// Např. "Sat, 07 Sep 2002 00:00:01 GMT"
		/// </example>
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
		/// The last time the content of the channel changed.
		/// </summary>
		[XmlElement("lastBuildDate")]
		public string LastBuildDate
		{
			get
			{
				return lastBuildDate;
			}
			set
			{
				lastBuildDate = value;
			}
		}

		/// <summary>
		/// Specify one or more categories that the channel belongs to.
		/// Současná implementace umožňuje zadat pouze jednu kategorii a to ještě bez zadání domény (domain atribut).
		/// </summary>
		/// <remarks>
		/// Follows the same rules as the ITEM-level category element.
		/// </remarks>
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
		/// A string indicating the program used to generate the channel.
		/// </summary>
		[XmlElement("generator")]
		public string Generator
		{
			get
			{
				return generator;
			}
			set
			{
				generator = value;
			}
		}

		/// <summary>
		/// Time to Live.
		/// It's a number of minutes that indicates how long a channel can be cached before refreshing from the source.
		/// </summary>
		[XmlElement("ttl")]
		public string Ttl
		{
			get
			{
				return ttl;
			}
			set
			{
				ttl = value;
			}
		}

		/// <summary>
		/// The PICS rating for the channel.
		/// http://www.w3.org/PICS/
		/// </summary>
		[XmlElement("rating")]
		public string Rating
		{
			get
			{
				return rating;
			}
			set
			{
				rating = value;
			}
		}

		/// <summary>
		/// Specifies a GIF, JPEG or PNG image that can be displayed with the channel.
		/// </summary>
		[XmlElement("image")]
		public RssImage Image
		{
			get
			{
				return image;
			}
			set
			{
				image = value;
			}
		}

		/// <summary>
		/// Kolekce jednotlivých prvků ITEM.
		/// </summary>
		[XmlElement("item")]
		public RssItemCollection Items
		{
			get
			{
				return items;
			}
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor nutný pro XmlSerializer.
		/// Nutno nastavit required položky Title, Link a Description.
		/// </summary>
		public RssChannel()
		{
			this.items = new RssItemCollection();
		}

		/// <summary>
		/// Constructor s povinnými položkami.
		/// </summary>
		public RssChannel(string title, string link, string description)
		{
			this.Title = title;
			this.Link = link;
			this.Description = description;
			this.items = new RssItemCollection();
		}

		#endregion
	}
}
