Rozšiřuje [FaceIT.Hangfire.Tags](https://www.nuget.org/packages/FaceIT.Hangfire.Tags) o

* automatické tagování spouštěných jobů v Hangfire,
* rozšíření dashboardu Hangfire - recurring joby jsou odkazem na stejně pojmenovaný tag.

## Zapojení do projektu

* Předpokládá se zapojení [FaceIT.Hangfire.Tags.SqlServer](https://www.nuget.org/packages/FaceIT.Hangfire.Tags.SqlServer) dle [návodu](https://github.com/face-it/Hangfire.Tags?tab=readme-ov-file#setup) jak v projektu s hangfire serverem, tak v projektu s Hangfire dashboardem.
* V projektu s Hangfire serverem je potřeba zaregistrovat automatické tagování jobů.
* V projektu s Hangfire dashboardem je potřeba zaregistrovat rozšíření dashboardu.

#### Registrace v Hangfire serveru

```csharp
service.AddHangfire(configuration => configuration
	...
	.UseSqlServerStorage(() => new Microsoft.Data.SqlClient.SqlConnection(...), new SqlServerStorageOptions
	{
		...
	})
	.UseTagsWithSql(new TagsOptions { Clean = Clean.None })
	.UseJobsTagging()
	...
```

#### Registrace v Hangfire dashboardu

```csharp
service.AddHangfire(configuration => configuration
	...
	.UseSqlServerStorage(() => new Microsoft.Data.SqlClient.SqlConnection(...), new SqlServerStorageOptions
	{
		...
	})
	.UseTagsWithSql(new TagsOptions { Clean = Clean.None })
	.UseTagsDashboardExtension()
	...
```
