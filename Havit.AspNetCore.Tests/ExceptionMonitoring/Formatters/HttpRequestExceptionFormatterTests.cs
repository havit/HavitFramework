using Havit.AspNetCore.ExceptionMonitoring.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.AspNetCore.Tests.ExceptionMonitoring.Formatters;

[TestClass]
public class HttpRequestExceptionFormatterTests
{
	[TestMethod]
	public void HttpRequestExceptionFormatter_CanBeUsedWithoutHttpContextAccessorDependency()
	{
		// Arrange
		ServiceCollection services = new ServiceCollection();
		services.AddTransient<IExceptionFormatter, HttpRequestExceptionFormatter>();
		using (ServiceProvider serviceProvider = services.BuildServiceProvider())
		{
			// Act
			var service = serviceProvider.GetRequiredService<IExceptionFormatter>();
			service.FormatException(new ApplicationException());

			// Assert
			// no exception was thrown
		}
	}
}
