using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments
{
	public class XmlCommentType : XmlCommentMember
	{
		public List<XmlCommentMember> Properties { get; } = new List<XmlCommentMember>();

		public List<XmlCommentMember> Methods { get; } = new List<XmlCommentMember>();

		public XmlCommentType(string name)
			: base(name)
		{
		}
	}
}