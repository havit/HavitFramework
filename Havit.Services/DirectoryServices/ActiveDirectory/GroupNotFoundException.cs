using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Havit.Services.DirectoryServices.ActiveDirectory;

/// <summary>
/// Exception thrown when group not found.
/// </summary>
public class GroupNotFoundException : Exception
{
	/// <summary>
	/// Group which was not found.
	/// </summary>
	public string GroupName { get; private set; }

	/// <summary>
	/// Gets a message that describes the current exception.
	/// </summary>
	public override string Message
	{
		get
		{
			return String.Format("Group {0} not found.", this.GroupName);
		}
	}

	/// <summary>
	/// Konstruktor.
	/// </summary>
	public GroupNotFoundException(string groupName)
	{
		this.GroupName = groupName;
	}
}
