using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace Lab1_Task5;

public partial class MainWindow : Window
{
    private readonly ActionHub _hub;
    private readonly ObservableCollection<string> _logs = new();
    private readonly Random _random = new();

    // Палітра фону
    private readonly Color[] _bgColors = { Colors.LightGray, Colors.IndianRed, Colors.Gold, Colors.LightGreen };
    private int _bgIndex = 0;

    // Лічильники
    private int _opacityCount, _bgCount, _helloCount, _superCount;

    // Анімація фону
    private DispatcherTimer? _bgTimer;
    private Color _bgFrom;
    private Color _bgTo;
    private int _bgStep;
    private int _bgStepsTotal;

    public MainWindow()
    {
        InitializeComponent();

        LogList.ItemsSource = _logs;

        _hub = new ActionHub(
            changeOpacity: ChangeOpacity,
            changeBackground: ChangeBackground,
            hello: HelloWorldAsync
        );

        // Звичайні кнопки
        BtnOpacity.Click += Opacity_Click;
        BtnBg.Click += Bg_Click;
        BtnHello.Click += Hello_Click;

        // Суперкнопка: завжди показує своє повідомлення
        BtnSuper.Click += SuperButton_Click;

        // Чекбокси керують тим, чи підписана суперкнопка на ті ж самі обробники (той самий код!)
        CbDoOpacity.IsCheckedChanged += (_, __) =>
        {
            if (CbDoOpacity.IsChecked == true) BtnSuper.Click += Opacity_Click;
            else BtnSuper.Click -= Opacity_Click;
        };

        CbDoBg.IsCheckedChanged += (_, __) =>
        {
            if (CbDoBg.IsChecked == true) BtnSuper.Click += Bg_Click;
            else BtnSuper.Click -= Bg_Click;
        };

        CbDoHello.IsCheckedChanged += (_, __) =>
        {
            if (CbDoHello.IsChecked == true) BtnSuper.Click += Hello_Click;
            else BtnSuper.Click -= Hello_Click;
        };

        AddLog("Готово. Натискай кнопки 🙂");
        UpdateStats();
    }

    private void Opacity_Click(object? sender, RoutedEventArgs e)
    {
        _hub.ChangeOpacity();
        _opacityCount++;
        AddLog($"Дія: Прозорість (#{_opacityCount})");
        UpdateStats();
    }

    private void Bg_Click(object? sender, RoutedEventArgs e)
    {
        _hub.ChangeBackground();
        _bgCount++;
        AddLog($"Дія: Колір тла (#{_bgCount})");
        UpdateStats();
    }

    private async void Hello_Click(object? sender, RoutedEventArgs e)
    {
        await _hub.Hello();
        _helloCount++;
        AddLog($"Дія: hello world (#{_helloCount})");
        UpdateStats();
    }

    // Вимога: тіло методу суперкнопки максимально просте
    private async void SuperButton_Click(object? sender, RoutedEventArgs e)
    {
        _superCount++;
        UpdateStats();
        await SimpleDialog.Show(this, "Я супермегакнопка,\nі цього мене не позбавиш!", "Super");
    }

    // ===== ЛОГІКА ДІЙ =====

    private void ChangeOpacity()
    {
        // 0.4 .. 1.0
        double newOpacity = 0.4 + _random.NextDouble() * 0.6;
        Opacity = newOpacity;

        AddLog($"Нова прозорість: {newOpacity:F2}");
    }

    private void ChangeBackground()
    {
        // Можеш зробити рандом, але зараз по колу — щоб було наочно
        _bgIndex = (_bgIndex + 1) % _bgColors.Length;
        AnimateBackgroundTo(_bgColors[_bgIndex], durationMs: 420);
    }

    private Task HelloWorldAsync()
        => SimpleDialog.Show(this, "Hello, world!", "Hello");

    // ===== Анімація фону =====

    private void AnimateBackgroundTo(Color target, int durationMs = 400)
    {
        _bgTimer?.Stop();

        _bgFrom = (Background as SolidColorBrush)?.Color ?? Colors.LightGray;
        _bgTo = target;

        _bgStepsTotal = Math.Max(8, durationMs / 16);
        _bgStep = 0;


        _bgTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        _bgTimer.Tick += (_, __) =>
        {
            _bgStep++;
            double t = (double)_bgStep / _bgStepsTotal;
            if (t >= 1.0) t = 1.0;

            Background = new SolidColorBrush(LerpColor(_bgFrom, _bgTo, t));

            if (_bgStep >= _bgStepsTotal)
                _bgTimer?.Stop();
        };

        _bgTimer.Start();
    }

    private static Color LerpColor(Color a, Color b, double t)
    {
        static byte lerp(byte x, byte y, double tt) => (byte)(x + (y - x) * tt);

        return Color.FromArgb(
            lerp(a.A, b.A, t),
            lerp(a.R, b.R, t),
            lerp(a.G, b.G, t),
            lerp(a.B, b.B, t)
        );
    }

    // ===== UI допоміжне =====

    private void UpdateStats()
    {
        TxtStats.Text =
            "Статистика:\n" +
            $"Прозорість: {_opacityCount}\n" +
            $"Колір тла: {_bgCount}\n" +
            $"Hello: {_helloCount}\n" +
            $"Суперкнопка: {_superCount}";
    }

    private void AddLog(string text)
        => _logs.Insert(0, $"{DateTime.Now:HH:mm:ss} — {text}");
}
