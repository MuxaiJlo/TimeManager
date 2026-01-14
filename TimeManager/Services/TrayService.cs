using System;
using System.IO;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace TimeManager.Services
{
    public class TrayService
    {
        private TaskbarIcon _trayIcon;
        private Window _mainWindow;

        public TrayService(Window mainWindow)
        {
            _mainWindow = mainWindow;
            _trayIcon = new TaskbarIcon
            {
                // Используем иконку из ресурсов или полный путь
                Icon = LoadIcon(),
                ToolTipText = "Time Manager"
            };

            _trayIcon.TrayMouseDoubleClick += (s, e) =>
            {
                _mainWindow.Show();
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.Activate();
            };

            var contextMenu = new System.Windows.Controls.ContextMenu();

            // Добавим пункт "Показать"
            var showMenuItem = new System.Windows.Controls.MenuItem { Header = "Показать" };
            showMenuItem.Click += (s, e) =>
            {
                _mainWindow.Show();
                _mainWindow.WindowState = WindowState.Normal;
                _mainWindow.Activate();
            };
            contextMenu.Items.Add(showMenuItem);

            var exitMenuItem = new System.Windows.Controls.MenuItem { Header = "Выход" };
            exitMenuItem.Click += (s, e) =>
            {
                _trayIcon.Dispose();
                Application.Current.Shutdown();
            };
            contextMenu.Items.Add(exitMenuItem);

            _trayIcon.ContextMenu = contextMenu;
        }

        private System.Drawing.Icon LoadIcon()
        {
            try
            {
                // Попробуем загрузить из ресурсов или из папки приложения
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.ico");
                if (File.Exists(iconPath))
                {
                    return new System.Drawing.Icon(iconPath);
                }
            }
            catch { }

            // Если не нашли, используем системную иконку
            return System.Drawing.SystemIcons.Application;
        }

        public void Dispose()
        {
            _trayIcon?.Dispose();
        }
    }
}