using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Newtonsoft.Json;
using System.Linq;
using MauiApp2.Page;
using MauiApp2.Model;
using System.Diagnostics;

namespace MauiApp2.ViewModel
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string username = "";

        [ObservableProperty]
        private string password = "";

        [ObservableProperty]
        private bool isBusy = false;

        public bool IsNotBusy => !IsBusy; // เพิ่ม property นี้เพื่อแก้ปัญหา binding

        [RelayCommand]
        async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Username and password are required.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                var users = await ReadJsonAsync();

                if (users == null || !users.Any())
                {
                    await Shell.Current.DisplayAlert("Error", "No user data found.", "OK");
                    return;
                }

                var user = users.FirstOrDefault(u => u.Email == Username && u.Password == Password);

                if (user != null)
                {
                    AppData.CurrentUser = user;
                     await Shell.Current.GoToAsync($"{nameof(HomePage)}?uid={user.Uid}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Login Failed", "Invalid username or password.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<List<User>> ReadJsonAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("user.json");
                using var reader = new StreamReader(stream);
                var contents = await reader.ReadToEndAsync();
                var users = JsonConvert.DeserializeObject<List<User>>(contents);
                return users ?? new List<User>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading JSON: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to load user data. Please try again later.", "OK");
                return new List<User>();
            }
        }

        [RelayCommand]
        async Task ForgotPassword() // เพิ่ม command นี้เพื่อแก้ปัญหา binding
        {
            await Shell.Current.DisplayAlert("Forgot Password", "Please contact administrator.", "OK");
        }

        [RelayCommand]
        async Task Register()
        {
            try 
            {
                await Shell.Current.GoToAsync(nameof(RegisterPage)); // แก้เป็น absolute route
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Navigation Error", ex.Message, "OK");
            }
        }
    }
}