using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Havit.Business.BusinessLayerToEntityFrameworkGenerator.Generators;

public class AttributeStringBuilder
{
	public string AttributeName { get; }

        public List<string> Parameters { get; }

	public Dictionary<string, string> NamedParameters { get; }

	public AttributeStringBuilder(string attributeName)
	{
		AttributeName = attributeName;

		NamedParameters = new Dictionary<string, string>();
            Parameters = new List<string>();
	}

	public AttributeStringBuilder AddParameter(string value)
	{
		Parameters.Add(value);
		return this;
	}

	public AttributeStringBuilder AddParameter(string name, string value)
	{
		NamedParameters.Add(name, value);
		return this;
	}

	public override string ToString()
        {
            var allParameters = Parameters.Concat(NamedParameters.Select(p => $"{p.Key} = {p.Value}"));

            string attribute = $"[{AttributeName}";
		if (NamedParameters.Count > 0 || Parameters.Count > 0)
            {
                attribute += $"({string.Join(", ", allParameters)})";
            }
		attribute += "]";
		return attribute;
	}
}