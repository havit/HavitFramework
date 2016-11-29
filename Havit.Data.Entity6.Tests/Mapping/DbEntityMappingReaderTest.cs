using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Havit.Data.Entity.Mapping;
using Havit.Data.Entity.Tests.Infrastructure;
using Havit.Data.Entity.Tests.Infrastructure.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.Entity.Tests.Mapping
{
	[TestClass]
	public class DbEntityMappingReaderTest
	{
		[TestMethod]
		public void DbEntityMappingReader_GetMappedEntities()
		{
			// Arrange
			DbEntityMappingReader reader = new DbEntityMappingReader();

			// Act
			List<MappedEntity> mappedEntities = reader.GetMappedEntities(new MasterChildDbContext());

			// Assert
			Assert.AreEqual(2, mappedEntities.Count, "length");
			MappedEntity masterEntity = mappedEntities.Single(item => item.Type == typeof(Master));
			MappedEntity childEntity = mappedEntities.Single(item => item.Type == typeof(Child));

			Assert.AreEqual(2, masterEntity.DeclaredProperties.Count, "length");

		}
	}
}
