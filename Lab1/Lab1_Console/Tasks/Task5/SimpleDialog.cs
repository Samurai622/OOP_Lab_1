using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Lab1_Task5;

public static class SimpleDialog
{
    public static Task Show(Window owner, string message, string title)
    {
        var win = new Window
        {
            Title = title,
            Width = 360,
            Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Avalonia.Thickness(14),
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap, FontSize = 16 },
                    new Button
                    {
                        Content = "OK",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Width = 90,
                        [!Button.CommandProperty] = new Avalonia.Data.Binding()
                    }
                }
            }
        };

        ((Button)((StackPanel)win.Content!).Children[1]).Click += (_, __) => win.Close();

        return win.ShowDialog(owner);
    }
}
