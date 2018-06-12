using System;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	/// <summary>
	/// Pomocné metody pro práci s aplikačním časem.
	/// </summary>
	public static class ApplicationTimeHelper
	{
		#region GetApplicationCurrentTimeExpression
		/// <summary>
		/// Vrátí výraz pro získání aktuálního aplikačního času.
		/// </summary>
		public static string GetApplicationCurrentTimeExpression()
		{
			if (String.IsNullOrEmpty(_applicationCurrentTimeExpression))
			{
				_applicationCurrentTimeExpression = ExtendedPropertiesHelper.GetString(ExtendedPropertiesKey.FromDatabase(), "ApplicationCurrentTimeExpression");
				if (String.IsNullOrEmpty(_applicationCurrentTimeExpression))
				{
					_applicationCurrentTimeExpression = "System.DateTime.Now";
				}
			}
			return _applicationCurrentTimeExpression;
		}
		private static string _applicationCurrentTimeExpression;
		#endregion

	}
}
