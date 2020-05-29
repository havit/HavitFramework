using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
		public static async IAsyncEnumerable<IListBlobItem> ListBlobsAsync(this CloudBlobContainer cloudBlobContainer, string prefix, bool useFlatBlobListing)
		{
			BlobResultSegment response = await cloudBlobContainer.ListBlobsSegmentedAsync(prefix, useFlatBlobListing, BlobListingDetails.None, null, null, null, null, CancellationToken.None).ConfigureAwait(false);

			foreach (IListBlobItem listBlobItem in response.Results)
			{
				yield return listBlobItem;
			}

			BlobContinuationToken continuationToken = response.ContinuationToken;

			while (continuationToken != null)
			{
				response = await cloudBlobContainer.ListBlobsSegmentedAsync(continuationToken).ConfigureAwait(false);

				foreach (IListBlobItem listBlobItem in response.Results)
				{
					yield return listBlobItem;
				}

				continuationToken = response.ContinuationToken;
			}
		}
	}
}
