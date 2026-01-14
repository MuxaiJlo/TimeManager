# Time Manager 24h 🕒

A sleek, lightweight desktop widget designed for visual time tracking. Unlike traditional planners, **Time Manager** uses a circular 24-hour analog interface to visualize your day, helping you see "time blocks" at a glance.

## ✨ Features

* **Circular Time Visualization**: Tasks are rendered as colored sectors on a 24-hour clock face, synchronized with your local time.
* **Real-time Active Clock**: Features animated hour, minute, and second hands.
* **System Tray Integration**: Close the window to hide it in the system tray. The app stays active in the background without cluttering your taskbar.
* **Smart Windows Notifications**:
* **Start**: Alerts you when a new process begins.
* **Mid-point**: A gentle reminder halfway through to keep you focused.
* **End**: Notifies you when it's time to wrap up.


* **Dynamic Task Management**: Add, sort, and delete processes using a clean, card-based UI.
* **Modern UI/UX**: Dark mode aesthetic with silver borders, gradients, and custom color presets.

## 🛠 Tech Stack

* **Language**: C# 12
* **Framework**: WPF (Windows Presentation Foundation)
* **Pattern**: MVVM (Model-View-ViewModel)
* **Libraries**:
* `System.Windows.Forms` (System Tray and NotifyIcon integration)



## 🚀 Installation & Setup

1. **Clone the repository**:
```bash
git clone https://github.com/MuxaiJlo/TimeManager.git

```


2. **Prerequisites**:
* .NET 6.0 SDK or higher.
* Windows 10/11 (for native notifications).


3. **Build and Run**:
Open the solution in Visual Studio and press `F5` to build and launch the application.

## 📖 How to Use

1. **Add a Task**: Click the **"+"** button. Enter the process name, start/end times (HH:mm), and pick a color from the preset palette.
2. **Monitor**: View your schedule on the analog clock. The red needle shows your current progress through the day.
3. **Tray Mode**: Clicking the "X" on the window hides the app. Right-click the tray icon to **Open** or **Exit** the application completely.
