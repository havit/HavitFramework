namespace Havit.Data.Entity.Tests.Validators.Infrastructure.Model;

public class WithNestedEnumOtherClass
{
	public int Id { get; set; }

	public enum Other
	{
		One, Two, Three
	}
}
