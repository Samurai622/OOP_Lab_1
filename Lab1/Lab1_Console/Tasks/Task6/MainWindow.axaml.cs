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
            // Безпечне отримання значень з ComboBox (захист від NullReferenceException)
            if (LanguageCombo.SelectedItem is not ComboBoxItem langItem || 
                AlgorithmCombo.SelectedItem is not ComboBoxItem algoItem)
            {
                ShowResult("Будь ласка, оберіть мову та алгоритм.", false);
                return;
            }

            string lang = langItem.Content!.ToString()!;
            string algo = algoItem.Content!.ToString()!;

            // Безпечна перевірка наявності ключа
            if (ReferenceData.Codes.TryGetValue(lang, out var langCodes) && 
                langCodes.TryGetValue(algo, out var code))
            {
                CodeInput.Document.Text = code;
                ShowResult("Приклад підставлено. Зверніть увагу: це повноцінна програма.", true, true);
            }
            else
            {
                ShowResult("Для обраної мови та алгоритму немає прикладу.", false);
            }
        }

        private async void BtnCheck_Click(object? sender, RoutedEventArgs e)
        {
            var checkButton = sender as Button;

            // Безпечне отримання значень з ComboBox
            if (LanguageCombo.SelectedItem is not ComboBoxItem langItem || 
                AlgorithmCombo.SelectedItem is not ComboBoxItem algoItem)
            {
                ShowResult("Помилка: Оберіть мову та алгоритм.", false);
                return;
            }

            string lang = langItem.Content!.ToString()!;
            string algo = algoItem.Content!.ToString()!;
            string studentCode = CodeInput.Document?.Text ?? "";

            if (string.IsNullOrWhiteSpace(studentCode))
            {
                ShowResult("Помилка: Код порожній. Вставте повний код програми (з main).", false);
                return;
            }

            // Оновлюємо UI перед стартом перевірки
            TxtResult.Foreground = Avalonia.Media.Brushes.White;
            TxtResult.Text = $"Компіляція та виконання тестів ({lang})...\nЗачекайте, це може зайняти до 15 секунд.";
            if (checkButton != null) checkButton.IsEnabled = false;

            // Запускаємо важку роботу у фоновому потоці
            await Task.Run(() =>
            {
                TestingService.RunCheck(studentCode, lang, algo, UpdateUIResult);
            });

            // Повертаємо доступ до кнопки після завершення
            Dispatcher.UIThread.Post(() =>
            {
                if (checkButton != null) checkButton.IsEnabled = true;
            });
        }

        private void UpdateUIResult(string message, bool isSuccess, bool isNeutral = false)
        {
            // Метод-callback, який TestingService безпечно викликає для оновлення UI
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