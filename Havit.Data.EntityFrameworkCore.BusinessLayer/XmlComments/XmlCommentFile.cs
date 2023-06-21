using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Havit.Diagnostics.Contracts;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.XmlComments;

public class XmlCommentFile
{
	public List<XmlCommentType> Types { get; } = new List<XmlCommentType>();

	public XmlCommentMember FindMethod(MethodInfo methodInfo)
	{
		Contract.Requires<ArgumentNullException>(methodInfo != null);

		XmlCommentType xmlCommentType = FindType(methodInfo);
		return xmlCommentType?.Methods.FirstOrDefault(member => member.Name == (xmlCommentType.Name + "." + methodInfo.Name));
	}

	private XmlCommentType FindType(MethodInfo methodInfo)
	{
		string preprocessedTypeName = methodInfo.DeclaringType.FullName.Replace('+', '.');
		XmlCommentType xmlCommentType = Types.FirstOrDefault(t => t.Name == preprocessedTypeName);
		return xmlCommentType;
	}
}