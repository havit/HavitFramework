namespace Havit.AspNetCore.Mvc.ErrorToJson.Services
{
	/// <summary>
	/// Result data.
	/// </summary>
	public class ResultData
	{
		/// <summary>
		/// Object to be returned.
		/// </summary>
		public object Data { get; set; }

		/// <summary>
		/// Http response status code.
		/// </summary>
		public int StatusCode { get; set; }

		/// <summary>
		/// Indicates whether the exception should be concidered as handled.
		/// </summary>
		public bool ExceptionHandled { get; set; }
	}
}