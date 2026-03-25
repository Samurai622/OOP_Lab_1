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
            string expectedMethodName = algorithm == "Selection Sort" ? "SelectionSort" : "ShakerSort";
            try
            {
                MethodInfo studentMethod = CompileAndGetMethod(code);
                MethodInfo refMethod = typeof(ReferenceData.Methods).GetMethod(expectedMethodName)!;

                Func<int[], long> studentExec = (arr) =>
                {
                    var sw = Stopwatch.StartNew();
                    studentMethod.Invoke(null, new object[] { arr });
                    sw.Stop();
                    return sw.ElapsedMilliseconds;
                };

                Func<int[], long> refExec = (arr) =>
                {
                    var sw = Stopwatch.StartNew();
                    refMethod.Invoke(null, new object[] { arr });
                    sw.Stop();
                    return sw.ElapsedMilliseconds;
                };

                RunTestsAndVerify(studentExec, refExec, "C#", updateUi);
            }
            catch (Exception ex)
            {
                updateUi($"Помилка компіляції/виконання: {ex.Message}", false, false);
            }
        }

        private static MethodInfo CompileAndGetMethod(string rawMethodCode)
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
            var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees: new[] { syntaxTree }, references: references, options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);
            if (!result.Success) throw new Exception(string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).Select(f => f.GetMessage())));

            ms.Seek(0, SeekOrigin.Begin);
            Assembly assembly = Assembly.Load(ms.ToArray());
            foreach (Type type in assembly.GetTypes())
            {
                foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                {
                    var parameters = m.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(int[]) && m.Name.Contains("Sort", StringComparison.OrdinalIgnoreCase))
                        return m;
                }
            }
            throw new Exception("Не знайдено методу сортування.");
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
                string refCode = ReferenceData.Codes[lang][algo];
                string methodName = GetMethodNameForLang(lang, algo);

                var studentExe = BuildExternalLanguageExecutable(studentCode, lang, methodName, studentDir, "Solution");
                var refExe = BuildExternalLanguageExecutable(refCode, lang, methodName, refDir, "Solution");

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

        private static (string exePath, string args) BuildExternalLanguageExecutable(string code, string lang, string methodName, string dir, string className)
        {
            if (lang == "Python")
            {
                string scriptPath = Path.Combine(dir, "script.py");
                string wrapper = $@"
import sys, time
{code}
def run():
    data = sys.stdin.read().split()
    if not data: return
    n = int(data[0])
    arr = [int(x) for x in data[1:]]
    start = time.time()
    {methodName}(arr)
    end = time.time()
    print(f'TIME:{{int((end-start)*1000)}}')
    print(' '.join(str(x) for x in arr))
if __name__ == '__main__': run()";
                File.WriteAllText(scriptPath, wrapper);
                return ("python", $"\"{scriptPath}\"");
            }
            else if (lang == "Java")
            {
                string scriptPath = Path.Combine(dir, $"{className}.java");
                string wrapper = $@"
import java.util.*;
public class {className} {{
    {code}
    public static void main(String[] args) {{
        Scanner sc = new Scanner(System.in);
        if (!sc.hasNextInt()) return;
        int n = sc.nextInt();
        int[] arr = new int[n];
        for (int i = 0; i < n; i++) arr[i] = sc.nextInt();
        long start = System.currentTimeMillis();
        {methodName}(arr);
        long end = System.currentTimeMillis();
        System.out.println(""TIME:"" + (end - start));
        for(int i=0; i<n; i++) System.out.print(arr[i] + "" "");
    }}
}}";
                File.WriteAllText(scriptPath, wrapper);
                return ("java", $"\"{scriptPath}\"");
            }
            else if (lang == "C" || lang == "C++")
            {
                string ext = lang == "C" ? "c" : "cpp";
                string compiler = lang == "C" ? "gcc" : "g++";
                string scriptPath = Path.Combine(dir, $"source.{ext}");
                string outPath = Path.Combine(dir, "program.exe");

                string includes = lang == "C"
                    ? "#include <stdio.h>\n#include <stdlib.h>\n#include <time.h>\n"
                    : "#include <iostream>\n#include <vector>\n#include <chrono>\nusing namespace std;\n";

                string mainFuncC = $@"
int main() {{
    int n; if (scanf(""%d"", &n) != 1) return 0;
    int* arr = (int*)malloc(n * sizeof(int));
    for (int i = 0; i < n; i++) scanf(""%d"", &arr[i]);
    clock_t start = clock();
    {methodName}(arr, n);
    clock_t end = clock();
    printf(""TIME:%ld\n"", (end - start) * 1000 / CLOCKS_PER_SEC);
    for(int i=0; i<n; i++) printf(""%d "", arr[i]);
    free(arr); return 0;
}}";
                string mainFuncCpp = $@"
int main() {{
    int n; if (!(cin >> n)) return 0;
    vector<int> arr(n);
    for (int i = 0; i < n; i++) cin >> arr[i];
    auto start = chrono::high_resolution_clock::now();
    {methodName}(arr.data(), n);
    auto end = chrono::high_resolution_clock::now();
    auto dur = chrono::duration_cast<chrono::milliseconds>(end - start).count();
    cout << ""TIME:"" << dur << ""\n"";
    for(int i=0; i<n; i++) cout << arr[i] << "" "";
    return 0;
}}";
                string wrapper = includes + code + (lang == "C" ? mainFuncC : mainFuncCpp);
                File.WriteAllText(scriptPath, wrapper);

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
            proc.Start();

            using (var sw = proc.StandardInput)
            {
                sw.WriteLine(inputArray.Length);
                sw.WriteLine(string.Join(" ", inputArray));
            }

            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit(10000);
            if (!proc.HasExited) { proc.Kill(); throw new Exception("Перевищено ліміт часу (Time Limit)."); }

            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            string timeLine = lines.FirstOrDefault(l => l.StartsWith("TIME:")) ?? "TIME:-1";
            long time = long.Parse(timeLine.Replace("TIME:", "").Trim());

            string arrLine = lines.LastOrDefault(l => !l.StartsWith("TIME:")) ?? "";
            sortedResult = arrLine.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            Array.Copy(sortedResult, inputArray, sortedResult.Length);
            return time;
        }

        private static string GetMethodNameForLang(string lang, string algo)
        {
            if (lang == "Python") return algo == "Selection Sort" ? "selection_sort" : "shaker_sort";
            return algo == "Selection Sort" ? "selectionSort" : "shakerSort";
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
                                   $"Еталон: {tEta} мс, Ваш код: {tStud} мс.\n" +
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
                new int[0],
                new int[] { 5, 2, 9, 1, 5, 6 },
                Enumerable.Range(1, largeArraySize).Select(_ => rnd.Next(0, 10000)).ToArray(),
                Enumerable.Range(1, largeArraySize).ToArray(),
                Enumerable.Range(1, largeArraySize).Reverse().ToArray()
            };
        }
    }
}