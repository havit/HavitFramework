namespace Havit.PersonalDataInterchange;

/// <summary>
/// Base class for representing a document in a personal data interchange format.
/// </summary>
public abstract class VDocumentBase
{
	/// <summary>
	/// Version of the document.
	/// </summary>
	public abstract double Version { get; }

	/// <summary>
	/// Product ID of the document.
	/// </summary>
	public string ProductID { get; set; }

	/// <summary>
	/// Content type (MIME type) of the document.
	/// </summary>
	public abstract string ContentType { get; }

	/// <summary>
	/// Writes the document to a stream.
	/// </summary>
	/// <param name="writer">The stream writer to write the document to.</param>
	public abstract void WriteToStream(StreamWriter writer);
}
