
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Targets;
using Serca.Controle.Core.Application.Abstraction;
using Serca.Controle.Core.Application.Data;
using Serca.Controle.Core.Domain.Entities;

namespace Serca.Controle.Infrastructures.Logging
{
    [Target("MemoryFlushableTarget")]
    public class MemoryFlushableTarget : AsyncTaskTarget
    {
        private double RetentionDuration = TimeSpan.FromDays(7).TotalMinutes;

        protected IServiceProvider ServiceProvider;
        protected IDbContextExtendedFactory<ApplicationDbContext>? LogDbContextFactory;
        private List<Trace> Traces = new List<Trace>();

        public MemoryFlushableTarget(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            ServiceProvider = serviceProvider;
            double.TryParse(configuration["LogRetention"], out RetentionDuration);
        }

        protected override Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken)
        {
            string logMessage = RenderLogEvent(Layout, logEvent);
            return Task.Run(() => { CustumWriteAsync(logMessage); });
        }

        private void CustumWriteAsync(string message)
        {
            Traces.Add(Trace.CreateTrace(message));
        }

        public async Task FlushDatabase()
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
                    await ctx.Traces.AddRangeAsync(Traces);
                    await ctx.SaveChangesAsync();
                    Traces.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
