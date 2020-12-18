using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();


			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", async context =>
				{
					try
					{
						for (int i = 0; i < 50; i++)
						{
							System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
							using var sqlConnection = Havit.Data.SqlClient.Azure.SqlServerAadAuthentication.SqlConnectionFactory.CreateSqlConnectionWithAadAuthenticationSupport("server=tcp:kandadbserver.database.windows.net;database=VolejbalDb");
							sqlConnection.Open();
							sw.Stop();
							await context.Response.WriteAsync(sw.ElapsedMilliseconds.ToString());
							await context.Response.WriteAsync(" ");
						}
						await context.Response.WriteAsync("OK");
					}
					catch (Exception ex)
					{
						await context.Response.WriteAsync(ex.ToString());
					}

				});
			});
		}
	}
}
