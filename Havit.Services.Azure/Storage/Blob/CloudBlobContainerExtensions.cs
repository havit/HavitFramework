using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Havit.Services.Azure.Storage.Blob
{
	/// <summary>
	/// Extension metody k CloudBlobContainer.
	/// </summary>
	public static class CloudBlobContainerExtensions
	{
		/// <summary>
		/// Načte seznam blobů v containeru.
		/// Pod pokličkou používá ListBlobsSegmentedAsync a v případě vyskytu více než 5000 položek zřetězí volání, aby došlo k vrácení všech záznamů.
		/// </summary>
		public static async Task<List<IListBlobItem>> ListBlobsAsync(this CloudBlobContainer cloudBlobContainer, string prefix, bool useFlatBlobListing)
		{
			List<IListBlobItem> results = new List<IListBlobItem>();
			BlobResultSegment response = await cloudBlobContainer.ListBlobsSegmentedAsync(prefix, useFlatBlobListing, BlobListingDetails.None, null, null, null, null, CancellationToken.None).ConfigureAwait(false);
			results.AddRange(response.Results);
			BlobContinuationToken continuationToken = response.ContinuationToken;

			while (continuationToken != null)
			{
				response = await cloudBlobContainer.ListBlobsSegmentedAsync(continuationToken).ConfigureAwait(false);
				results.AddRange(response.Results);
				continuationToken = response.ContinuationToken;
			}

			return results;
		}
	}
}
