using Havit.Hangfire.Extensions.Filters;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Havit.HangfireApp.Jobs
{
    public class JobOne : IJobOne
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<JobOne> logger;

        public JobOne(HttpClient httpClient, ILogger<JobOne> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 5; i++)
            {
                logger.LogInformation(this.GetType().Name + ": " + i);
                //Console.WriteLine(this.GetType().Name + ": " + i);
                await httpClient.GetAsync("/", cancellationToken);
                using (var connection = new SqlConnection("Data Source=sqldev.havit.local;Initial Catalog=HavitBusinessLayerTest;User Id=development;Password=development;"))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "SELECT * from sys.tables";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                        }
                    }
                }
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
