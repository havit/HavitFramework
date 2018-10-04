using System;
using System.Collections.Generic;
using System.Linq;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments
{
	public class XmlCommentMember
	{
		public string Name { get; }

		public List<XmlMemberTag> Tags { get; } = new List<XmlMemberTag>();

		public string Summary => Tags.FirstOrDefault(t => string.Equals(t.Name, "summary", StringComparison.OrdinalIgnoreCase))?.Content;

		public XmlCommentMember(string name)
		{
			Name = name;
		}
	}
}