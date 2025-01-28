﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Havit.Data.EntityFrameworkCore.Patterns.DependencyInjection;
using Havit.Data.EntityFrameworkCore.Patterns.Infrastructure;
using Havit.Data.Patterns.DataEntries;
using Havit.Data.Patterns.DataSources;
using Havit.Data.Patterns.Infrastructure;
using Havit.Data.Patterns.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Havit.EFCoreTests.DataLayer;

[System.CodeDom.Compiler.GeneratedCode("Havit.Data.EntityFrameworkCore.CodeGenerator", "1.0")]
public static partial class DataLayerServiceExtensions
{
	public static IServiceCollection AddDataLayerServices(this IServiceCollection services, ComponentRegistrationOptions options = null)
	{
		services.AddDataLayerCoreServices(options);
	
		AddDataSources(services);
		AddRepositories(services);
		AddDataEntries(services);
		AddEntityKeyAccessors(services);

		return services;
	}

	private static void AddDataSources(IServiceCollection services)
	{
		services.TryAddTransient<Havit.EFCoreTests.DataLayer.DataSources.IAddressDataSource, Havit.EFCoreTests.DataLayer.DataSources.AddressDbDataSource>();
		services.TryAddTransient<IDataSource<Havit.EFCoreTests.Model.Address>, Havit.EFCoreTests.DataLayer.DataSources.AddressDbDataSource>();

		services.TryAddTransient<Havit.EFCoreTests.DataLayer.DataSources.IBusinessCaseDataSource, Havit.EFCoreTests.DataLayer.DataSources.BusinessCaseDbDataSource>();
		services.TryAddTransient<IDataSource<Havit.EFCoreTests.Model.BusinessCase>, Havit.EFCoreTests.DataLayer.DataSources.BusinessCaseDbDataSource>();

		services.TryAddTransient<Havit.EFCoreTests.DataLayer.DataSources.ICheckedEntityDataSource, Havit.EFCoreTests.DataLayer.DataSources.CheckedEntityDbDataSource>();
		services.TryAddTransient<IDataSource<Havit.EFCoreTests.Model.CheckedEntity>, Havit.EFCoreTests.DataLayer.DataSources.CheckedEntityDbDataSource>();

		services.TryAddTransient<Havit.EFCoreTests.DataLayer.DataSources.ILanguageDataSource, Havit.EFCoreTests.DataLayer.DataSources.LanguageDbDataSource>();
		services.TryAddTransient<IDataSource<Havit.EFCoreTests.Model.Language>, Havit.EFCoreTests.DataLayer.DataSources.LanguageDbDataSource>();

		services.TryAddTransient<Havit.EFCoreTests.DataLayer.DataSources.IModelationDataSource, Havit.EFCoreTests.DataLayer.DataSources.ModelationDbDataSource>();
		services.TryAddTransient<IDataSource<Havit.EFCoreTests.Model.Modelation>, Havit.EFCoreTests.DataLayer.DataSources.ModelationDbDataSource>();

		services.TryAddTransient<Havit.EFCoreTests.DataLayer.DataSources.IPersonDataSource, Havit.EFCoreTests.DataLayer.DataSources.PersonDbDataSource>();
		services.TryAddTransient<IDataSource<Havit.EFCoreTests.Model.Person>, Havit.EFCoreTests.DataLayer.DataSources.PersonDbDataSource>();

		services.TryAddTransient<Havit.EFCoreTests.DataLayer.DataSources.IPropertyWithProtectedMembersDataSource, Havit.EFCoreTests.DataLayer.DataSources.PropertyWithProtectedMembersDbDataSource>();
		services.TryAddTransient<IDataSource<Havit.EFCoreTests.Model.PropertyWithProtectedMembers>, Havit.EFCoreTests.DataLayer.DataSources.PropertyWithProtectedMembersDbDataSource>();

		services.TryAddTransient<Havit.EFCoreTests.DataLayer.DataSources.IStateDataSource, Havit.EFCoreTests.DataLayer.DataSources.StateDbDataSource>();
		services.TryAddTransient<IDataSource<Havit.EFCoreTests.Model.State>, Havit.EFCoreTests.DataLayer.DataSources.StateDbDataSource>();

		services.TryAddTransient<Havit.EFCoreTests.DataLayer.DataSources.IStateLocalizationDataSource, Havit.EFCoreTests.DataLayer.DataSources.StateLocalizationDbDataSource>();
		services.TryAddTransient<IDataSource<Havit.EFCoreTests.Model.StateLocalization>, Havit.EFCoreTests.DataLayer.DataSources.StateLocalizationDbDataSource>();

		services.TryAddTransient<Havit.EFCoreTests.DataLayer.DataSources.IUserDataSource, Havit.EFCoreTests.DataLayer.DataSources.UserDbDataSource>();
		services.TryAddTransient<IDataSource<Havit.EFCoreTests.Model.User>, Havit.EFCoreTests.DataLayer.DataSources.UserDbDataSource>();

	}

	private static void AddRepositories(IServiceCollection services)
	{
		services.TryAddScoped<Havit.EFCoreTests.DataLayer.Repositories.IAddressRepository, Havit.EFCoreTests.DataLayer.Repositories.AddressDbRepository>();
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.Address, System.Int32>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IAddressRepository>());
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.Address>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IAddressRepository>());
	    // TODO: IRepositoryQueryProvider<Havit.EFCoreTests.Model.Address, System.Int32>

		services.TryAddScoped<Havit.EFCoreTests.DataLayer.Repositories.IBusinessCaseRepository, Havit.EFCoreTests.DataLayer.Repositories.BusinessCaseDbRepository>();
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.BusinessCase, System.Int32>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IBusinessCaseRepository>());
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.BusinessCase>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IBusinessCaseRepository>());
	    // TODO: IRepositoryQueryProvider<Havit.EFCoreTests.Model.BusinessCase, System.Int32>

		services.TryAddScoped<Havit.EFCoreTests.DataLayer.Repositories.ICheckedEntityRepository, Havit.EFCoreTests.DataLayer.Repositories.CheckedEntityDbRepository>();
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.CheckedEntity, System.Int32>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.ICheckedEntityRepository>());
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.CheckedEntity>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.ICheckedEntityRepository>());
	    // TODO: IRepositoryQueryProvider<Havit.EFCoreTests.Model.CheckedEntity, System.Int32>

		services.TryAddScoped<Havit.EFCoreTests.DataLayer.Repositories.ILanguageRepository, Havit.EFCoreTests.DataLayer.Repositories.LanguageDbRepository>();
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.Language, System.Int32>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.ILanguageRepository>());
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.Language>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.ILanguageRepository>());
	    // TODO: IRepositoryQueryProvider<Havit.EFCoreTests.Model.Language, System.Int32>

		services.TryAddScoped<Havit.EFCoreTests.DataLayer.Repositories.IModelationRepository, Havit.EFCoreTests.DataLayer.Repositories.ModelationDbRepository>();
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.Modelation, System.Int32>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IModelationRepository>());
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.Modelation>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IModelationRepository>());
	    // TODO: IRepositoryQueryProvider<Havit.EFCoreTests.Model.Modelation, System.Int32>

		services.TryAddScoped<Havit.EFCoreTests.DataLayer.Repositories.IPersonRepository, Havit.EFCoreTests.DataLayer.Repositories.PersonDbRepository>();
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.Person, System.Int32>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IPersonRepository>());
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.Person>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IPersonRepository>());
	    // TODO: IRepositoryQueryProvider<Havit.EFCoreTests.Model.Person, System.Int32>

		services.TryAddScoped<Havit.EFCoreTests.DataLayer.Repositories.IPropertyWithProtectedMembersRepository, Havit.EFCoreTests.DataLayer.Repositories.PropertyWithProtectedMembersDbRepository>();
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.PropertyWithProtectedMembers, System.Int32>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IPropertyWithProtectedMembersRepository>());
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.PropertyWithProtectedMembers>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IPropertyWithProtectedMembersRepository>());
	    // TODO: IRepositoryQueryProvider<Havit.EFCoreTests.Model.PropertyWithProtectedMembers, System.Int32>

		services.TryAddScoped<Havit.EFCoreTests.DataLayer.Repositories.IStateRepository, Havit.EFCoreTests.DataLayer.Repositories.StateDbRepository>();
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.State, System.Int32>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IStateRepository>());
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.State>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IStateRepository>());
	    // TODO: IRepositoryQueryProvider<Havit.EFCoreTests.Model.State, System.Int32>

		services.TryAddScoped<Havit.EFCoreTests.DataLayer.Repositories.IStateLocalizationRepository, Havit.EFCoreTests.DataLayer.Repositories.StateLocalizationDbRepository>();
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.StateLocalization, System.Int32>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IStateLocalizationRepository>());
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.StateLocalization>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IStateLocalizationRepository>());
	    // TODO: IRepositoryQueryProvider<Havit.EFCoreTests.Model.StateLocalization, System.Int32>

		services.TryAddScoped<Havit.EFCoreTests.DataLayer.Repositories.IUserRepository, Havit.EFCoreTests.DataLayer.Repositories.UserDbRepository>();
		services.TryAddScoped<IRepository<Havit.EFCoreTests.Model.User, System.Guid>>(sp => sp.GetRequiredService<Havit.EFCoreTests.DataLayer.Repositories.IUserRepository>());
	    // TODO: IRepositoryQueryProvider<Havit.EFCoreTests.Model.User, System.Guid>

	}

	private static void AddDataEntries(IServiceCollection services)
	{
	}

	private static void AddEntityKeyAccessors(IServiceCollection services)
	{
		services.TryAddTransient<IEntityKeyAccessor<Havit.EFCoreTests.Model.Address, System.Int32>, DbEntityKeyAccessor<Havit.EFCoreTests.Model.Address, System.Int32>>();
		services.TryAddTransient<IEntityKeyAccessor<Havit.EFCoreTests.Model.BusinessCase, System.Int32>, DbEntityKeyAccessor<Havit.EFCoreTests.Model.BusinessCase, System.Int32>>();
		services.TryAddTransient<IEntityKeyAccessor<Havit.EFCoreTests.Model.CheckedEntity, System.Int32>, DbEntityKeyAccessor<Havit.EFCoreTests.Model.CheckedEntity, System.Int32>>();
		services.TryAddTransient<IEntityKeyAccessor<Havit.EFCoreTests.Model.Language, System.Int32>, DbEntityKeyAccessor<Havit.EFCoreTests.Model.Language, System.Int32>>();
		services.TryAddTransient<IEntityKeyAccessor<Havit.EFCoreTests.Model.Modelation, System.Int32>, DbEntityKeyAccessor<Havit.EFCoreTests.Model.Modelation, System.Int32>>();
		services.TryAddTransient<IEntityKeyAccessor<Havit.EFCoreTests.Model.Person, System.Int32>, DbEntityKeyAccessor<Havit.EFCoreTests.Model.Person, System.Int32>>();
		services.TryAddTransient<IEntityKeyAccessor<Havit.EFCoreTests.Model.PropertyWithProtectedMembers, System.Int32>, DbEntityKeyAccessor<Havit.EFCoreTests.Model.PropertyWithProtectedMembers, System.Int32>>();
		services.TryAddTransient<IEntityKeyAccessor<Havit.EFCoreTests.Model.State, System.Int32>, DbEntityKeyAccessor<Havit.EFCoreTests.Model.State, System.Int32>>();
		services.TryAddTransient<IEntityKeyAccessor<Havit.EFCoreTests.Model.StateLocalization, System.Int32>, DbEntityKeyAccessor<Havit.EFCoreTests.Model.StateLocalization, System.Int32>>();
		services.TryAddTransient<IEntityKeyAccessor<Havit.EFCoreTests.Model.User, System.Guid>, DbEntityKeyAccessor<Havit.EFCoreTests.Model.User, System.Guid>>();
	}
}
