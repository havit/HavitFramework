using System.Xml.Serialization;

namespace Havit.Services.Xml.Rss
{
	/// <summary>
	/// Třída reprezentující RSS prvek "image"
	/// </summary>
	public class RssImage
	{
		#region Fields

		private string url;
		private string title;
		private string link;
		private string width;
		private string height;
		private string description;

		/// <summary>
		/// URL of a GIF, JPEG or PNG image that represents the channel.
		/// REQUIRED.
		/// </summary>
		[XmlElement("url")]
		public string Url
		{
			get
			{
				return url;
			}
			set
			{
				url = value;
			}
		}

		/// <summary>
		/// Describes the image, it's used in the ALT attribute of the HTML "img" tag when the channel is rendered in HTML.
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
		/// The URL of the site, when the channel is rendered, the image is a link to the site.
		/// REQUIRED.
		/// </summary>
		/// <remarks>
		/// Note, in practice the image "title" and "link" should have the same value as the channel's "title" and "link". 
		/// </remarks>
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
		/// Width of the image in pixels.
		/// Maximum value for width is 144, default value is 88.
		/// </summary>
		[XmlElement("width")]
		public string Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		/// <summary>
		/// Height of the image in pixels.
		/// Maximum value for height is 400, default value is 31.
		/// </summary>
		[XmlElement("height")]
		public string Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		/// <summary>
		/// Contains text that is included in the TITLE attribute of the link formed around the image in the HTML rendering.
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

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor musí být kvůli XmlSerializeru.
		/// Nutné zadat Url, Title a Link.
		/// </summary>
		public RssImage()
		{
		}

		/// <summary>
		/// Constructor s povinnými prvky elementu Image.
		/// </summary>
		public RssImage(string url, string title, string link)
		{
			this.Url = url;
			this.Title = title;
			this.Link = link;
		}

		#endregion
	}
}
