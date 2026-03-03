using System;
using Avalonia;

namespace Lab1_Task5;

public static class Task5
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