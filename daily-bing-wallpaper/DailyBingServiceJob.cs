using FluentScheduler;
using Microsoft.Win32;
using RestSharp;
using Serilog;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace daily_bing_wallpaper
{
	public class DailyBingServiceJob : IJob
	{
		public void Execute()
		{
			try
			{
				var client = new RestClient("http://www.bing.com/");
				var request = new RestRequest("HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US", Method.GET);
				var response = client.Execute<dynamic>(request);
				string imageUrl = response.Data["images"][0]["url"];

				var imageRequest = new RestRequest(imageUrl, Method.GET);
				var imageBytes = client.DownloadData(imageRequest);

				const string filePath = "wallpaper.jpg";
				File.WriteAllBytes(filePath, imageBytes);
				var imagePath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
				Wallpaper.Set(imagePath, Wallpaper.Style.Stretched);
			}
			catch (Exception ex)
			{
				Log.Error(ex.StackTrace);
				Log.Information("~~~~~~~~~~~~~~~~~~~~~");
				Log.Error(ex.Message);
				Log.CloseAndFlush();
			}
		}
	}

	public sealed class Wallpaper
	{
		private Wallpaper() { }

		private const int SPI_SETDESKWALLPAPER = 20;
		private const int SPIF_UPDATEINIFILE = 0x01;
		private const int SPIF_SENDWININICHANGE = 0x02;

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

		public enum Style
		{
			Tiled,
			Centered,
			Stretched
		}

		public static void Set(string path, Style style)
		{
			RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
			if (style == Style.Stretched)
			{
				key.SetValue("WallpaperStyle", 2.ToString());
				key.SetValue("TileWallpaper", 0.ToString());
			}

			if (style == Style.Centered)
			{
				key.SetValue("WallpaperStyle", 1.ToString());
				key.SetValue("TileWallpaper", 0.ToString());
			}

			if (style == Style.Tiled)
			{
				key.SetValue("WallpaperStyle", 1.ToString());
				key.SetValue("TileWallpaper", 1.ToString());
			}

			SystemParametersInfo(SPI_SETDESKWALLPAPER,
				0,
				path,
				SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
		}
	}

}
