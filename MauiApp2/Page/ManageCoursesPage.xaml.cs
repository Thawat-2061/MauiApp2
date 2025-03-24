using MauiApp2.ViewModel;

namespace MauiApp2.Page;

public partial class ManageCoursesPage : ContentPage
{
	public ManageCoursesPage(ManageCoursesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // ตั้งค่า BindingContext จาก DI
    }
}