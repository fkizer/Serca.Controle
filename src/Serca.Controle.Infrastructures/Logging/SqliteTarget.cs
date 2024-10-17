using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog.Targets;
using NLog;
using Serca.Controle.Core.Application.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serca.Controle.Core.Application.Data;
using Serca.Controle.Core.Domain.Entities;

namespace Serca.Controle.Infrastructures.Logging
{
    [Target("SqliteTarget")]
    public class SqliteTarget : AsyncTaskTarget
    {
        private double RetentionDuration = TimeSpan.FromDays(7).TotalMinutes;

        protected IServiceProvider ServiceProvider;
        protected IDbContextExtendedFactory<ApplicationDbContext>? LogDbContextFactory;

        public SqliteTarget(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            ServiceProvider = serviceProvider;
            double.TryParse(configuration["LogRetention"], out RetentionDuration);
        }

        protected override Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {
            string logMessage = RenderLogEvent(this.Layout, logEvent);
            return CustumWriteAsync(logMessage);
        }

        private async Task CustumWriteAsync(string message)
        {
            try
            {
                if (LogDbContextFactory == null)
                {
                    LogDbContextFactory = (IDbContextExtendedFactory<ApplicationDbContext>?)ServiceProvider.GetService(typeof(IDbContextExtendedFactory<ApplicationDbContext>));
                }

                if (LogDbContextFactory != null)
                {
                    using var ctx = await LogDbContextFactory.CreateDbContextAsync();

                    //Clearing
                    var maxeAge = DateTime.Now.AddMinutes(-RetentionDuration);
                    await ctx.Database.ExecuteSqlRawAsync($"DELETE FROM Traces WHERE Date < '{maxeAge.ToString("yyyy-MM-dd HH:mm:ss")}'");

                    //Adding
                    ctx.Traces.Add(Trace.CreateTrace(message));

                    await ctx.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
