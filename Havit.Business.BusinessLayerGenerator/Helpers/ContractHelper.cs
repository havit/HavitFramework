using System;

namespace Havit.Business.BusinessLayerGenerator.Helpers
{
	/// <summary>
	/// Vrací kód pro generování podmínek code contracts.
	/// </summary>
	public static class ContractHelper
	{
		#region GetEnsuresResultNotNull
		/// <summary>
		/// Vrátí kód pro kontrolu, že návratová hodnota není null.
		/// </summary>
		public static string GetEnsuresResultNotNull(string resultType)
		{
			return String.Format("global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<{0}>() != null);", resultType);
		}
		#endregion

		#region GetEnsuresResultCollectionDoesNotContainNull
		/// <summary>
		/// Vrátí kód pro kontrolu, že návratová hodnota není null.
		/// </summary>
		public static string GetEnsuresResultCollectionDoesNotContainNull(string collectionType)
		{
			return String.Format("global::System.Diagnostics.Contracts.Contract.Ensures(global::System.Diagnostics.Contracts.Contract.Result<{0}>().All(resultItem => resultItem != null));", collectionType);
		}
		#endregion

		#region GetContractVerificationAttribute
		/// <summary>
		/// Vrátí kód atributu ContractVerificationAttribute s hodnotou, zda má docházet ke verifikaci.
		/// </summary>
		public static string GetContractVerificationAttribute(bool requiresVerification)
		{
			return String.Format("[System.Diagnostics.Contracts.ContractVerification({0})]", requiresVerification.ToString().ToLower());
		}
		#endregion
	}
}
