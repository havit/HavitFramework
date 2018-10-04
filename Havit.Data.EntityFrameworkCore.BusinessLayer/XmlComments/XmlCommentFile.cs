using System.Collections.Generic;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments
{
	public class XmlCommentFile
	{
		public List<XmlCommentType> Types { get; } = new List<XmlCommentType>();
	}
}