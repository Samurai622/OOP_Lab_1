using System;
using Avalonia;

namespace Lab1_Task6;

public static class Task6
{
    [STAThread]
    public static void Run() => Run(Array.Empty<string>());

    [STAThread]
    public static void Run(string[] args)
        => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}