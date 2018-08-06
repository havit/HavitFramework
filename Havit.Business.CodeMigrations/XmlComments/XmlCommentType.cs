using System.Collections.Generic;

namespace Havit.Business.CodeMigrations.XmlComments
{
	public class XmlCommentType : XmlCommentMember
	{
		public List<XmlCommentMember> Properties { get; } = new List<XmlCommentMember>();

		public XmlCommentType(string name) 
			: base(name)
		{
		}
	}
}