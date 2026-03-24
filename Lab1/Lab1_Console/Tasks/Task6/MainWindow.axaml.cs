using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task6
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, Dictionary<string, string>> _referenceCodes;

        public MainWindow()
        {
            InitializeComponent();
            _referenceCodes = InitializeReferenceCodes();
        }

        // --- ОБРОБНИКИ ПОДІЙ ІНТЕРФЕЙСУ ---

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
                    CodeInput.Text = await reader.ReadToEndAsync();
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

            if (_referenceCodes.ContainsKey(lang) && _referenceCodes[lang].ContainsKey(algo))
            {
                CodeInput.Text = _referenceCodes[lang][algo];
                ShowResult("Приклад підставлено.", true, true);
            }
        }

        private async void BtnCheck_Click(object? sender, RoutedEventArgs e)
        {
            // Отримуємо кнопку, на яку натиснули, щоб заблокувати її від подвійних кліків
            var checkButton = sender as Button;

            TxtResult.Foreground = Avalonia.Media.Brushes.White;
            TxtResult.Text = "Аналізую та виконую тести (це може зайняти пару секунд)...";
            
            if (checkButton != null) checkButton.IsEnabled = false; 

            string studentCode = CodeInput.Text ?? "";
            string lang = ((ComboBoxItem)LanguageCombo.SelectedItem!).Content!.ToString()!;
            string algo = ((ComboBoxItem)AlgorithmCombo.SelectedItem!).Content!.ToString()!;

            if (string.IsNullOrWhiteSpace(studentCode))
            {
                ShowResult("Помилка: Код порожній.", false);
                if (checkButton != null) checkButton.IsEnabled = true;
                return;
            }

            // Запускаємо важку перевірку у фоновому потоці, щоб UI не зависав
            await Task.Run(() => 
            {
                if (lang == "C#")
                {
                    CheckCSharpCodeExecution(studentCode, algo);
                }
                else
                {
                    CheckOtherLanguageCode(studentCode, lang, algo);
                }
            });

            // Розблоковуємо кнопку після завершення тестів (через UI потік)
            Dispatcher.UIThread.Post(() => 
            {
                if (checkButton != null) checkButton.IsEnabled = true;
            });
        }

        // --- ЛОГІКА ПЕРЕВІРКИ C# (Через запуск, перевірку масивів та Stopwatch) ---

        private void CheckCSharpCodeExecution(string code, string algorithm)
        {
            string expectedMethodName = algorithm == "Selection Sort" ? "SelectionSort" : "ShakerSort";

            try
            {
                // Евристична перевірка: щоб студент не підсунув Selection замість Shaker
                if (!CheckHeuristics(code, algorithm))
                {
                    UpdateUIResult($"❌ Помилка: Структура коду не відповідає алгоритму '{algorithm}'. \n(Наприклад: Shaker Sort має містити цикл 'while', а Selection - ні).", false);
                    return;
                }

                MethodInfo studentMethod = CompileAndGetMethod(code);
                MethodInfo referenceMethod = typeof(ReferenceMethods).GetMethod(expectedMethodName)!;

                List<int[]> testCases = GenerateTestCases();

                // Формула перевірки співмірності часу
                Func<long, long, bool> isTimeCommensurate = (tEta, tStud) =>
                {
                    long lowerBound = Math.Max(0, (tEta / 5) - 200);
                    long upperBound = (5 * tEta) + 200;
                    return tStud >= lowerBound && tStud <= upperBound;
                };

                bool allTestsPassed = true;
                string errorMessage = "";
                int testNumber = 1;

                foreach (var originalArray in testCases)
                {
                    int[] refArray = (int[])originalArray.Clone();
                    int[] studArray = (int[])originalArray.Clone();

                    long tEta = MeasureExecutionTime(referenceMethod, refArray);
                    long tStud = MeasureExecutionTime(studentMethod, studArray);

                    if (!refArray.SequenceEqual(studArray))
                    {
                        allTestsPassed = false;
                        errorMessage = $"Помилка на тесті {testNumber} (розмір {originalArray.Length}): Результат сортування не збігається з еталонним.";
                        break;
                    }

                    if (!isTimeCommensurate(tEta, tStud))
                    {
                        allTestsPassed = false;
                        errorMessage = $"Помилка на тесті {testNumber} (розмір {originalArray.Length}): Тривалість виконання неспівмірна.\n" +
                                       $"Еталон: {tEta} мс, Ваш код: {tStud} мс.\n" +
                                       $"(Можливо, ви використали алгоритм з іншою складністю).";
                        break;
                    }

                    testNumber++;
                }

                if (allTestsPassed)
                {
                    UpdateUIResult($"✅ Успіх! Усі {testCases.Count} тестів пройдено.\nРезультати збігаються, час виконання визнано співмірним.", true);
                }
                else
                {
                    UpdateUIResult($"❌ {errorMessage}", false);
                }
            }
            catch (Exception ex)
            {
                UpdateUIResult($"Помилка компіляції/виконання: {ex.Message}", false);
            }
        }

        // Базова евристична перевірка тексту
        private bool CheckHeuristics(string code, string algo)
        {
            // Видаляємо коментарі
            string cleanCode = Regex.Replace(code, @"//.*|/\*[\s\S]*?\*/", "");
            
            if (algo == "Shaker Sort")
            {
                if (!cleanCode.Contains("while")) return false;
            }
            if (algo == "Selection Sort")
            {
                if (cleanCode.Contains("while")) return false;
            }
            return true;
        }

        private List<int[]> GenerateTestCases()
        {
            Random rnd = new Random(42);
            return new List<int[]>
            {
                new int[0],                                     
                new int[] { 5, 2, 9, 1, 5, 6 },                 
                Enumerable.Range(1, 5000).Select(_ => rnd.Next(0, 10000)).ToArray(), 
                // СПЕЦІАЛЬНИЙ ТЕСТ: Величезний вже відсортований масив. 
                Enumerable.Range(1, 40000).ToArray(),           
                Enumerable.Range(1, 4000).Reverse().ToArray()   
            };
        }

        private long MeasureExecutionTime(MethodInfo method, int[] array)
        {
            var sw = Stopwatch.StartNew();
            method.Invoke(null, new object[] { array });
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        private MethodInfo CompileAndGetMethod(string rawMethodCode)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(rawMethodCode);
            var assemblyName = Path.GetRandomFileName();

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location)
            };

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                string errorMsg = string.Join("\n", failures.Select(f => f.GetMessage()));
                throw new Exception(errorMsg);
            }

            ms.Seek(0, SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());
            
            foreach (Type type in assembly.GetTypes())
            {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                foreach (var m in methods)
                {
                    var parameters = m.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(int[]) && m.Name.Contains("Sort", StringComparison.OrdinalIgnoreCase))
                    {
                        return m;
                    }
                }
            }
            throw new Exception("Не знайдено методу сортування. Метод повинен приймати 'int[]' і мати слово 'Sort' у назві.");
        }

        // --- ЛОГІКА ПЕРЕВІРКИ ІНШИХ МОВ ---
        private void CheckOtherLanguageCode(string studentCode, string lang, string algo)
        {
            string referenceCode = _referenceCodes[lang][algo];
            string normalizedStudent = Regex.Replace(studentCode, @"\s+", "");
            string normalizedReference = Regex.Replace(referenceCode, @"\s+", "");

            if (normalizedStudent.Contains(normalizedReference))
            {
                 UpdateUIResult($"✅ Успіх! Логіка {lang} коду збігається з еталонною.\n(Динамічні заміри Stopwatch працюють лише для C#)", true);
            }
            else
            {
                 UpdateUIResult($"❌ Невірно. Структура коду відрізняється від еталонної для {lang}.", false);
            }
        }

        // Оновлення UI з іншого потоку
        private void UpdateUIResult(string message, bool isSuccess, bool isNeutral = false)
        {
            Dispatcher.UIThread.Post(() => ShowResult(message, isSuccess, isNeutral));
        }

        private void ShowResult(string message, bool isSuccess, bool isNeutral = false)
        {
            TxtResult.Text = message;
            if (isNeutral)
                TxtResult.Foreground = Avalonia.Media.Brushes.LightBlue;
            else if (!isSuccess)
                TxtResult.Foreground = Avalonia.Media.Brushes.OrangeRed;
            else
                TxtResult.Foreground = Avalonia.Media.Brushes.LightGreen;
        }

        // --- ЕТАЛОННІ МЕТОДИ C# ---
        public static class ReferenceMethods
        {
            public static void SelectionSort(int[] array)
            {
                int n = array.Length;
                for (int i = 0; i < n - 1; i++)
                {
                    int min_idx = i;
                    for (int j = i + 1; j < n; j++)
                    {
                        if (array[j] < array[min_idx])
                        {
                            min_idx = j;
                        }
                    }
                    int temp = array[min_idx];
                    array[min_idx] = array[i];
                    array[i] = temp;
                }
            }

            public static void ShakerSort(int[] array)
            {
                bool swapped = true;
                int start = 0;
                int end = array.Length;
                while (swapped == true)
                {
                    swapped = false;
                    for (int i = start; i < end - 1; ++i)
                    {
                        if (array[i] > array[i + 1])
                        {
                            int temp = array[i];
                            array[i] = array[i + 1];
                            array[i + 1] = temp;
                            swapped = true;
                        }
                    }
                    if (swapped == false) break;
                    swapped = false;
                    end = end - 1;
                    for (int i = end - 1; i >= start; i--)
                    {
                        if (array[i] > array[i + 1])
                        {
                            int temp = array[i];
                            array[i] = array[i + 1];
                            array[i + 1] = temp;
                            swapped = true;
                        }
                    }
                    start = start + 1;
                }
            }
        }

        // --- БАЗА ЕТАЛОННИХ КОДІВ ---
        private Dictionary<string, Dictionary<string, string>> InitializeReferenceCodes()
        {
            return new Dictionary<string, Dictionary<string, string>>
            {
                ["C#"] = new Dictionary<string, string>
                {
                    ["Selection Sort"] = @"using System;
class ExampleClass {
    public static void SelectionSort(int[] array) {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++) {
            int min_idx = i;
            for (int j = i + 1; j < n; j++) {
                if (array[j] < array[min_idx]) {
                    min_idx = j;
                }
            }
            int temp = array[min_idx];
            array[min_idx] = array[i];
            array[i] = temp;
        }
    }
}",
                    ["Shaker Sort"] = @"using System;
class ExampleClass {
    public static void ShakerSort(int[] array) {
        bool swapped = true;
        int start = 0;
        int end = array.Length;
        while (swapped == true) {
            swapped = false;
            for (int i = start; i < end - 1; ++i) {
                if (array[i] > array[i + 1]) {
                    int temp = array[i];
                    array[i] = array[i + 1];
                    array[i + 1] = temp;
                    swapped = true;
                }
            }
            if (swapped == false) break;
            swapped = false;
            end = end - 1;
            for (int i = end - 1; i >= start; i--) {
                if (array[i] > array[i + 1]) {
                    int temp = array[i];
                    array[i] = array[i + 1];
                    array[i + 1] = temp;
                    swapped = true;
                }
            }
            start = start + 1;
        }
    }
}"
                },
                ["Python"] = new Dictionary<string, string>
                {
                    ["Selection Sort"] = @"def selection_sort(arr):
    n = len(arr)
    for i in range(n - 1):
        min_idx = i
        for j in range(i + 1, n):
            if arr[j] < arr[min_idx]:
                min_idx = j
        temp = arr[min_idx]
        arr[min_idx] = arr[i]
        arr[i] = temp",
                    ["Shaker Sort"] = @"def shaker_sort(arr):
    n = len(arr)
    swapped = True
    start = 0
    end = n
    while swapped == True:
        swapped = False
        for i in range(start, end - 1):
            if arr[i] > arr[i + 1]:
                temp = arr[i]
                arr[i] = arr[i + 1]
                arr[i + 1] = temp
                swapped = True
        if swapped == False:
            break
        swapped = False
        end = end - 1
        for i in range(end - 1, start - 1, -1):
            if arr[i] > arr[i + 1]:
                temp = arr[i]
                arr[i] = arr[i + 1]
                arr[i + 1] = temp
                swapped = True
        start = start + 1"
                },
                ["Java"] = new Dictionary<string, string>
                {
                    ["Selection Sort"] = @"public static void selectionSort(int[] array) {
    int n = array.length;
    for (int i = 0; i < n - 1; i++) {
        int min_idx = i;
        for (int j = i + 1; j < n; j++) {
            if (array[j] < array[min_idx]) {
                min_idx = j;
            }
        }
        int temp = array[min_idx];
        array[min_idx] = array[i];
        array[i] = temp;
    }
}",
                    ["Shaker Sort"] = @"public static void shakerSort(int[] array) {
    boolean swapped = true;
    int start = 0;
    int end = array.length;
    while (swapped == true) {
        swapped = false;
        for (int i = start; i < end - 1; ++i) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = true;
            }
        }
        if (swapped == false) break;
        swapped = false;
        end = end - 1;
        for (int i = end - 1; i >= start; i--) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = true;
            }
        }
        start = start + 1;
    }
}"
                },
                ["C"] = new Dictionary<string, string>
                {
                    ["Selection Sort"] = @"void selectionSort(int array[], int n) {
    for (int i = 0; i < n - 1; i++) {
        int min_idx = i;
        for (int j = i + 1; j < n; j++) {
            if (array[j] < array[min_idx]) {
                min_idx = j;
            }
        }
        int temp = array[min_idx];
        array[min_idx] = array[i];
        array[i] = temp;
    }
}",
                    ["Shaker Sort"] = @"void shakerSort(int array[], int n) {
    int swapped = 1;
    int start = 0;
    int end = n;
    while (swapped == 1) {
        swapped = 0;
        for (int i = start; i < end - 1; ++i) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = 1;
            }
        }
        if (swapped == 0) break;
        swapped = 0;
        end = end - 1;
        for (int i = end - 1; i >= start; i--) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = 1;
            }
        }
        start = start + 1;
    }
}"
                },
                ["C++"] = new Dictionary<string, string>
                {
                    ["Selection Sort"] = @"void selectionSort(int array[], int n) {
    for (int i = 0; i < n - 1; i++) {
        int min_idx = i;
        for (int j = i + 1; j < n; j++) {
            if (array[j] < array[min_idx]) {
                min_idx = j;
            }
        }
        int temp = array[min_idx];
        array[min_idx] = array[i];
        array[i] = temp;
    }
}",
                    ["Shaker Sort"] = @"void shakerSort(int array[], int n) {
    bool swapped = true;
    int start = 0;
    int end = n;
    while (swapped == true) {
        swapped = false;
        for (int i = start; i < end - 1; ++i) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = true;
            }
        }
        if (swapped == false) break;
        swapped = false;
        end = end - 1;
        for (int i = end - 1; i >= start; i--) {
            if (array[i] > array[i + 1]) {
                int temp = array[i];
                array[i] = array[i + 1];
                array[i + 1] = temp;
                swapped = true;
            }
        }
        start = start + 1;
    }
}"
                }
            };
        }
    }
}