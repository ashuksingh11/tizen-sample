using System;
using Tizen.NUI;
using MultiTimer.Views;

namespace MultiTimer
{
    class Program : NUIApplication
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            var window = Window.Default;
            window.Title = "Multi Timer";
            window.BackgroundColor = Color.White;

            var mainPage = new MainPage();
            window.Add(mainPage);
        }

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
