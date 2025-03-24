using MauiApp2.ViewModel;

namespace MauiApp2.Page;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		BindingContext = new LoginViewModel();
		
	}
	
}