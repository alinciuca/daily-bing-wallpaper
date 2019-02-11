using Topshelf;

namespace daily_bing_wallpaper
{
	internal static class ServiceConfig
	{
		public static void Run()
		{
			HostFactory.Run(configure =>
			{
				configure.Service<DailyBingService>(s =>
				{
					s.ConstructUsing(_ => new DailyBingService());
					s.WhenStarted(p => p.Start());
					s.WhenStopped(p => p.Stop());
				});

				configure.SetServiceName("Daily bing wallpaper changer");
				configure.SetDisplayName("Daily bing wallpaper changer");
			});
		}
	}
}
