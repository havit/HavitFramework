namespace Havit.Services.TimeServices.Fakes;

public class FakeTimeService : ITimeService
{
	private readonly Func<DateTime> nowFunc;

	public FakeTimeService(DateTime now) : this(() => now)
	{
		// NOOP
	}

	public FakeTimeService(Func<DateTime> nowFunc)
	{
		this.nowFunc = nowFunc;
	}

	public DateTime GetCurrentDate()
	{
		return GetCurrentTime().Date;
	}

	public DateTime GetCurrentTime()
	{
		return nowFunc();
	}
}
