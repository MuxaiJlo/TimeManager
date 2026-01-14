using Microsoft.Win32;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace TimeManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Регистрируем приложение для уведомлений
            RegisterAppForNotifications();
        }

        private void RegisterAppForNotifications()
        {
            try
            {
                string appId = "TimeManager.App";
                string displayName = "Time Manager";
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");

                // Путь в реестре для уведомлений
                string regPath = @"SOFTWARE\Classes\AppUserModelId\" + appId;

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(regPath))
                {
                    key.SetValue("DisplayName", displayName);

                    if (File.Exists(iconPath))
                    {
                        key.SetValue("IconUri", iconPath);
                    }
                }

                System.Diagnostics.Debug.WriteLine("Приложение зарегистрировано для уведомлений");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка регистрации: {ex.Message}");
            }
        }
    }
}
