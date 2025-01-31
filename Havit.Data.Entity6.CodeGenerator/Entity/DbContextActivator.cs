﻿using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Reflection;

namespace Havit.Data.Entity.CodeGenerator.Entity;

public class DbContextActivator
{
	public DbContext Activate(Type dbContextType)
	{
		// pokud existuje, spustíme metodu InitializeForCodeGenerator s parametrem connection stringu
		MethodInfo configureForCodeGeneratorMethod = dbContextType.GetMethod("ConfigureForCodeGenerator", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
		if (configureForCodeGeneratorMethod != null)
		{
			configureForCodeGeneratorMethod.Invoke(null, new object[] { ConfigurationManager.ConnectionStrings["DefaultConnectionString"].ConnectionString });
		}

		// pokud je pro DbContext vytvořena a zaregistrována konfigurace přes DbConfiguration.SetConfiguration, funguje tento přístup
		var activator = DbConfiguration.DependencyResolver.GetService<Func<System.Data.Entity.DbContext>>(dbContextType);
		if (activator != null)
		{
			return (DbContext)activator.Invoke();
		}

		// pokud konfigurace není udělána a existuje bezparametrický konstruktor, funguje tento přístup
		var dbContextInfo = new DbContextInfo(dbContextType);
		return (DbContext)dbContextInfo.CreateInstance();
	}

}
