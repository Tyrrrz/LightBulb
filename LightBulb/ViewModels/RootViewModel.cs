using System;
using System.Diagnostics;
using System.Reflection;
using Stylet;

namespace LightBulb.ViewModels
{
    public class RootViewModel : Screen
    {
        public string ApplicationVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);

        public bool IsEnabled { get; set; }

        public RootViewModel()
        {
        }

        public void ToggleIsEnabled() => IsEnabled = !IsEnabled;

        public void DisableTemporarily(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public void ShowAbout() => Process.Start("https://github.com/Tyrrrz/LightBulb");

        public void Exit() => RequestClose();
    }
}