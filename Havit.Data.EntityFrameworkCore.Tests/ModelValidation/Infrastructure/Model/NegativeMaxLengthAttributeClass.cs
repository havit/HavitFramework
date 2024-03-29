﻿using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;

public class NegativeMaxLengthAttributeClass
{
	public int Id { get; set; }

	[MaxLength(-10)]
	public string Value { get; set; }
}
