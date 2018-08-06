using System.Collections.Generic;
using System.Linq;

namespace Havit.Business.CodeMigrations.XmlComments
{
	public class XmlCommentMember
	{
		public string Name { get; }

		public List<XmlMemberTag> Tags { get; } = new List<XmlMemberTag>();

		public string Summary => Tags.FirstOrDefault(t => t.Name == "sumary")?.Content;

		public XmlCommentMember(string name)
		{
			Name = name;
		}
	}
}