using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators
{
	public class AttributeStringBuilder
	{
		public string AttributeName { get; }

		public Dictionary<string, string> Parameters { get; }

		public AttributeStringBuilder(string attributeName)
		{
			AttributeName = attributeName;

			Parameters = new Dictionary<string, string>();
		}

		public AttributeStringBuilder AddParameter(string name, string value)
		{
			Parameters.Add(name, value);
			return this;
		}

		public override string ToString()
		{
			string attribute = $"[{AttributeName}";
			if (Parameters.Count > 0)
			{
				attribute += $"({string.Join(", ", Parameters.Select(p => $"{p.Key} = {p.Value}"))})";
			}
			attribute += "]";
			return attribute;
		}
	}
}