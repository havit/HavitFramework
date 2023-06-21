using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ModelExtensions.ExtendedProperties.Attributes;
using Havit.Data.EntityFrameworkCore.BusinessLayer.ExtendedProperties;
using Havit.Data.EntityFrameworkCore.Migrations.ModelExtensions.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Havit.Data.EntityFrameworkCore.BusinessLayer.Tests.ModelExtensions;

public class ViewEndToEndTests
{
	[TestClass]
	public class ViewWithMsDescriptionExtendedProperty
	{
		[Attach(nameof(Invoice))]
		public class InvoiceViews : ViewModelExtender
		{
			/// <summary>
			/// Gets all unpaid invoices.
			/// </summary>
			public ViewModelExtension UnpaidInvoices()
			{
				return new ViewModelExtension { CreateSql = "", ViewName = nameof(UnpaidInvoices) };
			}
		}

		[Table("Dummy")]
		private class Invoice
		{
			public int Id { get; set; }
		}

		//[TestMethod]
		// TODO: support for XML comments / MS_Description on views
		public void ViewModelExtensions_EndToEnd_ViewWithMsDescriptionExtendedProperty()
		{
			var source = new EndToEndTestModelExtensionsDbContext<Invoice>(typeof(InvoiceViews));
			var model = source.Model;

			IDictionary<string, string> extendedProperties = model.GetExtendedProperties();
			Assert.AreEqual("Gets all unpaid invoices.", extendedProperties.FirstOrDefault(a => a.Key.EndsWith("MS_Description")).Value?.Trim());
		}
	}
}