using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MauiApp2.ViewModel;
using MauiApp2.Page;

namespace MauiApp2;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit() 
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
			
		builder.Services.AddSingleton<HomeViewModel>();
		builder.Services.AddTransient<ManageCoursesViewModel>();
		builder.Services.AddTransient<ManageCoursesPage>();
		builder.Services.AddTransient<ProfilePage>(); // เพิ่ม ProfilePage ใน DI container

#if DEBUG
		builder.Logging.AddDebug();
#endif
		
		return builder.Build();
	}
}
