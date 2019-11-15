using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using RokPak;

namespace RokPak
{
    public partial class App : Application
    {
        public static bool IsUserLoggedIn { get; set; }

        public App()
        {
            if (!IsUserLoggedIn)
            {
                MainPage = new NavigationPage(new RokPak.LoginPage());
            }
            else
            {
                MainPage = new NavigationPage(new RokPak.MainPage());
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
