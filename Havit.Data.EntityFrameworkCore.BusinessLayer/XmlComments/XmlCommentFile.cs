using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments
{
	public class XmlCommentFile
	{
		public List<XmlCommentType> Types { get; } = new List<XmlCommentType>();

		public XmlCommentMember GetMember(MemberInfo memberInfo)
		{
			Contract.Requires<ArgumentNullException>(memberInfo != null);

			string preprocessedTypeName = memberInfo.DeclaringType.FullName.Replace('+', '.');
			XmlCommentType xmlCommentType = Types.FirstOrDefault(t => t.Name == preprocessedTypeName);

			return xmlCommentType?.Properties.FirstOrDefault(member => member.Name == (xmlCommentType.Name + "." + memberInfo.Name));
		}
	}
}