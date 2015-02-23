using Havit.Diagnostics.Contracts;
using Havit.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace WebApplicationTest.Services
{
	[ServiceContract(Namespace = "")]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class AutoCompleteTextBoxService
	{
		[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		public GetSuggestionsResult GetSuggestions(string query = "")
		{
			List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("1", "Praha" ),
				new KeyValuePair<string, string>("2", "Brno" ),
				new KeyValuePair<string, string>("3", "Ostrava" ),
				new KeyValuePair<string, string>("4", "Olomouc" ),
				new KeyValuePair<string, string>("5", "Olbramovice" ),
				new KeyValuePair<string, string>("6", "Olbramovice" ),
				new KeyValuePair<string, string>("7", "Olbramovice" ),
				new KeyValuePair<string, string>("8", "Olbramovice" ),
				new KeyValuePair<string, string>("9", "Olbramovice" ),
				new KeyValuePair<string, string>("10", "Olbramovice" ),
				new KeyValuePair<string, string>("11", "Olbramovice" )
			};

			GetSuggestionsResult result = new GetSuggestionsResult();
			result.Fill<KeyValuePair<string, string>>(data.Where(i => i.Value.StartsWith(query, StringComparison.InvariantCultureIgnoreCase)), i => i.Key, i => i.Value);
			return result;
		}
	}
}
