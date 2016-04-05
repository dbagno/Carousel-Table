using System;

using Xamarin.Forms;

namespace CarouseTables
{
    public class App : Application
    {
        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();

        public static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return getrandom.Next(min, max);
            }
        }

        public App()
        {
            // The root page of your application
            MainPage = new NavigationPage(new TablePage());
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

