using System.ComponentModel.DataAnnotations;

namespace Havit.Data.EntityFrameworkCore.Tests.ModelValidation.Infrastructure.Model;

public class DateTimeIdClass
{
	[Key]
	public DateTime Id { get; set; }
}
