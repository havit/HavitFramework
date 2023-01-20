using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Havit.Services.FileStorage
{
	/// <summary>
	/// Detailed file information.
	/// </summary>
	public class FileInfo
	{
		/// <summary>
		/// File name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Date of last modification.
		/// </summary>
		public DateTime LastModifiedUtc { get; set; }

		/// <summary>
		/// Size of file. In bytes.
		/// </summary>
		public long Size { get; set; }

		/// <summary>
		/// Content Type.
		/// </summary>
		public string ContentType { get; set; }
	}
}
