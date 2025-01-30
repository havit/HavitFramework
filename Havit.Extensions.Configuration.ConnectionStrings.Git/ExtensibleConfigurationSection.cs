using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Havit.Extensions.Configuration.ConnectionStrings.Git;

public class ExtensibleConfigurationSection : IConfigurationSection
{
	private readonly IConfigurationRoot root;
	private readonly Lazy<string> keyProvider;

	public ExtensibleConfigurationSection(IConfigurationRoot root, string path)
	{
		Path = path;
		this.root = root;

		keyProvider = new Lazy<string>(() => ConfigurationPath.GetSectionKey(this.Path));
	}

	public IConfigurationSection GetSection(string key)
	{
		return this.root.GetSection(ConfigurationPath.Combine(Path, key));
	}

	public IEnumerable<IConfigurationSection> GetChildren()
	{
		return root.GetChildren();
	}

	public IChangeToken GetReloadToken()
	{
		return root.GetReloadToken();
	}

	public string this[string key]
	{
		get => root[ConfigurationPath.Combine(Path, key)];
		set => root[ConfigurationPath.Combine(Path, key)] = value;
	}

	public string Key => keyProvider.Value;

	public string Path { get; }

	public string Value { get; set; }
}