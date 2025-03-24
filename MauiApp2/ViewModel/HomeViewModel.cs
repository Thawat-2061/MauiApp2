using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp2.Model;
using MauiApp2.Page;

namespace MauiApp2.ViewModel
{
    public partial class HomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private User currentUser;

        [ObservableProperty]
        private List<string> terms;

        [ObservableProperty]
        private string selectedTerm;

        [ObservableProperty]
        private ObservableCollection<Course> currentCourses;

        public HomeViewModel()
        {
            LoadCurrentUser();
            InitializeTerms();

            // สมัครรับข้อความจาก ManageCoursesViewModel
            MessagingCenter.Subscribe<ManageCoursesViewModel, IEnumerable<Course>>(this, "UpdateCourses", (sender, courses) =>
            {
                UpdateCurrentCourses(courses);
            });
        }

        [RelayCommand]
        public async Task NavigateToProfilePage()
        {
            try
            {
                // ใช้ DI เพื่อสร้าง ProfilePage
                var profilePage = MauiProgram.CreateMauiApp().Services.GetRequiredService<ProfilePage>();
                await Shell.Current.Navigation.PushAsync(profilePage);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating to ProfilePage: {ex.Message}");
            }
        }
        public ICommand LogoutCommand => new Command(Logout);

        private async void Logout()
        {
            try
            {
                // Show the confirmation alert
                bool isConfirmed = await App.Current.MainPage.DisplayAlert(
                    "Confirm Logout",
                    "Are you sure you want to logout?",
                    "Yes",
                    "No");

                if (isConfirmed)
                {
                    // Logic to log out the user
                    // Example: clear session or token here
                    // You can call your logout logic here, like clearing user data or tokens.

                    // If you're using Shell, use this method for navigation:
                    await Shell.Current.GoToAsync(nameof(LoginPage));
                }
            }
            catch (Exception ex)
            {
                // Log the error or display it to the user
                await App.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }



        private void LoadCurrentUser()
        {
            CurrentUser = AppData.CurrentUser;

            if (CurrentUser == null)
            {
                Debug.WriteLine("CurrentUser is null");
            }
            else
            {
                Debug.WriteLine($"CurrentUser: {CurrentUser.Uid}");
                Debug.WriteLine($"Profile: {CurrentUser.Profile?.Name}");
            }
        }

        private void InitializeTerms()
        {
            Terms = new List<string> { "Term 1", "Term 2", "Term 3 (Current)" };
            SelectedTerm = Terms[2];
            UpdateCourses();
        }

        [RelayCommand]
        private void ChangeTerm(string term)
        {
            SelectedTerm = term;
            UpdateCourses();
        }

        public void UpdateCourses()
        {
            if (CurrentUser == null) return;

            switch (SelectedTerm)
            {
                case "Term 1":
                    CurrentCourses = new ObservableCollection<Course>(
                        CurrentUser.PreviousTermsRegistration
                            .FirstOrDefault(t => t.Term == "1/2023")?.Courses);
                    break;
                case "Term 2":
                    CurrentCourses = new ObservableCollection<Course>(
                        CurrentUser.PreviousTermsRegistration
                            .FirstOrDefault(t => t.Term == "2/2023")?.Courses);
                    break;
                case "Term 3 (Current)":
                    CurrentCourses = new ObservableCollection<Course>(
                        CurrentUser.CurrentTermRegistration);
                    break;
                default:
                    CurrentCourses = new ObservableCollection<Course>();
                    break;
            }
        }

        [RelayCommand]
        private async Task NavigateToManageCourses()
        {
            // ใช้ DI เพื่อสร้าง ManageCoursesPage
            var manageCoursesPage = MauiProgram.CreateMauiApp().Services.GetRequiredService<ManageCoursesPage>();
            await Shell.Current.Navigation.PushAsync(manageCoursesPage);
        }



        [RelayCommand]
        public void UpdateCurrentCourses(IEnumerable<Course> courses)
        {
            if (courses == null) return;

            // อัปเดต CurrentCourses
            CurrentCourses.Clear();
            foreach (var course in courses)
            {
                CurrentCourses.Add(course);
            }

            // อัปเดตข้อมูลใน CurrentUser.CurrentTermRegistration
            if (CurrentUser.CurrentTermRegistration == null)
            {
                CurrentUser.CurrentTermRegistration = new List<Course>();
            }
            CurrentUser.CurrentTermRegistration.Clear();
            CurrentUser.CurrentTermRegistration.AddRange(courses);

            // รีเฟรช UI โดยการเรียก UpdateCourses
            UpdateCourses();
        }


    }


}