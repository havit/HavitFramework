namespace Havit.PersonalDataInterchange;

/// <summary>
/// Contact card
/// </summary>
public class VCard : VDocumentBase
{
	/// <summary>
	/// vCard version (currently 2.1)
	/// </summary>
	public override double Version => 2.1;

	/// <summary>
	/// Contact name
	/// </summary>
	public ContactName N { get; set; }

	/// <summary>
	/// Formatted full name of the contact
	/// </summary>
	public string FN { get; set; }

	/// <summary>
	/// Role of the contact (director, manager, etc.)
	/// </summary>
	public string Title { get; set; }

	/// <summary>
	/// Organization name
	/// </summary>
	public string Org { get; set; }

	/// <summary>
	/// Contact email
	/// </summary>
	public string Email { get; set; }

	/// <summary>
	/// Preferred contact email
	/// </summary>
	public string EmailPref { get; set; }

	/// <summary>
	/// Mobile phone number
	/// </summary>
	public string TelCell { get; set; }

	/// <summary>
	/// Work fax number
	/// </summary>
	public string Fax { get; set; }

	/// <summary>
	/// Company website address
	/// </summary>
	public string Url { get; set; }

	/// <summary>
	/// Work phone number
	/// </summary>
	public string TelWork { get; set; }

	/// <summary>
	/// Home phone number
	/// </summary>
	public string TelHome { get; set; }

	/// <summary>
	/// Work address
	/// </summary>
	public string AdrWork { get; set; }

	/// <summary>
	/// Is work address encoded as QuotedPrintable?
	/// </summary>
	public bool IsAdrWorkAsQuotedPrintable { get; set; }

	/// <summary>
	/// Home address
	/// </summary>
	public string AdrHome { get; set; }

	/// <summary>
	/// Is home address encoded as QuotedPrintable?
	/// </summary>
	public bool IsAdrHomeAsQuotedPrintable { get; set; }

	/// <summary>
	/// Note
	/// </summary>
	public string Note { get; set; }

	/// <summary>
	/// Mime-type for vCard
	/// </summary>
	public override string ContentType => "text/x-vCard";

	/// <summary>
	/// Writes the vCard content to a stream
	/// </summary>
	/// <param name="writer">The stream writer</param>
	public override void WriteToStream(StreamWriter writer)
	{
		writer.WriteLine("BEGIN:VCARD");
		writer.WriteLine($"VERSION:{Version.ToString().Replace(',', '.')}");
		writer.WriteLine($"N;CHARSET=windows-1250:{N.LastName};{N.FirstName};{N.MiddleName};{N.Title};{N.Suffix}");
		if (!string.IsNullOrEmpty(FN))
		{
			writer.WriteLine($"FN;CHARSET=windows-1250:{FN}");
		}
		if (!string.IsNullOrEmpty(Title))
		{
			writer.WriteLine($"TITLE;CHARSET=windows-1250:{Title}");
		}
		if (!string.IsNullOrEmpty(Org))
		{
			writer.WriteLine($"ORG;CHARSET=windows-1250:{Org}");
		}
		if (!string.IsNullOrEmpty(EmailPref))
		{
			writer.WriteLine($"EMAIL;PREF;INTERNET:{EmailPref}");
		}
		if (!string.IsNullOrEmpty(Email))
		{
			writer.WriteLine($"EMAIL;INTERNET:{Email}");
		}
		if (!string.IsNullOrEmpty(TelCell))
		{
			writer.WriteLine($"TEL;CELL;VOICE:{TelCell}");
		}
		if (!string.IsNullOrEmpty(Fax))
		{
			writer.WriteLine($"TEL;WORK;FAX:{Fax}");
		}
		if (!string.IsNullOrEmpty(Url))
		{
			writer.WriteLine($"URL;WORK:{Url}");
		}
		if (!string.IsNullOrEmpty(TelWork))
		{
			writer.WriteLine($"TEL;WORK;VOICE:{TelWork}");
		}
		if (!string.IsNullOrEmpty(TelHome))
		{
			writer.WriteLine($"TEL;HOME;VOICE:{TelHome}");
		}
		if (!string.IsNullOrEmpty(AdrWork))
		{
			if (IsAdrWorkAsQuotedPrintable)
			{
				writer.WriteLine($"ADR;WORK;PREF;CHARSET=windows-1250;ENCODING=QUOTED-PRINTABLE:;;{AdrWork}");
			}
			else
			{
				writer.WriteLine($"ADR;WORK;PREF;CHARSET=windows-1250:;;{AdrWork}");
			}
		}
		if (!string.IsNullOrEmpty(AdrHome))
		{
			if (IsAdrHomeAsQuotedPrintable)
			{
				writer.WriteLine($"ADR;HOME;CHARSET=windows-1250;ENCODING=QUOTED-PRINTABLE:;;{AdrHome}");
			}
			else
			{
				writer.WriteLine($"ADR;HOME;CHARSET=windows-1250:;;{AdrHome}");
			}
		}
		if (!string.IsNullOrEmpty(Note))
		{
			string encodedNote = Note.Replace("=", "=3D").Replace("\r\n", "=0D=0A");
			writer.WriteLine($"NOTE;CHARSET=windows-1250;ENCODING=QUOTED-PRINTABLE:{encodedNote}");
		}
		writer.WriteLine("END:VCARD");
	}

	/// <summary>
	/// Contact name
	/// </summary>
	public class ContactName
	{
		/// <summary>
		/// Title of the contact
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// First name of the contact
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		/// Middle name of the contact
		/// </summary>
		public string MiddleName { get; set; }

		/// <summary>
		/// Last name of the contact
		/// </summary>
		public string LastName { get; set; }

		/// <summary>
		/// Suffix of the contact
		/// </summary>
		public string Suffix { get; set; }
	}
}
