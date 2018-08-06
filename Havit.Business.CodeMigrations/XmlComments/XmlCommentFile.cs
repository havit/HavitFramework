using System.Collections.Generic;

namespace Havit.Business.CodeMigrations.XmlComments
{
	public class XmlCommentFile
	{
		public List<XmlCommentType> Types { get; } = new List<XmlCommentType>();
	}
}