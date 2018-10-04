namespace Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments
{
	public class XmlMemberTag
	{
		public string Name { get; }

		public string Content { get; }

		public XmlMemberTag(string name, string content)
		{
			Name = name;
			Content = content;
		}
	}
}