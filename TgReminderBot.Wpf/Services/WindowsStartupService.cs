using System.Reflection;

namespace TgReminderBot.Wpf.Services
{
    public static class WindowsStartupService
    {
        public static void SetAutolaunch(bool enabled)
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                    true
                    );
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                var name = curAssembly.GetName().Name
                    .Replace(".dll", ".exe");
                var location = curAssembly.Location
                    .Replace(".dll", ".exe");

                if (enabled)
                {
                    key.SetValue(name, location);
                }
                else
                {
                    key.DeleteValue(name );
                }
            }
            catch
            {
            }

        }
    }
}
