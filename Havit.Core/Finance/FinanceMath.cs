using Havit.Diagnostics.Contracts;
using System;

namespace Havit.Finance
{
	/// <summary>
	/// Finance mathematics.
	/// </summary>
	public static class FinanceMath
	{
		/// <summary>
		/// Calculate the total payment (principal and interest) required to settle a loan or an investment with a fixed interest rate over a specific time period. 
		/// </summary>
		/// <param name="interestRate">The interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="numberOfPeriods">The total number of payment periods in the annuity. For example, if you make monthly payments on a four-year car loan, your loan has a total of 4 × 12 (or 48) payment periods.</param>
		/// <param name="presentValue">The present value (or lump sum) that a series of payments to be paid in the future is worth now. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make.</param>
		/// <param name="futureValue">The future value or cash balance you want after you have made the final payment. For example, the future value of a loan is $0 because that is its value after the final payment. However, if you want to save $50,000 during 18 years for your child's education, then $50,000 is the future value. If omitted, 0 is assumed.</param>
		/// <param name="dueDate">Object of type <see cref="DueDate"/> that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <remarks>
		/// https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualbasic.financial.pmt (ILSpy)
		/// https://www.wikihow.com/Calculate-Mortgage-Payments
		/// https://support.office.com/en-us/article/pmt-function-0214da64-9a63-4996-bc20-214433fa6441
		/// https://www.quora.com/What-would-be-the-mathematical-equivalent-for-PMT-Excel-Formula-PMT
		/// </remarks>
		public static double Pmt(double interestRate, int numberOfPeriods, double presentValue, double futureValue = 0, DueDate dueDate = DueDate.EndOfPeriod)
		{
			Contract.Requires<ArgumentException>(numberOfPeriods != 0, nameof(numberOfPeriods));

			double result;
			if (interestRate == 0.0)
			{
				result = (-futureValue - presentValue) / numberOfPeriods;
			}
			else
			{
				double num;
				if (dueDate != DueDate.EndOfPeriod)
				{
					num = 1.0 + interestRate;
				}
				else
				{
					num = 1.0;
				}
				double num2 = Math.Pow(interestRate + 1.0, numberOfPeriods);
				result = (-futureValue - presentValue * num2) / (num * (num2 - 1.0)) * interestRate;
			}
			return result;
		}

		/// <summary>
		/// Calculate specifying the number of periods for an annuity based on periodic fixed payments and a fixed interest rate.
		/// </summary>
		/// <param name="rate">The interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="paymentAmount">The payment to be made each period. Payments usually contain principal and interest that does not change over the life of the annuity.</param>
		/// <param name="presentValue">The present value, or value today, of a series of future payments or receipts. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make.</param>
		/// <param name="futureValue">The future value or cash balance you want after you have made the final payment. For example, the future value of a loan is $0 because that is its value after the final payment. However, if you want to save $50,000 over 18 years for your child's education, then $50,000 is the future value. If omitted, 0 is assumed.</param>
		/// <param name="due">Object of type DueDate that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <remarks>
		/// https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualbasic.financial.nper
		/// </remarks>
		public static double NPer(double rate, double paymentAmount, double presentValue, double futureValue = 0.0, DueDate due = DueDate.EndOfPeriod)
		{
			Contract.Requires<ArgumentException>(rate > -1.0, nameof(rate));

			double num;
			if (rate == 0.0)
			{
				Contract.Requires<ArgumentException>(paymentAmount != 0.0, nameof(paymentAmount));

				return (-(presentValue + futureValue) / paymentAmount);
			}
			if (due != DueDate.EndOfPeriod)
			{
				num = (paymentAmount * (1.0 + rate)) / rate;
			}
			else
			{
				num = paymentAmount / rate;
			}
			double d = -futureValue + num;
			double num4 = presentValue + num;
			if ((d < 0.0) && (num4 < 0.0))
			{
				d = -1.0 * d;
				num4 = -1.0 * num4;
			}
			else if ((d <= 0.0) || (num4 <= 0.0))
			{
				throw new ArgumentException("Cannot calculate NPer financial formula");
			}
			double num2 = rate + 1.0;
			return ((Math.Log(d) - Math.Log(num4)) / Math.Log(num2));
		}

		/// <summary>
		/// Returns a value specifying the future value of an annuity based on periodic, fixed payments and a fixed interest rate.
		/// </summary>
		/// <param name="interestRate">The interest rate per period. For example, if you get a car loan at an annual percentage rate (APR) of 10 percent and make monthly payments, the rate per period is 0.1/12, or 0.0083.</param>
		/// <param name="numberOfPeriods"> The total number of payment periods in the annuity. For example, if you make monthly payments on a four-year car loan, your loan has a total of 4 x 12 (or 48) payment periods.</param>
		/// <param name="paymentAmount">The payment to be made each period. Payments usually contain principal and interest that doesn't change over the life of the annuity.</param>
		/// <param name="presentValue">The present value (or lump sum) of a series of future payments. For example, when you borrow money to buy a car, the loan amount is the present value to the lender of the monthly car payments you will make. If omitted, 0 is assumed.</param>
		/// <param name="due">Object of type <see cref="DueDate"/> that specifies when payments are due. This argument must be either DueDate.EndOfPeriod if payments are due at the end of the payment period, or DueDate.BegOfPeriod if payments are due at the beginning of the period. If omitted, DueDate.EndOfPeriod is assumed.</param>
		/// <remarks>
		/// https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualbasic.financial.fv?view=netframework-4.7.2
		/// </remarks>
		public static double FV(double interestRate, double numberOfPeriods, double paymentAmount, double presentValue = 0.0, DueDate due = DueDate.EndOfPeriod)
		{
			if (interestRate == 0.0)
			{
				return 0.0 - presentValue - paymentAmount * numberOfPeriods;
			}
			double num = ((due == DueDate.EndOfPeriod) ? 1.0 : (1.0 + interestRate));
			double num2 = Math.Pow(1.0 + interestRate, numberOfPeriods);
			return (0.0 - presentValue) * num2 - paymentAmount / interestRate * num * (num2 - 1.0);
		}
	}
}
