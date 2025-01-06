using System.Xml.Linq;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Projects;

public class Project : IProject
{
	public const string ModelProjectKey = nameof(ModelProjectKey);
	public const string MetadataProjectKey = nameof(MetadataProjectKey);
	public const string DataLayerProjectKey = nameof(DataLayerProjectKey);

	protected XDocument Content { get; }
	protected string Filename { get; }

	public Project(string filename, XDocument content)
	{
		Filename = filename;
		Content = content;
	}

	public string GetProjectRootPath()
	{
		return Path.GetDirectoryName(Filename);
	}

	public string GetProjectRootNamespace()
	{
		// support for default root namespace:
		// Microsoft.NET.Sdk.props
		/*
		  <PropertyGroup>
			<OutputType Condition=" '$(OutputType)' == '' ">Library</OutputType>
			<FileAlignment Condition=" '$(FileAlignment)' == '' ">512</FileAlignment>
			<ErrorReport Condition=" '$(ErrorReport)' == '' ">prompt</ErrorReport>
			<AssemblyName Condition=" '$(AssemblyName)' == '' ">$(MSBuildProjectName)</AssemblyName>
			<RootNamespace Condition=" '$(RootNamespace)' == '' ">$(MSBuildProjectName)</RootNamespace>
			<Deterministic Condition=" '$(Deterministic)' == '' ">true</Deterministic>
		  </PropertyGroup>
		 */

		return GetProjectRootNamespaceCore("") ?? Path.GetFileNameWithoutExtension(Filename);
	}

	protected string GetProjectRootNamespaceCore(XNamespace @namespace)
	{
		return (string)Content.Root
			.Elements(@namespace + "PropertyGroup")
			.Elements(@namespace + "RootNamespace")
			.FirstOrDefault();
	}

}