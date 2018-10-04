using System.IO;
using System.Xml.Linq;

namespace Havit.Data.EntityFrameworkCore.CodeGenerator.Services
{
	public class NetCoreProject : ProjectBase
	{
		public NetCoreProject(string filename, XDocument content) 
			: base(filename, content)
		{
		}

		public override void AddOrUpdate(string filename)
		{
			// noop in .NET Core project
		}

		public override string GetProjectRootNamespace()
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

		public override string[] GetUnusedGeneratedFiles()
		{
			return new string[0];
		}

		public override void RemoveUnusedGeneratedFiles()
		{
			// noop in .NET Core project
		}

		public override void SaveChanges()
		{
			// noop in .NET Core project
		}
	}
}