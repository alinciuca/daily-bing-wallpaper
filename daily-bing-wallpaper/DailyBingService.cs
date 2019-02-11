using FluentScheduler;
using Serilog;

namespace daily_bing_wallpaper
{
	public class DailyBingService
	{
		public DailyBingService()
		{
			Log.Logger = new LoggerConfiguration()
					.MinimumLevel.Debug()
					.WriteTo.Console()
					.CreateLogger();
		}

		public void Start()
		{
			var registry = new Registry();
			registry.Schedule<DailyBingServiceJob>().ToRunNow().AndEvery(1).Days();
			JobManager.Initialize(registry);
			JobManager.Start();
		}

		public void Stop()
		{
			Log.Information("Service stopped");
			JobManager.StopAndBlock();
			JobManager.RemoveAllJobs();
			Log.CloseAndFlush();
		}
	}
}
