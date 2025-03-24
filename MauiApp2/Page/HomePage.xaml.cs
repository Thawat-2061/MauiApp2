using MauiApp2.ViewModel;

namespace MauiApp2.Page;

public partial class HomePage : ContentPage
{
	private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
         InitializeComponent();
    // ตรวจสอบการตั้งค่า BindingContext
    BindingContext = new HomeViewModel();
    }

    protected override void OnAppearing()
{
    base.OnAppearing();

    if (BindingContext is HomeViewModel viewModel)
    {
        viewModel.UpdateCourses();
    }
}
}