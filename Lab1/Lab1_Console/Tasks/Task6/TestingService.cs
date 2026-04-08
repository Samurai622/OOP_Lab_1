using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lab1_Task6
{
    public static class TestingService
    {
        private static readonly object consoleLock = new object(); // Для потокобезпеки In/Out консолі

        // Головний метод, який викликається з MainWindow
        public static void RunCheck(string studentCode, string lang, string algo, Action<string, bool, bool> updateUi)
        {
            try
            {
                if (lang == "C#") CheckCSharpCodeExecution(studentCode, algo, updateUi);
                else CheckExternalLanguageExecution(studentCode, lang, algo, updateUi);
            }
            catch (Exception ex)
            {
                updateUi($"Критична помилка перевірки: {ex.Message}", false, false);
            }
        }

        private static void CheckCSharpCodeExecution(string code, string algorithm, Action<string, bool, bool> updateUi)
        {
            try
            {
                // Компілюємо студентський код та отримуємо метод Main
                MethodInfo studentMain = CompileAndGetMain(code);
                        
                // Очікується, що в ReferenceData.Codes["C#"][algo] лежить ПОВНИЙ еталонний C# код з Main
                string refCode = ReferenceData.Codes["C#"][algorithm];
                MethodInfo refMain = CompileAndGetMain(refCode);

                Func<MethodInfo, Func<int[], long>> createExecuter = (mainMethod) => (arr) =>
                {
                    string inputStr = arr.Length + "\n" + string.Join(" ", arr);
                    using var sr = new StringReader(inputStr);
                    using var swOut = new StringWriter();

                    long timeElapsed;
                    lock (consoleLock) // Блокуємо доступ до консолі на випадок паралельних перевірок
                    {
                        var oldIn = Console.In;
                        var oldOut = Console.Out;
                        try
                        {
                            Console.SetIn(sr);
                            Console.SetOut(swOut);

                            var sw = Stopwatch.StartNew();
                            
                            // ВИПРАВЛЕННЯ: Перевіряємо, чи приймає метод Main параметри (args)
                            var parameters = mainMethod.GetParameters();
                            if (parameters.Length == 0)
                            {
                                mainMethod.Invoke(null, null); // Виклик без параметрів
                            }
                            else
                            {
                                mainMethod.Invoke(null, new object[] { Array.Empty<string>() }); // Виклик з масивом args
                            }
                            
                            sw.Stop();
                            timeElapsed = sw.ElapsedMilliseconds;
                        }
                        finally
                        {
                            Console.SetIn(oldIn);
                            Console.SetOut(oldOut);
                        }
                    }

                    // Парсимо вивід як масив цілих чисел
                    var tokens = swOut.ToString().Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    var sortedResult = tokens.Select(int.Parse).ToArray();
                    Array.Copy(sortedResult, arr, sortedResult.Length);

                    return timeElapsed;
                };

                RunTestsAndVerify(createExecuter(studentMain), createExecuter(refMain), "C#", updateUi);
            }
            catch (Exception ex)
            {
                // Якщо це помилка рефлексії (наприклад, TargetInvocationException), дістаємо реальну причину
                string errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                updateUi($"Помилка компіляції/виконання: {errorMsg}", false, false);
            }
        }

        private static MethodInfo CompileAndGetMain(string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var assemblyName = Path.GetRandomFileName();
            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location)
            };

            // Компілюємо як ConsoleApplication, щоб гарантовано мати EntryPoint (Main)
            var options = new CSharpCompilationOptions(OutputKind.ConsoleApplication);
            var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees: new[] { syntaxTree }, references: references, options: options);

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            if (!result.Success)
            {
                var errors = result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(f => f.GetMessage());
                throw new Exception(string.Join("\n", errors));
            }

            ms.Seek(0, SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());
            return assembly.EntryPoint ?? throw new Exception("Не знайдено точку входу (метод Main).");
        }

        private static void CheckExternalLanguageExecution(string studentCode, string lang, string algo, Action<string, bool, bool> updateUi)
        {
            string baseTempDir = Path.Combine(Path.GetTempPath(), "SortCheck_" + Guid.NewGuid().ToString("N"));
            string studentDir = Path.Combine(baseTempDir, "Student");
            string refDir = Path.Combine(baseTempDir, "Reference");

            Directory.CreateDirectory(studentDir);
            Directory.CreateDirectory(refDir);

            try
            {
                // Очікується, що еталонний код – це теж ПОВНА програма (з main та вводом-виводом)
                string refCode = ReferenceData.Codes[lang][algo];

                var studentExe = PrepareExecutable(studentCode, lang, studentDir);
                var refExe = PrepareExecutable(refCode, lang, refDir);

                Func<int[], long> studentExec = (arr) => RunProcessTest(studentExe.exePath, studentExe.args, arr, out int[] sortedResult);
                Func<int[], long> refExec = (arr) => RunProcessTest(refExe.exePath, refExe.args, arr, out int[] sortedResult);

                RunTestsAndVerify(studentExec, refExec, lang, updateUi);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                updateUi($"❌ Не знайдено інструменти для мови {lang}. Переконайтеся, що компілятор встановлений та доданий до PATH.", false, false);
            }
            catch (Exception ex)
            {
                updateUi($"Помилка виконання: {ex.Message}", false, false);
            }
            finally
            {
                if (Directory.Exists(baseTempDir)) Directory.Delete(baseTempDir, true);
            }
        }

        private static (string exePath, string args) PrepareExecutable(string code, string lang, string dir)
        {
            if (lang == "Python")
            {
                string scriptPath = Path.Combine(dir, "script.py");
                File.WriteAllText(scriptPath, code);
                return ("python", $"\"{scriptPath}\"");
            }
            else if (lang == "Java")
            {
                // Очікується, що публічний клас програми користувача називається Main
                string scriptPath = Path.Combine(dir, "Main.java");
                File.WriteAllText(scriptPath, code);
                
                var compileProc = Process.Start(new ProcessStartInfo { FileName = "javac", Arguments = $"\"{scriptPath}\"", CreateNoWindow = true, UseShellExecute = false, RedirectStandardError = true });
                compileProc?.WaitForExit();
                if (compileProc?.ExitCode != 0) throw new Exception($"Помилка компіляції Java: {compileProc?.StandardError.ReadToEnd()}");

                return ("java", $"-cp \"{dir}\" Main");
            }
            else if (lang == "C" || lang == "C++")
            {
                string ext = lang == "C" ? "c" : "cpp";
                string compiler = lang == "C" ? "gcc" : "g++";
                string scriptPath = Path.Combine(dir, $"source.{ext}");
                string outPath = Path.Combine(dir, "program.exe");

                File.WriteAllText(scriptPath, code);
                
                var compileProc = Process.Start(new ProcessStartInfo { FileName = compiler, Arguments = $"-O2 \"{scriptPath}\" -o \"{outPath}\"", CreateNoWindow = true, UseShellExecute = false, RedirectStandardError = true });
                compileProc?.WaitForExit();
                if (compileProc?.ExitCode != 0) throw new Exception($"Помилка компіляції: {compileProc?.StandardError.ReadToEnd()}");

                return (outPath, "");
            }
            throw new Exception("Невідома мова.");
        }

        private static long RunProcessTest(string exePath, string args, int[] inputArray, out int[] sortedResult)
        {
            var psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = args,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = new Process { StartInfo = psi };
            
            var sw = Stopwatch.StartNew(); // Стартуємо таймер ДО запуску процесу
            proc.Start();

            using (var stdin = proc.StandardInput)
            {
                stdin.WriteLine(inputArray.Length);
                stdin.WriteLine(string.Join(" ", inputArray));
            }

            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit(15000); // Ліміт часу збільшено для врахування ініціалізації середовища (JVM/CLR)

            if (!proc.HasExited) 
            { 
                proc.Kill(); 
                throw new Exception("Перевищено ліміт часу (Time Limit)."); 
            }
            
            sw.Stop(); // Зупиняємо таймер ПІСЛЯ повного завершення роботи програми

            // Безпечно парсимо вивід (шукаємо лише числа, пробіли та переноси рядків)
            var tokens = output.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            sortedResult = tokens.Select(int.Parse).ToArray();

            Array.Copy(sortedResult, inputArray, sortedResult.Length);
            
            return sw.ElapsedMilliseconds;
        }

        private static void RunTestsAndVerify(Func<int[], long> studentExecuter, Func<int[], long> refExecuter, string lang, Action<string, bool, bool> updateUi)
        {
            Func<long, long, bool> isTimeCommensurate = (tEta, tStud) =>
            {
                long lowerBound = Math.Max(0, (tEta / 2) - 50);
                long upperBound = (2 * tEta) + 50;
                return tStud >= lowerBound && tStud <= upperBound;
            };

            int largeSize = lang == "Python" ? 2000 : 15000;
            List<int[]> testCases = GenerateTestCases(largeSize);

            bool allTestsPassed = true;
            string errorMessage = "";
            int testNumber = 1;

            foreach (var originalArray in testCases)
            {
                int[] studArray = (int[])originalArray.Clone();
                int[] refArray = (int[])originalArray.Clone();
                int[] expectedArray = (int[])originalArray.Clone();
                Array.Sort(expectedArray);

                long tEta = refExecuter(refArray);
                long tStud = studentExecuter(studArray);

                if (!expectedArray.SequenceEqual(studArray))
                {
                    allTestsPassed = false;
                    errorMessage = $"Помилка на тесті {testNumber} (розмір {originalArray.Length}): Результат сортування не збігається з еталонним.";
                    break;
                }

                if (!isTimeCommensurate(tEta, tStud))
                {
                    allTestsPassed = false;
                    errorMessage = $"Помилка на тесті {testNumber} (розмір {originalArray.Length}): Тривалість виконання неспівмірна.\n" +
                                   $"Еталон (вкл. I/O): {tEta} мс, Ваш код: {tStud} мс.\n" +
                                   $"(Ймовірно, використано неправильний алгоритм).";
                    break;
                }

                testNumber++;
            }

            if (allTestsPassed)
                updateUi($"✅ Успіх! Усі {testCases.Count} тестів пройдено.\nЧас виконання співмірний із еталоном для {lang}.", true, false);
            else
                updateUi($"❌ {errorMessage}", false, false);
        }

        private static List<int[]> GenerateTestCases(int largeArraySize)
        {
            Random rnd = new Random(42);
            return new List<int[]>
            {
                Array.Empty<int>(),
                new int[] { 5, 2, 9, 1, 5, 6 },
                Enumerable.Range(1, largeArraySize).Select(_ => rnd.Next(0, 10000)).ToArray(),
                Enumerable.Range(1, largeArraySize).ToArray(),
                Enumerable.Range(1, largeArraySize).Reverse().ToArray()
            };
        }
    }
}