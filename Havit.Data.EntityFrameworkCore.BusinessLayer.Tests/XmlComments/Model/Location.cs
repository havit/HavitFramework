﻿namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model;

/// <summary>
/// Location object
/// </summary>
public class Location
{
	/// <summary>
	/// Primary key of <see cref="Location"/> object.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Summary tag with
	/// new lines and whitespace.  
	/// </summary>
	public string Description { get; set; }
}