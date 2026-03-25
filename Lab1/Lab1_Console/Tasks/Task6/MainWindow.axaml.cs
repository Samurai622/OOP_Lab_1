using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Lab1_Task6
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CodeInput.Document = new TextDocument();
        }

        private async void BtnSelectFile_Click(object? sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Оберіть файл з кодом",
                AllowMultiple = false
            });

            if (files.Count >= 1)
            {
                try
                {
                    await using var stream = await files[0].OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    CodeInput.Document.Text = await reader.ReadToEndAsync();
                    ShowResult($"Файл {files[0].Name} успішно завантажено.", true, true);
                }
                catch (Exception ex)
                {
                    ShowResult($"Помилка читання файлу: {ex.Message}", false);
                }
            }
        }

        private void BtnExample_Click(object? sender, RoutedEventArgs e)
        {
            string lang = ((ComboBoxItem)LanguageCombo.SelectedItem!).Content!.ToString()!;
            string algo = ((ComboBoxItem)AlgorithmCombo.SelectedItem!).Content!.ToString()!;

            if (ReferenceData.Codes.ContainsKey(lang) && ReferenceData.Codes[lang].ContainsKey(algo))
            {
                CodeInput.Document.Text = ReferenceData.Codes[lang][algo];
                ShowResult("Приклад підставлено.", true, true);
            }
        }

        private async void BtnCheck_Click(object? sender, RoutedEventArgs e)
        {
            var checkButton = sender as Button;

            TxtResult.Foreground = Avalonia.Media.Brushes.White;
            TxtResult.Text = "Аналізую та виконую тести (це може зайняти пару секунд)...";
            if (checkButton != null) checkButton.IsEnabled = false;

            string studentCode = CodeInput.Document?.Text ?? "";
            string lang = ((ComboBoxItem)LanguageCombo.SelectedItem!).Content!.ToString()!;
            string algo = ((ComboBoxItem)AlgorithmCombo.SelectedItem!).Content!.ToString()!;

            if (string.IsNullOrWhiteSpace(studentCode))
            {
                ShowResult("Помилка: Код порожній.", false);
                if (checkButton != null) checkButton.IsEnabled = true;
                return;
            }

            // Передаємо логіку у сервіс. Метод UpdateUIResult передаємо як callback, 
            // щоб сервіс міг повідомляти інтерфейс про результати.
            await Task.Run(() =>
            {
                TestingService.RunCheck(studentCode, lang, algo, UpdateUIResult);
            });

            Dispatcher.UIThread.Post(() =>
            {
                if (checkButton != null) checkButton.IsEnabled = true;
            });
        }

        private void UpdateUIResult(string message, bool isSuccess, bool isNeutral = false)
        {
            Dispatcher.UIThread.Post(() => ShowResult(message, isSuccess, isNeutral));
        }

        private void ShowResult(string message, bool isSuccess, bool isNeutral = false)
        {
            TxtResult.Text = message;
            if (isNeutral) TxtResult.Foreground = Avalonia.Media.Brushes.LightBlue;
            else if (!isSuccess) TxtResult.Foreground = Avalonia.Media.Brushes.OrangeRed;
            else TxtResult.Foreground = Avalonia.Media.Brushes.LightGreen;
        }
    }
}