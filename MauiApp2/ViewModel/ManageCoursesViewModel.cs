using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp2.Model;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MauiApp2.ViewModel
{
    public partial class ManageCoursesViewModel : ObservableObject
    {
        private readonly HomeViewModel _homeViewModel;

        [ObservableProperty]
        private ObservableCollection<Course> availableCourses = new();

        [ObservableProperty]
        private ObservableCollection<Course> selectedCourses = new();

        [ObservableProperty]
        private ObservableCollection<Course> filteredAvailableCourses = new();

        [ObservableProperty]
        private string searchText = "";

        public ManageCoursesViewModel(HomeViewModel homeViewModel)
        {
            _homeViewModel = homeViewModel;
            Task.Run(async () => await LoadInitialData());
        }

        private async Task LoadInitialData()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("subject.json");
                using var reader = new StreamReader(stream);
                var contents = await reader.ReadToEndAsync();

                var allCourses = JsonConvert.DeserializeObject<List<Course>>(contents);
                if (allCourses == null) return;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (_homeViewModel?.CurrentCourses != null)
                    {
                        foreach (var course in _homeViewModel.CurrentCourses)
                        {
                            SelectedCourses.Add(course);
                        }

                        foreach (var course in allCourses)
                        {
                            if (!SelectedCourses.Any(c => c.CourseId == course.CourseId))
                            {
                                AvailableCourses.Add(course);
                            }
                        }
                    }

                    FilteredAvailableCourses = new ObservableCollection<Course>(AvailableCourses);
                    OnPropertyChanged(nameof(FilteredAvailableCourses));
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading courses: {ex.Message}");
            }
        }

        [RelayCommand]
        private void AddCourse(Course course)
        {
            if (!SelectedCourses.Contains(course))
            {
                SelectedCourses.Add(course);
                AvailableCourses.Remove(course);
                FilterCourses();
            }
        }

        [RelayCommand]
        private void RemoveCourse(Course course)
        {
            if (SelectedCourses.Contains(course))
            {
                SelectedCourses.Remove(course);
                AvailableCourses.Add(course);
                FilterCourses();
            }
        }
         [RelayCommand]
        private async Task SaveChanges()
        {
            try
            {
                // แสดง Alert เพื่อยืนยันการบันทึก
                bool confirm = await Shell.Current.DisplayAlert(
                    "Confirm Save",
                    "Are you sure you want to save changes?",
                    "Yes",
                    "No"
                );

                if (!confirm)
                {
                    return; // ถ้าผู้ใช้ไม่ยืนยัน ให้ยกเลิกการบันทึก
                }

                // ส่งข้อมูลกลับไปยัง HomePage
                if (_homeViewModel != null)
                {
                    _homeViewModel.UpdateCurrentCourses(SelectedCourses);
                }

                // กลับไปยังหน้า HomePage
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving changes: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                // เรียกใช้ DisplayAlert ใน UI Thread
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Error", "Unable to save changes. Please try again later.", "OK");
                });
            }
        }


        [RelayCommand]
        private void FilterCourses()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredAvailableCourses = new ObservableCollection<Course>(AvailableCourses);
            }
            else
            {
                var filtered = AvailableCourses
                    .Where(c => c.CourseName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                c.CourseId.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                FilteredAvailableCourses = new ObservableCollection<Course>(filtered);
            }

            OnPropertyChanged(nameof(FilteredAvailableCourses));
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterCourses();
        }
    }
}
