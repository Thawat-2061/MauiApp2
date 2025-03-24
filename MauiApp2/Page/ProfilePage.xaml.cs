using MauiApp2.ViewModel;

namespace MauiApp2.Page;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(HomeViewModel viewModel)
	{
		 InitializeComponent();
    
    // ตรวจสอบว่า BindingContext ถูกตั้งให้กับ HomeViewModel หรือ ProfileViewModel
    BindingContext = new HomeViewModel(); 
	}
}