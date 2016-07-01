namespace Havit.Text
{
	/// <summary>
	/// Format file size as text, main goal is to show same size as Internet Explorer when downloading file.
	/// </summary>
	public interface IFileSizeToTextService
	{
		/// <summary>
		/// Returns file size as text, main goal is to show same size as Internet Explorer when downloading file.
		/// </summary>
		string GetFileSizeToText(long size);
	}
}