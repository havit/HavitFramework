﻿namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;

public class WithNestedEnumOtherClass
{
	public int Id { get; set; }

	public enum Other
	{
		One, Two, Three
	}
}
