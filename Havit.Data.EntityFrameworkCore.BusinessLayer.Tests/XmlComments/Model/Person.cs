﻿namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.XmlComments.Model;

/// <summary>
/// Person object
/// </summary>
public class Person
{
	/// <summary>
	/// Primary key of <see cref="Person"/> object.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// First name
	/// </summary>
	public string FirstName { get; set; }

	/// <summary>
	/// Last name
	/// </summary>
	public string LastName { get; set; }

	/// <summary>
	/// Birthday of the person.
	/// </summary>
	public DateTime Birthday { get; set; }

	/// <summary>
	/// Concatenates first and last name of the person.
	/// </summary>
	/// <returns>Person's full name.</returns>
	public string GetFullName()
	{
		return FirstName + " " + LastName;
	}
}