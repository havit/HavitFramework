using Havit.Finance;

namespace Havit.FinanceTests;

[TestClass]
public class FinanceMathTests
{
	[TestMethod]
	[DataRow(6 / 100.0 / 12.0, 1, 1_000_000, -1_005_000)]
	[DataRow(2.3 / 100.0 / 12.0, 180, 1_000_000, -6574.154)]
	[DataRow(2.3 / 100.0 / 12.0, 180, 78_000, -512.78401)]
	public void FinanceMath_Pmt_Simple5Decimals(double interestRate, int numberOfPeriods, double presentValue, double expected)
	{
		var result = FinanceMath.Pmt(interestRate, numberOfPeriods, presentValue);

		Assert.AreEqual(expected, Math.Round(result, 5));
	}

	[TestMethod]
	[DataRow(6 / 100.0 / 12.0, -1_005_000, 1_000_000, 0, 1)]
	[DataRow(12 / 100.0 / 12.0, -100, -1_000, 10_000, 60.08212)]
	public void FinanceMath_Nper_Simple5Decimals(double interestRate, int paymentAmount, double presentValue, double futureValue, double expected)
	{
		var result = FinanceMath.NPer(interestRate, paymentAmount, presentValue, futureValue);

		Assert.AreEqual(expected, Math.Round(result, 5));
	}
}
